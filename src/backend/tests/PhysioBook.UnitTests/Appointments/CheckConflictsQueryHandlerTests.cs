using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using PhysioBook.Application.Appointments.Queries;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Domain.Entities;
using System.Linq.Expressions;

namespace PhysioBook.UnitTests.Appointments;

public class CheckConflictsQueryHandlerTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _therapistId = Guid.NewGuid();

    private static IApplicationDbContext CreateMockContext(List<Appointment> appointments)
    {
        var mockDbSet = MockDbSetFactory.Create(appointments);

        var mockContext = new Mock<IApplicationDbContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockDbSet);
        return mockContext.Object;
    }

    private Appointment CreateAppointment(
        DateTimeOffset start,
        DateTimeOffset end,
        string status = "scheduled",
        Guid? id = null,
        string? patientName = "Test Patient")
    {
        return new Appointment
        {
            Id = id ?? Guid.NewGuid(),
            TenantId = _tenantId,
            TherapistId = _therapistId,
            TreatmentTypeId = Guid.NewGuid(),
            PatientName = patientName,
            StartTime = start,
            EndTime = end,
            Status = status,
        };
    }

    [Fact]
    public async Task Handle_NoOverlap_ReturnsNoConflict()
    {
        var existing = CreateAppointment(
            DateTimeOffset.UtcNow.AddHours(1),
            DateTimeOffset.UtcNow.AddHours(2));

        var context = CreateMockContext([existing]);
        var handler = new CheckConflictsQueryHandler(context);

        var query = new CheckConflictsQuery(
            _therapistId,
            DateTimeOffset.UtcNow.AddHours(3),
            DateTimeOffset.UtcNow.AddHours(4));

        var result = await handler.Handle(query, CancellationToken.None);

        result.HasConflict.Should().BeFalse();
        result.ConflictingAppointments.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Overlap_ReturnsConflict()
    {
        var start = DateTimeOffset.UtcNow.AddHours(1);
        var end = DateTimeOffset.UtcNow.AddHours(2);
        var existing = CreateAppointment(start, end, patientName: "Jane Doe");

        var context = CreateMockContext([existing]);
        var handler = new CheckConflictsQueryHandler(context);

        var query = new CheckConflictsQuery(
            _therapistId,
            start.AddMinutes(30),
            end.AddMinutes(30));

        var result = await handler.Handle(query, CancellationToken.None);

        result.HasConflict.Should().BeTrue();
        result.ConflictingAppointments.Should().HaveCount(1);
        result.ConflictingAppointments[0].PatientName.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task Handle_ExcludeAppointmentId_ExcludesFromResults()
    {
        var start = DateTimeOffset.UtcNow.AddHours(1);
        var end = DateTimeOffset.UtcNow.AddHours(2);
        var appointmentId = Guid.NewGuid();
        var existing = CreateAppointment(start, end, id: appointmentId);

        var context = CreateMockContext([existing]);
        var handler = new CheckConflictsQueryHandler(context);

        var query = new CheckConflictsQuery(
            _therapistId,
            start,
            end,
            ExcludeAppointmentId: appointmentId);

        var result = await handler.Handle(query, CancellationToken.None);

        result.HasConflict.Should().BeFalse();
        result.ConflictingAppointments.Should().BeEmpty();
    }

    [Theory]
    [InlineData("cancelled")]
    [InlineData("no_show")]
    public async Task Handle_CancelledOrNoShow_IgnoredInConflictCheck(string status)
    {
        var start = DateTimeOffset.UtcNow.AddHours(1);
        var end = DateTimeOffset.UtcNow.AddHours(2);
        var existing = CreateAppointment(start, end, status: status);

        var context = CreateMockContext([existing]);
        var handler = new CheckConflictsQueryHandler(context);

        var query = new CheckConflictsQuery(
            _therapistId,
            start,
            end);

        var result = await handler.Handle(query, CancellationToken.None);

        result.HasConflict.Should().BeFalse();
        result.ConflictingAppointments.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ConflictingAppointment_ReturnsCorrectDetails()
    {
        var start = DateTimeOffset.UtcNow.AddHours(1);
        var end = DateTimeOffset.UtcNow.AddHours(2);
        var appointmentId = Guid.NewGuid();
        var existing = CreateAppointment(start, end, id: appointmentId, patientName: "John Smith");
        existing.Status = "confirmed";

        var context = CreateMockContext([existing]);
        var handler = new CheckConflictsQueryHandler(context);

        var query = new CheckConflictsQuery(
            _therapistId,
            start.AddMinutes(-30),
            end.AddMinutes(-30));

        var result = await handler.Handle(query, CancellationToken.None);

        result.HasConflict.Should().BeTrue();
        var conflict = result.ConflictingAppointments[0];
        conflict.Id.Should().Be(appointmentId);
        conflict.PatientName.Should().Be("John Smith");
        conflict.StartTime.Should().Be(start);
        conflict.EndTime.Should().Be(end);
        conflict.Status.Should().Be("confirmed");
    }
}

/// <summary>
/// Factory for creating mock DbSet instances that support async LINQ operations.
/// </summary>
internal static class MockDbSetFactory
{
    public static DbSet<T> Create<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var asyncProvider = new InMemoryAsyncQueryProvider<T>(queryable.Provider);

        var mockDbSet = new Mock<DbSet<T>>();
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(asyncProvider);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        mockDbSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => new InMemoryAsyncEnumerator<T>(queryable.GetEnumerator()));

        return mockDbSet.Object;
    }
}

internal sealed class InMemoryAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal InMemoryAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new InMemoryAsyncEnumerable<TEntity>(_inner.CreateQuery<TEntity>(expression), this);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new InMemoryAsyncEnumerable<TElement>(_inner.CreateQuery<TElement>(expression), this);
    }

    public object? Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: [typeof(Expression)])!
            .MakeGenericMethod(resultType)
            .Invoke(_inner, [expression]);

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(resultType)
            .Invoke(null, [executionResult])!;
    }
}

internal sealed class InMemoryAsyncEnumerable<T> : IAsyncEnumerable<T>, IOrderedQueryable<T>
{
    private readonly IQueryable<T> _inner;
    private readonly IQueryProvider _provider;

    public InMemoryAsyncEnumerable(IQueryable<T> inner, IQueryProvider provider)
    {
        _inner = inner;
        _provider = provider;
    }

    public Type ElementType => _inner.ElementType;
    public Expression Expression => _inner.Expression;
    public IQueryProvider Provider => _provider;

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new InMemoryAsyncEnumerator<T>(_inner.GetEnumerator());
    }

    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _inner.GetEnumerator();
}

internal sealed class InMemoryAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public InMemoryAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }
}
