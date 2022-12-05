using System;
using System.ComponentModel.DataAnnotations;

namespace YtmoviesApi.Model.DTO
{
	public class LoginModel
	{
        [Required]
        public string? Username { get; set; }

 

        [Required]
        public string? Password { get; set; }
    }
}

