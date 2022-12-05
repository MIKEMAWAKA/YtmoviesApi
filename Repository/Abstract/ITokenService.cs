using System;
using System.Security.Claims;
using YtmoviesApi.Model.DTO;

namespace YtmoviesApi.Repository.Abstract
{
	public interface ITokenService
	{
		TokenResponse GetToken(IEnumerable<Claim> claim);

		string GetRefreshToken();

		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
	}
}

