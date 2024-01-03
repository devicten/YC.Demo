using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using YC.Demo1.Configs;
using YC.Demo1.Helpers;
using YC.Demo1.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLog4Net("Configs/log4net.Config");
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<DBHelper>();
// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Jwt configuration starts here
builder.Services.AddSingleton<JwtHelpers>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();

var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();
var jwtSignKey = builder.Configuration.GetSection("Jwt:SignKey").Get<string>();
//builder.Services.AddAuthenticationCore();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("BearToken"))
                    context.Token = context.Request.Cookies["BearToken"];
                return Task.CompletedTask;
            }
        };
        // �����ҥ��ѮɡA�^�����Y�|�]�t WWW-Authenticate ���Y�A�o�̷|��ܥ��Ѫ��Բӿ��~��]
        options.IncludeErrorDetails = true; // �w�]�Ȭ� true�A���ɷ|�S�O����

        options.TokenValidationParameters = new TokenValidationParameters
        {// �z�L�o���ŧi�A�N�i�H�q "sub" ���Ȩó]�w�� User.Identity.Name
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
            // �z�L�o���ŧi�A�N�i�H�q "roles" ���ȡA�åi�� [Authorize] �P�_����
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

            // �@��ڭ̳��|���� Issuer
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            // �q�`���ӻݭn���� Audience
            ValidateAudience = false,
            //ValidAudience = "JwtAuthDemo", // �����ҴN���ݭn��g

            // �@��ڭ̳��|���� Token �����Ĵ���
            ValidateLifetime = true,

            // �p�G Token ���]�t key �~�ݭn���ҡA�@�볣�u��ñ���Ӥw
            ValidateIssuerSigningKey = false,

            // "1234567890123456" ���ӱq IConfiguration ���o
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSignKey))
        };

    })
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.Cookie.Name = "user-session";
        options.SlidingExpiration = true;
    }); ;

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var fileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets"));
var requestPath = "/assets";
// Enable displaying browser links.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();



app.Run();
