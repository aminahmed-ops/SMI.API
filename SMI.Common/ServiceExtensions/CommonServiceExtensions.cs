using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SMI.Common.Configuration;
using System.Text; 

namespace SMI.Common.ServiceExtensions
{
    public static class CommonServiceExtensions
    {
        public static IServiceCollection AddSMIAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<Jwt>(o =>
            {
                config.GetSection("JWT").Bind(o); 
            });
            var jwtSection = config.GetSection("JWT");
            services.Configure<Jwt>(jwtSection);
            var appSettings = jwtSection.Get<Jwt>();
            var secret = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = true;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = appSettings.ValidIssuer,
                    ValidAudience = appSettings.ValidAudience,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret)
                };

            });
            return services;
        }
    }
}
