using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using WebApi.Middleware;
using System;
using System.Text;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace ApiService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var cultureInfo = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            /*services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "telcoInsuranceClaim", Version = "v1" });
            });*/


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = "telco-jicp.com",
                    ValidIssuer = "mercurialminds.com",
                    RequireExpirationTime = false,
                    ClockSkew = TimeSpan.FromMinutes(10),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SECURE_KEY"]))
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            /*app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "telcoInsuranceClaim v1"));*/


            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else{
            app.UseExceptionHandler("/Home");
            // }

            // app.UseExceptionHandler(c => c.Run(async context => {
            //     var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
            //     var result = JsonConvert.SerializeObject(new { error = exception.Message });
            //     context.Response.ContentType = "application/json";
            //     await context.Response.WriteAsync(result);
            // }));

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home";
                    await next();
                }
            });

            // app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAllHeaders");

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Public")),
                RequestPath = new PathString("/content")
            });

            // app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization(); 

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
