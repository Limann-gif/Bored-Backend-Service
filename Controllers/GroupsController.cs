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

    [HttpGet("getAllGroupsActivityHistory")]
    public async Task<IActionResult> GetAllGroupsActivityHistory()
    {
        var response = await _userRepository.GetGroupList();
        return Ok(response);
    }
}