using System.Security.Claims;
using AuraDecor.APIs.Dtos.Incoming;
using AuraDecor.APIs.Dtos.Outgoing;
using AuraDecor.APIs.Errors;
using AuraDecor.Core.Entities;
using AuraDecor.Core.Services.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuraDecor.APIs.Controllers;

[Authorize]
public class NotificationController : ApiBaseController
{
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public NotificationController(INotificationService notificationService, IMapper mapper)
    {
        _notificationService = notificationService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize);
        var notificationDtos = _mapper.Map<IReadOnlyList<NotificationDto>>(notifications);
        
        return Ok(notificationDtos);
    }

    [HttpGet("unread")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetUnreadNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
        var notificationDtos = _mapper.Map<IReadOnlyList<NotificationDto>>(notifications);
        
        return Ok(notificationDtos);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<NotificationSummaryDto>> GetNotificationSummary()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(userId);
        var recentNotifications = await _notificationService.GetUserNotificationsAsync(userId, 1, 5);
        
        var summary = new NotificationSummaryDto
        {
            UnreadCount = unreadCount,
            RecentNotifications = _mapper.Map<List<NotificationDto>>(recentNotifications)
        };
        
        return Ok(summary);
    }

    [HttpPut("{id}/mark-read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _notificationService.MarkAsReadAsync(id, userId);
        
        if (!result)
            return NotFound(new ApiResponse(404, "Notification not found or already read"));
            
        return Ok();
    }

    [HttpPut("mark-all-read")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _notificationService.MarkAllAsReadAsync(userId);
        
        if (!result)
            return BadRequest(new ApiResponse(400, "No unread notifications found"));
            
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNotification(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _notificationService.DeleteNotificationAsync(id, userId);
        
        if (!result)
            return NotFound(new ApiResponse(404, "Notification not found"));
            
        return Ok();
    }

    [HttpDelete("all")]
    public async Task<ActionResult> DeleteAllNotifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _notificationService.DeleteAllNotificationsAsync(userId);
        
        if (!result)
            return BadRequest(new ApiResponse(400, "No notifications found to delete"));
            
        return Ok();
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<NotificationPreferencesDto>> GetNotificationPreferences()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var preferences = await _notificationService.GetUserNotificationPreferencesAsync(userId);
        var preferencesDto = _mapper.Map<NotificationPreferencesDto>(preferences);
        
        return Ok(preferencesDto);
    }

    [HttpPut("preferences")]
    public async Task<ActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferencesDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var preferences = _mapper.Map<NotificationPreference>(dto);
        var result = await _notificationService.UpdateNotificationPreferencesAsync(userId, preferences);
        
        if (!result)
            return BadRequest(new ApiResponse(400, "Failed to update preferences"));
            
        return Ok();
    }

    // Admin endpoints
    [HttpPost("admin/create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        var notification = await _notificationService.CreateNotificationAsync(
            dto.UserId, dto.Title, dto.Message, (NotificationType)dto.Type, dto.RelatedEntityId, dto.RelatedEntityType);
        
        var notificationDto = _mapper.Map<NotificationDto>(notification);
        return CreatedAtAction(nameof(GetNotifications), new { id = notification.Id }, notificationDto);
    }

    [HttpPost("admin/bulk")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SendBulkNotification([FromBody] BulkNotificationDto dto)
    {
        if (dto.UserIds != null && dto.UserIds.Any())
        {
            await _notificationService.SendNotificationToUsersAsync(
                dto.UserIds, dto.Title, dto.Message, (NotificationType)dto.Type);
        }
        else
        {
            await _notificationService.SendNotificationToAllUsersAsync(
                dto.Title, dto.Message, (NotificationType)dto.Type);
        }
        
        return Ok();
    }
}