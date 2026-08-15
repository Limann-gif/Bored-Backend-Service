using BoredWeb.Models;
using BoredWeb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BoredWeb.Controllers;

[Route("api/[controller]")]
[ApiController]

public class GroupsController: ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public GroupsController(IConfiguration config, ILogger<UserController> logger, IUserRepository userRepository)
    {
        _config = config;
        _logger = logger;
        _userRepository = userRepository;
    }

    //ADMIN Endpoint
    [HttpGet("getAllGroupsActivityHistory")]
    public async Task<IActionResult> GetAllGroupsActivityHistory()
    {
        var response = await _userRepository.GetGroupList();
        return Ok(response);
    }
    
    [HttpPost("updateGroupActivity")]
    public async Task<IActionResult> UpdateGroupActivity(ActivityProgressDto activity)
    {
        var response = await _userRepository.ManageGroupActivtyProgress(activity);
        return Ok(response);
    }
    
    [HttpGet("getGroupMembers/{userId}")]
    public async Task<IActionResult> GetGroupMembers([FromRoute] Guid userId)
    {
        var response = await _userRepository.GroupMembers(userId);
        return Ok(response);
    }
   
}