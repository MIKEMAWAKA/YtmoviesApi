using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YtmoviesApi.Model.Domain;
using YtmoviesApi.Model.DTO;
using YtmoviesApi.Repository.Abstract;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YtmoviesApi.Controllers
{
    [Route("api/[controller]/{action}")]
    public class TokenController : ControllerBase
    {
        private readonly DatabaseContext context;
        private readonly ITokenService service;

        public TokenController(DatabaseContext context,ITokenService service)
        {
            this.context = context;
            this.service = service;
        }

        [HttpPost]
        public IActionResult Refresh([FromBody] RefreshTokenRequest tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = service.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = context.TokenInfos.SingleOrDefault(u => u.Username== username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpire <= DateTime.Now)
                return BadRequest("Invalid client request");
            var newAccessToken = service.GetToken(principal.Claims);
            var newRefreshToken = service.GetRefreshToken();
            user.RefreshToken = newRefreshToken;
            context.SaveChanges();
            return Ok(new RefreshTokenRequest()
            {
                AccessToken = newAccessToken.TokenString,
                RefreshToken = newRefreshToken
            });
        }
   

        //revoken is use for removing token enntry
        [HttpPost, Authorize]
        public IActionResult Revoke()
        {
            try
            {
                var username = User.Identity.Name;
                var user = context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (user is null)
                    return BadRequest();
                user.RefreshToken = null;
                context.SaveChanges();
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



    }
}
