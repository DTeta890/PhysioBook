using MediatR;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentTypes.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentTypes.Commands;

public sealed record CreateTreatmentTypeCommand(
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    string? Color) : IRequest<TreatmentTypeDto>;

public sealed class CreateTreatmentTypeCommandHandler : IRequestHandler<CreateTreatmentTypeCommand, TreatmentTypeDto>
{
    private readonly IApplicationDbContext _context;

    public CreateTreatmentTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentTypeDto> Handle(CreateTreatmentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new TreatmentType
        {
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price,
            Color = request.Color,
            IsActive = true,
        };

        _context.TreatmentTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new TreatmentTypeDto(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.DurationMinutes,
            entity.Price,
            entity.Color,
            entity.IsActive,
            entity.CreatedAt);
    }
}
