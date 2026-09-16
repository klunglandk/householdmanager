using KermAdhd.Controllers;
using KermAdhd.Database;
using KermAdhd.Database.DataModels;
using Microsoft.EntityFrameworkCore;

namespace KermAdhd.Services;

public interface IReminderService
{
    Task<List<ReminderItem>> GetAll();
    Task<ReminderItem> Create(CreateReminderRequest request);
}

public class ReminderService : IReminderService
{
    public ReminderService(AdhdContext context)
    {
        _context = context;
    }

    private readonly AdhdContext _context;

    public async Task<List<ReminderItem>> GetAll()
    {
        return await (_context.Reminders
            .Select(r =>
            new ReminderItem
            {
                Id = r.Id,
                Name = r.Name,
                CompletedAt = r.CompletedAt,
                CreatedAt = r.CreatedAt,
                IsCompleted = r.IsCompleted
            }).ToListAsync());
    }

    public async Task<ReminderItem> Create(CreateReminderRequest request)
    {
        var result = _context.Reminders.Add(new Reminder { Name = request.Name });
        await _context.SaveChangesAsync();

        return new ReminderItem { 
            Name = result.Entity.Name, 
            CompletedAt = result.Entity.CompletedAt, 
            CreatedAt = result.Entity.CreatedAt, 
            Id = result.Entity.Id, 
            IsCompleted = result.Entity.IsCompleted 
        };
    }
}
