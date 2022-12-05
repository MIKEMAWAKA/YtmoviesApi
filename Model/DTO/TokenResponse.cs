using System;
namespace YtmoviesApi.Model.DTO
{
	public class TokenResponse
	{

        public string? TokenString { get; set; }

        public DateTime ValidTo { get; set; }
    }
}

