using System;
using Microsoft.AspNetCore.Identity;

namespace YtmoviesApi.Model.Domain
{
	public class ApplicationUser : IdentityUser
	{
		//public ApplicationUser()
		//{
		//}


		public string? Name { get; set; }

	}
}

