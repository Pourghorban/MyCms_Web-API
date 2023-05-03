﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyCmsWebApi2.Domain.Entities;
using MyCmsWebApi2.Domain.Enums;
using MyCmsWebApi2.Infrastructure.Extensions;
using MyCmsWebApi2.Presentations.Dtos.AuthenticationDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyCmsWebApi2.Presentations.Controllers.UserControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration Configuration;
        private readonly JwtTokenGenerator _jwtTokenGenerator;



        public RegistrationController(UserManager<ApplicationUser> userManager, IConfiguration configuration, JwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            Configuration = configuration;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Name = model.Name,
                FamilyName = model.FamilyName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (_userManager.Users.Count() == 1)
                {
                    // First user registered is made an Admin
                    await _userManager.AddToRoleAsync(user, "Admin");
                }

                return Ok(new { message = "Registration successful" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]

        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Check if user has Admin role
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                var tokenString = _jwtTokenGenerator.GenerateToken(user.UserName, user.Email, user.Id.ToString(), isAdmin);

                return Ok(new { token = tokenString });
            }

            return Unauthorized();
        }



    }
}
