
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApiCRUD.DataAccess.Data;
using WebApiCRUD.DataAccess.Implementation;
using WebApiCRUD.Models;
using WebApiCRUD.Repositories;

namespace WebApiCRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(op =>{
                    // op.SuppressModelStateInvalidFilter=true; // عشان يوقف (الموديل ستيت) عشان اعرف اهندل انا الاخطاء واغير رساله الايرور
                });                                                                                            // ModelState
           //----------------------------------------------------------------------------------------------
            builder.Services.AddDbContext<AppDbContext>(op =>
            {
                op.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });

            //----------------------------------------------------------------------------------------------
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(op =>
            {
                op.Password.RequireLowercase = false;
                op.Password.RequireUppercase = false;
                op.Password.RequiredLength = 6;
                op.Password.RequireNonAlphanumeric = false;
                op.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>();
            //----------------------------------------------------------------------------------------------
            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.RequireHttpsMetadata = true; // Sure that Request HTTP
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, // Sure that i was the auther *(But in microservices and distributed systems false)
                    ValidIssuer= builder.Configuration["JWT:IssuerIP"],

                    ValidateAudience = true,
                    ValidAudience= builder.Configuration["JWT:AudienceIP"],

                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecritKey"]))

                };

            });
            //----------------------------------------------------------------------------------------------
            builder.Services.AddCors(op =>
            {
                op.AddPolicy("MyPolicy", CorsPolicyBuilder =>
                {
                    CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
                op.AddPolicy("MyPolicy2", CorsPolicyBuilder =>
                {
                    // ???????????????????????
                });
            });

           //----------------------------------------------------------------------------------------------
            builder.Services.AddTransient<IUnitOfWork,UnitOfWork>();

            //----------------------------------------------------------------------------------------------
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            /*-----------------------------Swagger PArt-----------------------------*/
            #region Swagger REgion
            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 5 Web API",
                    Description = " ITI Projrcy"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    }
                    });
            });
            #endregion
            //----------------------------------------------------------

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
