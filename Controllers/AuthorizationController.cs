using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YtmoviesApi.Model;
using YtmoviesApi.Model.Domain;
using YtmoviesApi.Model.DTO;
using YtmoviesApi.Repository.Abstract;
using YtmoviesApi.Repository.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YtmoviesApi.Controllers
{
    [Route("api/[controller]/{action}")]
    public class AuthorizationController : Controller
    {
        private readonly DatabaseContext context;
        private readonly ITokenService service;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> rolemanager;

        public AuthorizationController(DatabaseContext context, ITokenService service,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> rolemanager
            )
        {
            this.context = context;
            this.service = service;
            this.userManager = userManager;
            this.rolemanager = rolemanager;
        }


        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);

            if(user !=null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };


            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            var token = service.GetToken(authClaims);
            var refreshToken = service.GetRefreshToken();
            var tokenInfo = context.TokenInfos.FirstOrDefault(a => a.Username == user.UserName);
            if (tokenInfo == null)
            {
                var info = new TokenInfo
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    RefreshTokenExpire = DateTime.Now.AddDays(1)
                };
                context.TokenInfos.Add(info);
            }

            else
            {
                tokenInfo.RefreshToken = refreshToken;
                tokenInfo.RefreshTokenExpire = DateTime.Now.AddDays(1);
            }
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new LoginResponse
            {
                Name = user.Name,
                Username = user.UserName,
                Token = token.TokenString,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                StatusCode = 1,
                Message = "Logged in"
            });

        }

        

            //login failed condition

            return Ok(
                new LoginResponse {
                    StatusCode = 0,
                    Message = "Invalid Username or Password",
                    Token = "", Expiration = null });
        }


        [HttpPost]
        public async Task<IActionResult> Registration([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return Ok(status);
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = string.Join(" ", result.Errors.Select(x => "User creation failed" + ":  " + x.Description));
                return Ok(status);
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await rolemanager.RoleExistsAsync(UserRoles.User))
                await rolemanager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await rolemanager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);

        }



        //admin commet after register admin
        [HttpPost]
        public async Task<IActionResult> RegistrationAdmin([FromBody] RegistrationModel model)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass all the required fields";
                return Ok(status);
            }
            // check if user exists
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                status.StatusCode = 0;
                status.Message = "Invalid username";
                return Ok(status);
            }
            var user = new ApplicationUser
            {
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = model.Email,
                Name = model.Name
            };
            // create a user here
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = string.Join(" ", result.Errors.Select(x => "Admin creation failed" + ":  " + x.Description));
                return Ok(status);
            }

            // add roles here
            // for admin registration UserRoles.Admin instead of UserRoles.Roles
            if (!await rolemanager.RoleExistsAsync(UserRoles.Admin))
                await rolemanager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (await rolemanager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            status.StatusCode = 1;
            status.Message = "Sucessfully registered";
            return Ok(status);

        }



        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var status = new Status();
            // check validations
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "please pass all the valid fields";
                return Ok(status);
            }
            // lets find the user
            var user = await userManager.FindByNameAsync(model.Username);
            if (user is null)
            {
                status.StatusCode = 0;
                status.Message = "invalid username";
                return Ok(status);
            }
            // check current password
            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                status.StatusCode = 0;
                status.Message = "invalid current password";
                return Ok(status);
            }

            // change password here
            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "Failed to change password";
                return Ok(status);
            }
            status.StatusCode = 1;
            status.Message = "Password has changed successfully";
            return Ok(result);
        }














    }
}

