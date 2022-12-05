using System;
namespace YtmoviesApi.Model
{
	public class TokenInfo
	{

		public string Id { get; set; }

		public string Username { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpire { get; set; }

       

    }
}

