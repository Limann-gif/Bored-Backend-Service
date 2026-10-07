using System.Security.Claims;
using BoredWeb.Models;
using BoredWeb.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BoredWeb.Controllers;


[Route("api/[controller]")]
[ApiController]

public class ActivitiesController: ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public ActivitiesController(IConfiguration config, ILogger<UserController> logger, IUserRepository userRepository)
    {
        _config = config;
        _logger = logger;
        _userRepository = userRepository;
    }
    
    // Get an activity
    [HttpGet("activity/{id}")]
    public async Task<IActionResult> GetActivity([FromRoute] Guid id)
    {
        var data = await _userRepository.GetActivityById(id);
        
        return Ok(data);
    }
    
    // Get list of activities
    [HttpGet("activityList")]
    public async Task<IActionResult> GetActivity()
    {
        var data = await _userRepository.GetActivitiesList();
        
        return Ok(data);
    }
    
    // In a real app, BCRYPT or Argon2 should be used to hash the password!
    //for ADMIN to add an activity
    [HttpPost("addActivity")]
    public async Task<IActionResult> AddActivity(Activity request)
    {
        var data = await _userRepository.AddActivity(request);
        return Ok(data);
    }
    
    // for ADMIN to delete an activity
    [HttpDelete("deleteActivity/{id}")]
    public async Task<IActionResult> DeleteActivity([FromRoute] string id)
    {
        // In a real app, BCRYPT or Argon2 should be used to hash the password!
        var data = await _userRepository.DeleteActivity(id);
        return Ok(data);
    }
    
    // for ADMIN to update an activity
    [HttpPost("updateActivity")]
    public async Task<IActionResult> UpdateActivity(Activity request)
    {
        // In a real app, BCRYPT or Argon2 should be used to hash the password!
        var data = await _userRepository.UpdateActivity(request);
        return Ok(data);
        
    }
    
    // Book and activity, pay later
    [HttpPost("bookActivity")]
    public async Task<IActionResult> BookActivity(BookingDto book)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("Invalid or missing user token.");

        book.UserId = userId;
        var data = await _userRepository.BookActivity(book);
        return Ok(data);
    }
    
    // Cancel a booked activity
    [HttpDelete("cancel/bookedActivity/{activityOrderId}")]
    public async Task<IActionResult> CancelBookedActivity([FromRoute] Guid activityOrderId)
    {
        var data = await _userRepository.CancelBookedActivity(activityOrderId);
        return Ok(data);
    }
    
    // Cancel a paid activity - send request to admin
    [HttpPost("cancel/paidActivity/{activityOrderId}")]
    public async Task<IActionResult> CancelPaidActivity([FromRoute] Guid activityOrderId)
    {
        var data = await _userRepository.CancelPaidActivity(activityOrderId);
        return Ok(data);
    }
    
    // Gets all activities booked for a user
    [HttpGet("getUserActivityHistory/{userId}")]
    public IActionResult GetUserActivityHistory([FromRoute] Guid userId)
    {
        var data = _userRepository.GetUserActivityHistory(userId);
        if (data.Result.Code != 200) return BadRequest("Failed to get user activity history");
        
        return Ok(data.Result);
        
    }
    
    
    // Get Refund requests for paid activities ADMIN Endpoint
    [HttpGet("refundRequests")]
    public async Task<IActionResult> GetRefundRequests()
    {
        var data = await _userRepository.GetRefundRequest();
        return Ok(data);
    }
    
    // // Refund a paid activity ADMIN Endpoint Make sure to update from paid to refunded after a successful refund
    // [HttpPost("refund/paidActivity")]
    // public async Task<IActionResult> BookActivity(BookingDto book)
    // {
    //     var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //     if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
    //         return Unauthorized("Invalid or missing user token.");
    //
    //     book.UserId = userId;
    //     var data = await _userRepository.BookActivity(book);
    //     return Ok(data);
    // }
}