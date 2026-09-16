using KermAdhd.Database;
using KermAdhd.Database.DataModels;
using KermAdhd.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KermAdhd.Controllers;

public class ReminderItem
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public bool IsCompleted { get; init; }
}

public class CreateReminderRequest
{
    [Required, MinLength(1)]
    public string Name { get; set; } = string.Empty;
}

[Route("api/[controller]")]
[ApiController]
public class ReminderController : ControllerBase
{
    public ReminderController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    private readonly IReminderService _reminderService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {


        return Ok(await _reminderService.GetAll());
            
    }

    [HttpPost]
    public async Task<IActionResult> AddReminder([FromBody]CreateReminderRequest request)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _reminderService.Create(request);

        return Created($"api/reminder/{result.Id}", result);
    }
}
