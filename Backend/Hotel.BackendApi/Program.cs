using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hotel.BackendApi.Helpers;
using Hotel.BackendApi.Middlewares;
using Hotel.Data;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Vinh Hotel API", Version = "v1" });
    }
);

builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IFileServices, FileServices>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IMenuServices,MenuService>();
builder.Services.AddScoped<IPagingService, PagingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddHostedService<ScheduleService>();


var maisetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(maisetting);
builder.Services.AddScoped<IMailService, MailService>();

builder.Services.AddDbContext<HotelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? throw new InvalidOperationException("Allowed origins invalid"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
//entity 
// đặt trước addAuth
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
   
})
.AddEntityFrameworkStores<HotelContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;
});

// authen
builder.Services.AddAuthentication(options => {

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:AccessTokenSecret"]!))
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {        
            context.HandleResponse();
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(
                new ApiResponse
                {
                    StatusCode = 401,
                    IsSuccess = false,
                    Message = "Bạn chưa được xác thực, làm ơn đăng nhập để lấy quyền."
                }
            );
        }
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();
// Configure the HTTP request pipeline.
 app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        //c.RoutePrefix = string.Empty;  // Swagger UI sẽ xuất hiện tại gốc URL
    });
app.UseMiddleware<VerifyRevokedToken>();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
