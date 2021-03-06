﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using System.IO;
using DynamoDb.libs.DynamoDb;
using CA2_Talents_Webapp.SQLDatabase;
using Microsoft.EntityFrameworkCore;
using CA2_Talents_Webapp.Services;
using Amazon.S3;

namespace CA2_Talents_Webapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Jing Hui's SQL Database - AWS RDS
            // Ref --> https://www.syncfusion.com/blogs/post/build-crud-application-with-asp-net-core-entity-framework-visual-studio-2019.aspx#comments
            var connection = Configuration.GetConnectionString("TalentDatabase");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
            //services.AddControllersWithViews();


            // Example From TMS
            //services.AddCors();//https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.2
            //services.AddDbContext<ApplicationDbContext>(
            //    options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            //);


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Jing Hui's dynamoDB
            //services.AddMvc();
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", Configuration["AWS:AccessKey"]);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", Configuration["AWS:SecretKey"]);
            Environment.SetEnvironmentVariable("AWS_REGION", Configuration["AWS:Region"]);

            string credential_path = "C:\\inetpub\\AspNetCoreWebApps\\app\\GoogleVisionAI.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);

            services.AddAWSService<IAmazonDynamoDB>();

            services.AddSingleton<ICreateTable, CreateTable>();
            services.AddSingleton<IAddUser, AddUser>();
            services.AddSingleton<IGetUser, GetUser>();
            services.AddSingleton<IUpdateUser, UpdateUser>();

            //Amazon S3
            services.AddSingleton<IS3Service, S3Service>();
            services.AddAWSService<IAmazonS3>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // JingHui's SQL Database
            //app.UseDataEngineProviders().AddDataEngine("talentdb-2", @"Data Source=;Provider=;Initial Catalog=", "talentdb");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Jing Hui's dynamoDB
            //app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "premium",
                    template: "{controller=Home}/{action=Main}/{name}/{plan}/{stripeId}"
                    );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");               
            });
        }
    }
}
