using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Mcshippers_Task.DTO;
using Mcshippers_Task.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

namespace Mcshippers_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private IMailRepository mailRepository;
        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration Configuration, IMailRepository _mailRepository)
        {
            this.userManager = userManager;
             configuration = Configuration;
            mailRepository = _mailRepository;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto) 
        {
            if (ModelState.IsValid==false)
            {
                return BadRequest(ModelState);
            }
            if(registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest("Please confirm the password");
            }
            //save data base
            ApplicationUser userModel = new ApplicationUser();
            userModel.UserName = registerDto.UserName;
            userModel.Email = registerDto.Email;
            IdentityResult result= await userManager.CreateAsync(userModel, registerDto.Password);
            if(result.Succeeded)
            {
                return Ok("Add Sucess");
            }else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return BadRequest(ModelState);
            }
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login( [FromForm] LoginDto loginDto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            //check identityt  "Create Token" ==Cookie
            ApplicationUser userModel= await userManager.FindByNameAsync(loginDto.UserName);
            if (userModel != null)
            {
                if(await userManager.CheckPasswordAsync(userModel, loginDto.Password) == true)
                {
                    //toke base on Claims "Name &Roles &id " +Jti ==>unique Key Token "String"
                    var mytoken =await GenerateToke(userModel);
                    return Ok(new { 
                        token=new JwtSecurityTokenHandler().WriteToken(mytoken) ,
                        expiration= mytoken.ValidTo
                    });
                }
                else
                {
                    //return BadRequest("User NAme and PAssword Not Valid");
                    return Unauthorized();//401
                }
            }
            return Unauthorized();
        }
        [NonAction]
        public async Task<JwtSecurityToken> GenerateToke(ApplicationUser userModel)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("Mcshippers", "1234"));//Custom
            claims.Add(new Claim(ClaimTypes.Name, userModel.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userModel.Id));
            var roles = await userManager.GetRolesAsync(userModel);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //Jti "Identifier Token
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            //---------------------------------(: Token :)--------------------------------------
            var key =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecrtKey"]));
            var mytoken = new JwtSecurityToken(
                audience: configuration["JWT:ValidAudience"],
                issuer: configuration["JWT:ValidIssuer"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials:
                       new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );
            return mytoken;
        }
        [HttpGet("ForgetPassword")]
        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await mailRepository.SendEmailAsync(email, "Reset Password", "<h1>Follow the instructions to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a></p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully!"
            };
        }
        [HttpPost("ResetPassword")]
        public async Task<UserManagerResponse> ResetPasswordAsync([FromForm] ResetPasswordViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email",
                };

            if (model.NewPassword != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Password doesn't match its confirmation",
                };

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Password has been reset successfully!",
                    IsSuccess = true,
                };

            return new UserManagerResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }
    }
}
