# T-14: Drag-and-Drop Calendar Scheduling with @dnd-kit

## Status: ⬜ Not Started
## Phase: 1 — Core Calendar
## Dependencies: T-12 (Appointments API), T-13 (Calendar UI base)
## Agents: frontend-developer

---

## Objective
Implement the core drag-and-drop functionality on the weekly calendar view. Users must be able to drag appointments between time slots and therapist columns to reschedule, and resize appointment blocks to change duration. This is THE most important feature of PhysioBook.

## User Stories

1. **As a receptionist**, I can drag an appointment card from 09:00 to 10:30 to reschedule it, and the API updates automatically.
2. **As a receptionist**, I can drag an appointment from Dr. A's column to Dr. B's column to change the assigned therapist.
3. **As a receptionist**, I can drag the bottom edge of an appointment to make it longer or shorter (resize).
4. **As a receptionist**, I see a red highlight when I try to drop an appointment onto an occupied slot (conflict).
5. **As a receptionist**, the drag feels smooth with a ghost preview showing where the appointment will land.
6. **As a therapist**, I see my calendar update in real-time when the receptionist moves my appointments (SignalR).

## Technical Implementation

### Libraries
- `@dnd-kit/core` — DndContext, useDraggable, useDroppable
- `@dnd-kit/sortable` — if needed for list reordering within a column
- `@dnd-kit/utilities` — CSS transform utilities

### Calendar Grid Structure
```
┌─────────┬──────────────┬──────────────┬──────────────┐
│  Time   │  Dr. Alban   │  Dr. Elira   │  Dr. Besnik  │
├─────────┼──────────────┼──────────────┼──────────────┤
│  08:00  │              │ ████████████ │              │
│  08:30  │ ████████████ │ ████████████ │              │
│  09:00  │ ████████████ │              │ ████████████ │
│  09:30  │              │              │ ████████████ │
│  10:00  │              │ ████████████ │              │
│  ...    │              │ ████████████ │              │
└─────────┴──────────────┴──────────────┴──────────────┘
```

### Drag Logic
1. Each appointment = `useDraggable` with data: `{ appointmentId, currentTherapistId, currentStartTime, currentEndTime }`
2. Each empty 30-min slot = `useDroppable` with data: `{ therapistId, timeSlot }`
3. On `onDragEnd`:
   - Calculate new `starts_at` and `ends_at` from the drop target
   - Calculate new `therapist_id` from the column
   - Call `PATCH /api/v1/appointments/:id` with optimistic update via TanStack Query
   - If API returns conflict (409), revert the optimistic update and show error toast
4. On `onDragOver`:
   - Highlight the target slot (green if available, red if occupied)
   - Show ghost preview of appointment in new position

### Resize Logic
1. Each appointment has a resize handle at the bottom edge
2. Dragging the handle snaps to 30-minute increments
3. On resize end: update `ends_at` via PATCH with optimistic update

### Optimistic Updates (TanStack Query)
```typescript
const moveAppointment = useMutation({
  mutationFn: (data: MoveAppointmentDto) => api.appointments.move(data),
  onMutate: async (newData) => {
    await queryClient.cancelQueries({ queryKey: ['appointments', weekDate] });
    const previous = queryClient.getQueryData(['appointments', weekDate]);
    queryClient.setQueryData(['appointments', weekDate], (old) => {
      // Move appointment to new position in the cached data
    });
    return { previous };
  },
  onError: (err, vars, context) => {
    queryClient.setQueryData(['appointments', weekDate], context.previous);
    toast.error('Could not move appointment — time slot is occupied');
  },
  onSettled: () => {
    queryClient.invalidateQueries({ queryKey: ['appointments', weekDate] });
  },
});
```

## Steps

- [ ] 1. Install and configure @dnd-kit packages
- [ ] 2. Create `DndCalendarContext` provider wrapping the calendar
- [ ] 3. Create `DraggableAppointment` component using `useDraggable`
- [ ] 4. Create `DroppableTimeSlot` component using `useDroppable`
- [ ] 5. Implement `onDragStart` — show ghost overlay, dim original
- [ ] 6. Implement `onDragOver` — highlight target slot with availability check
- [ ] 7. Implement `onDragEnd` — calculate new time/therapist, fire mutation
- [ ] 8. Implement optimistic update pattern with rollback
- [ ] 9. Add resize handles to appointment cards
- [ ] 10. Implement resize logic with 30-min snap
- [ ] 11. Add conflict visual feedback (red highlight + toast on drop failure)
- [ ] 12. Add keyboard accessibility (arrow keys to move, Enter to confirm)
- [ ] 13. Test on tablet (768px) — touch drag must work smoothly
- [ ] 14. Connect to SignalR for real-time updates from other users

## Verification
- Drag appointment between time slots → API called, calendar updates
- Drag appointment between therapist columns → therapist changes
- Drop on occupied slot → red flash, appointment returns to original position
- Resize → duration updates, API called
- Open two browser tabs → move in one, see update in other (SignalR)
- Test on tablet-sized viewport → touch drag works
- `npm run build` — no errors
- `npm run lint` — no errors

## Summary of Changes
<!-- Filled in after completion -->
