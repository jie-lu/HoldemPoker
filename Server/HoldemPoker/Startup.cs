using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoldemPoker.Configurations;
using HoldemPoker.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HoldemPoker
{
	public class Startup
	{
		private readonly string[] _allowedOrigins;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			_allowedOrigins = Configuration["AllowedOrigins"].Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<Game>();
			services.AddSingleton<PlayerAgent>();
			services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(builder =>
				{
					builder.WithOrigins(_allowedOrigins)
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials();
				});
			});
			services.AddAuthentication(options =>
				{
					// Identity made Cookie authentication the default.
					// However, we want JWT Bearer Auth to be the default.
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateLifetime = true,
						ValidateIssuer = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = Configuration["Jwt:Issuer"],
						ValidateAudience = false,
						//ValidAudience = Configuration["Jwt:Issuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
					};

					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];

							// If the request is for our hub...
							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) &&
								 (path.StartsWithSegments("/game")))
							{
								// Read the token out of the query string
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						}
					};
				});
			
			services.AddControllers();
			services.AddSignalR();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors();
			app.UseAuthentication();
			app.UseAuthorization();
			

			var wsOptions = new WebSocketOptions();
			foreach (var origin in _allowedOrigins)
			{
				wsOptions.AllowedOrigins.Add(origin);
			}
			app.UseWebSockets(wsOptions);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<GameHub>("/game");
			});

			// Trigger the constructor of PlayerAgent so it will hook up the singleton Game instance.
			app.ApplicationServices.GetService<PlayerAgent>();
		}
	}
}
