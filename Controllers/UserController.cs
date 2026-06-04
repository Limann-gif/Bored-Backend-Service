using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BoredWeb.Data;
using BoredWeb.Models;
using BoredWeb.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BoredWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<UserController> _logger;

    private readonly IUserRepository _userRepository;
    // For a real app, you'd use a Database Context here
    private static BoredDbContext  _dbContext;

    public UserController(IConfiguration config , ILogger<UserController> logger, IUserRepository userRepository, BoredDbContext dbContext)
    {
        _config = config;
        _logger = logger;
        _dbContext = dbContext;
        _userRepository = userRepository;
    }
    // In a real app, BCRYPT or Argon2 should be used to hash the password!
    
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(UserDto request)
    {
       var data = await _userRepository.AddUser(request);
       if (data != 1)
       {
           return BadRequest("SignUp Failed or User already exists");
       }
       return Ok("User created successfully");
    }

    [HttpPost("login")]
    public IActionResult Login(UserDto request)
    {
        var user = _dbContext.Users.FirstOrDefault(u=>u.Email==request.Email && u.PasswordHash == request.Password);
        
        if (user == null) return Unauthorized("Invalid credentials");

        var validUser = new UserDto
        {
            Email = user.Email,
            Password = user.PasswordHash,
            Username = user.Name,
            Role = user.Role
        };

        var token = GenerateJwtToken(validUser, user.Id);
        return Ok(new { token });
    }
    
    [HttpGet("getUser/{id}")]
    public async Task<IActionResult> GetUserDetails([FromRoute]string id)
    {
        // In a real app, BCRYPT or Argon2 should be used to hash the password!
        var data = await _userRepository.GetUserById(id);
        return Ok(data);
        
    }
    
    [HttpGet("getUsers")]
    public async Task<IActionResult> GetUserDetails()
    {
        // In a real app, BCRYPT or Argon2 should be used to hash the password!
        var data = await _userRepository.GetUsers();
        return Ok(data);
        
    }

    private string GenerateJwtToken(UserDto user, Guid userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

