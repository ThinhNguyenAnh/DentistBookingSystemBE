﻿using BokkingDentist.Constant;
using DentistBooking.Application.ClaimTokens;
using DentistBooking.Application.System.Users;
using DentistBooking.ViewModels.System.Dentists;
using DentistBooking.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DentisBooking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _userService.Authenticate(request);
            if (token == null)
            {
                return Ok(new
                {
                    code = 400,
                    Message = "Username or Password is Incorrect"
                });
            }

            DentistResponse response = new DentistResponse();
            try
            {
            var userPrincipalac = this.ValidateToken(token.AccessToken);

            }
            catch (Exception)
            {

                response.Code = "400";
                return Ok(response);
            }



            var userPrincipalrf = this.ValidateToken(token.RefreshToken);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(5),
                IsPersistent = true,
                AllowRefresh = true,
            };
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipalac, authProperties);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipalrf, authProperties);
            return Ok(token);
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            RegisterResponse rs = await _userService.Register(request);
            return Ok(rs);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] Token tokenModel)
        {
            var rs = await _userService.RefreshToken(tokenModel);

            return Ok(rs);
        }
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ0aGluaEBmcHQuZWR1LnZuIiwiaHR0cDovL3NjaGVtasMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6WyJ0aGluaCIsIlRIaW5oIl0sImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IiIsImV4cCI6MTY1MzczMzkyOSwiaXNzIjoiaHR0cHM6Ly9EZW50aXN0Qm9va2luZy5jb20udm4iLCJhdWQiOiJodHRwczovL0RlbnRpc3RCb29raW5nLmNvbS52biJ9.p-LVcOzybm4kplJdVYFt77_KXyiXrDgoFfqDqOOhQM0";

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            ClaimsPrincipal principal;
               principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out validatedToken);

            return principal;
        }
    }
}
