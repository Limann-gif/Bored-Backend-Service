using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BoredWeb.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BoredWeb.Services;

public class UserService : IUserService
{
    private readonly IConfiguration _config;

    public UserService(IConfiguration config)
    {
        _config = config;
    }

  
}

