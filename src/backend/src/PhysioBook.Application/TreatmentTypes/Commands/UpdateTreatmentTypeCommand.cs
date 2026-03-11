using MediatR;
using Microsoft.EntityFrameworkCore;
using PhysioBook.Application.Common.Exceptions;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Application.TreatmentTypes.DTOs;
using PhysioBook.Domain.Entities;

namespace PhysioBook.Application.TreatmentTypes.Commands;

public sealed record UpdateTreatmentTypeCommand(
    Guid Id,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    string? Color,
    bool IsActive) : IRequest<TreatmentTypeDto>;

public sealed class UpdateTreatmentTypeCommandHandler : IRequestHandler<UpdateTreatmentTypeCommand, TreatmentTypeDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateTreatmentTypeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TreatmentTypeDto> Handle(UpdateTreatmentTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TreatmentTypes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(TreatmentType), request.Id);
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.DurationMinutes = request.DurationMinutes;
        entity.Price = request.Price;
        entity.Color = request.Color;
        entity.IsActive = request.IsActive;

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
