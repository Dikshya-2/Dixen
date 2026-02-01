using Dixen.Repo.Model;
using Dixen.Repo.Model.Entities;
using Dixen.Repo.Repositories;
using Dixen.Repo.Repositories.Interfaces;
using Dixen.Repo.Services;
using Dixen.Repo.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped(typeof(IGRepo<>), typeof(GRepo<>));
builder.Services.AddTransient<IEmailSender, EmailService>();// New instance every time
builder.Services.AddScoped<IJWTService,JWTService>();
builder.Services.AddScoped<TwoFAService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IEventService, EventService>();
//builder.Services.AddScoped<IEventAttendanceService, EventAttendanceService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<EventAnalysisService>();
builder.Services.AddScoped<IGRepo<SocialShare>, GRepo<SocialShare>>();// one instance per request





//core to connect with frontent
builder.Services.AddCors(options =>
{
    options.AddPolicy("Test",
                          policy =>
                          {
                              //policy.WithOrigins("http://localhost:4200","https://localhost:4200,  ")  // I can replace with my frontend URL
                              policy.AllowAnyOrigin()
                                     .AllowAnyHeader()
                                     .AllowAnyMethod();
                          });
});
//connecting string
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(new SoftDeleteInterceptor());
});

//builder.Services.AddDbContext<DatabaseContext>(obj => obj.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler =
//            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Dixen API",
        Version = "v1"
    });

    // JWT Authentication setup in Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.ProviderMap.Add("Email", new TokenProviderDescriptor(typeof(EmailTokenProvider<ApplicationUser>)));
})
.AddEntityFrameworkStores<DatabaseContext>() // replace with your actual DbContext
.AddDefaultTokenProviders();

// Add authentication if using JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// AddJwtBearer: rules that say how to validate tokens using that data.
.AddJwtBearer(options =>
{
    //builder.Configuration.GetSection("Jwt") expects you to have an appsettings.json entry:
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Test");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
