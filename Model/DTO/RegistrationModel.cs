using System;
using System.ComponentModel.DataAnnotations;

namespace YtmoviesApi.Model.DTO
{
	public class RegistrationModel
	{
        [Required]
        public string? Name { get; set; }

        [Required]
		public string? Username { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

    


        //public string? Username { get; set; }


        //public string? Username { get; set; }
    }
}

