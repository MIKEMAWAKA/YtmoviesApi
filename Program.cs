using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using YtmoviesApi.Model.Domain;
using System.Text;
using YtmoviesApi.Repository.Domain;
using YtmoviesApi.Repository.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//for using my sql database
builder.Services.AddDbContext<DatabaseContext>(options =>
  options.UseMySql(builder.Configuration.GetConnectionString("YmovietConnectionString"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("YmovietConnectionString")))

);

//For identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

//Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})

// Adding Jwt Bearer
.AddJwtBearer(options =>
 {
     options.SaveToken = true;
     options.RequireHttpsMetadata = false;
     options.TokenValidationParameters = new TokenValidationParameters()
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         //ValidateLifetime = true,
         //ValidateIssuerSigningKey = true,
         //ClockSkew = TimeSpan.Zero,

         ValidAudience = builder.Configuration["JWT:ValidAudience"],
         ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
     };
 });

builder.Services.AddTransient<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(
    options => options.WithOrigins("*")
    .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()


    );


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

