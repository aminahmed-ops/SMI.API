using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SMI.AuthService.Interfaces;
using SMI.AuthService.Interfaces.Facebook;
using SMI.AuthService.Services;
using SMI.AuthService.Services.Facebook;
using SMI.DataAccess.Context;
using SMI.Entities.Entities;
using SMI.Util.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequiredLength = 8;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();


builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(24);
});




builder.Services.AddScoped<IFacebookAuthService, FacebookAuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.Configure<FacebookAuthConfig>(builder.Configuration.GetSection("Facebook"));

builder.Services.AddHttpClient("Facebook", c =>
{
    c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Facebook:BaseUrl"));
});

var jwtSection = builder.Configuration.GetSection("JWT");
builder.Services.Configure<Jwt>(jwtSection);

var appSettings = jwtSection.Get<Jwt>();
var secret = Encoding.ASCII.GetBytes(appSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;

})
    .AddJwtBearer(o =>
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

})

.AddFacebook(options =>
{
    options.AppId = builder.Configuration.GetValue<string>("Facebook:AppId");//"852130150420508";
    options.AppSecret = builder.Configuration.GetValue<string>("Facebook:AppSecret");// "78d6e9a7276cbb126a0eda1d2bca335a";
    options.CallbackPath = builder.Configuration.GetValue<string>("Facebook:Callbackurl");
    //options.Scope.Add("email");
    //options.Scope.Add("public_profile");
    options.SaveTokens = true;  // Ensures tokens are stored
    
    options.Events = new OAuthEvents
    {
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/Home/Error?FailureMessage=" + context.Failure.Message);
            context.HandleResponse();
            return Task.CompletedTask;
        }
        
        
    };
   
})
.AddGoogle(options =>
{
    options.ClientId = "your_google_client_id";
    options.ClientSecret = "your_google_client_secret";
})
.AddMicrosoftAccount(options =>
{
    options.ClientId = "your_microsoft_client_id";
    options.ClientSecret = "your_microsoft_client_secret";
})
.AddTwitter(options =>
{
    options.ConsumerKey = "your_twitter_consumer_key";
    options.ConsumerSecret = "your_twitter_consumer_secret";
})
.AddCookie();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("Manager"));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7251")  // or use your specific client URL
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();  // Important if authentication cookies are used
    });
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
});


var app = builder.Build();
//// Call the method during app initialization
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await CreateRolesAndAdminUser(services);
//}
// Configure the HTTP request pipeline.
app.UseCors("AllowAllOrigins");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    // Define roles
    string[] roleNames = { "Admin", "User", "Manager" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var role = new Role
            {
                Name = roleName,
                NormalizedName = $"{roleName} role"
            };
            await roleManager.CreateAsync(role);
        }
    }

    // Create a default Admin user
    var adminUser = new User
    {
        UserName = "capriconnaeem@gmail.com",
        Email = "capriconnaeem@gmail.com",
        FirstName = "Admin",
        LastName = "User",
        EmailConfirmed = true,
        ProfilePicture = "https://platform-lookaside.fbsbx.com/platform/profilepic/?asid=8849245255099109&height=50&width=50&ext=1733192347&hash=AbbCY1hzP2qXhbUn1ERmHc9O"
    };

    string adminPassword = "Admin@1234";
    var user = await userManager.FindByEmailAsync(adminUser.Email);

    if (user == null)
    {
        var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
        if (createAdminUser.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
