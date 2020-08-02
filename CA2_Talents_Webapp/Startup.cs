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

            services.AddAWSService<IAmazonDynamoDB>();

            services.AddSingleton<ICreateTable, CreateTable>();
            services.AddSingleton<IAddUser, AddUser>();
            services.AddSingleton<IGetUser, GetUser>();
            services.AddSingleton<IUpdateUser, UpdateUser>();

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Jing Hui's dynamoDB
            //app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "standard",
                    template: "{controller=Home}/{action=StandardUser}/{name}"
                    );

                routes.MapRoute(
                    name: "premium",
                    template: "{controller=Home}/{action=Main}/{name}/{plan}"
                    );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");               
            });
        }
    }
}
