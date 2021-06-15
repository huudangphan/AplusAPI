using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.PosmanErp.Commons;
using Apzon.PosmanErp.Services;
using ApzonIrsWeb.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PosmanErp;
using PosmanErp.Attributes;

namespace Apzon.PosmanErp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var appConfig = Configuration.GetSection("AppConfiguration");
            GlobalData.CommonDbUserName = Function.Decrypt(appConfig["CommonDbUserName"]);
            GlobalData.CommonDbPassword = Function.Decrypt(appConfig["CommonDbPassword"]);
            GlobalData.CommonDbServerType = Function.ToString(appConfig["CommonDbServerType"]);
            GlobalData.CommonDbServer = Function.Decrypt(Function.ToString(appConfig["CommonDbServer"]));
            GlobalData.CommonSchemaName = Function.Decrypt(Function.ToString(appConfig["CommonDbSchema"]));
            GlobalData.CommonDb = Function.Decrypt(appConfig["CommonDb"]);
            GlobalData.CommonDbPort = Function.Decrypt(Function.ToString(appConfig["CommonDbPort"]));
            GlobalData.DatabaseSystemType = (DatabaseSystemType)Function.ParseInt(Function.Decrypt(appConfig["DatabaseSystemType"]));
            GlobalData.OpenApiAdress = Function.Decrypt(appConfig["OpenApiAdress"]);
            GlobalData.CommonConnectionString = GlobalData.DatabaseSystemType == DatabaseSystemType.HANA ?
          string.Format(@"Server = {0};CS=""{1}""; UserID = {2}; Password = {3}", GlobalData.CommonDbServer, GlobalData.CommonDb, GlobalData.CommonDbUserName, GlobalData.CommonDbPassword)
          : GlobalData.DatabaseSystemType == DatabaseSystemType.PostgresSql 
          ? string.Format(@"Server={0};Port={1};Database={4};User id={2};Password={3};Pooling=false;MinPoolSize=5;MaxPoolSize=50;", GlobalData.CommonDbServer, GlobalData.CommonDbPort
                                                                                , GlobalData.CommonDbUserName, GlobalData.CommonDbPassword, GlobalData.CommonDb)
          : string.Format(@"Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ",
              GlobalData.CommonDbServer, GlobalData.CommonDbUserName, GlobalData.CommonDbPassword, string.IsNullOrEmpty(GlobalData.CommonDb) ? "SBO-COMMON" : GlobalData.CommonDb);
            GlobalData.RootUser = Function.Decrypt(appConfig["RootUser"]).ToLower();
            GlobalData.RootPass = Function.Decrypt(appConfig["RootPass"]);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().AddXmlSerializerFormatters();
            services.AddMvc().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            services.AddControllers()
                .AddNewtonsoftJson();
            services.AddHttpContextAccessor();
            services.AddScoped(provider =>
            {
                var accessToken = "";
                var controllerContext = provider.GetRequiredService<IHttpContextAccessor>();
                try
                {
                    var header = controllerContext?.HttpContext?.Request?.Headers;
                    if(header != null && header.ContainsKey("Authorization"))
                    {
                        var authHeader = header["Authorization"];
                        //lấy thông tin jwt token từ authorization của request
                        accessToken = authHeader.FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    Logging.Write(Logging.ERROR,
                        new StackTrace(new StackFrame(0)).ToString()
                            .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                }
                return accessToken;
            });
            services.AddCors(x => x.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                    }));
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(t=>t.FirstOrDefault());
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "A-PLUS API", Version = "v1" });
                c.SchemaFilter<SwaggerSchemaFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "A-PLUS API v1");
                });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "default",
                                 pattern: "api/v1/{controller=Home}/{action=Index}/{id?}");
            });

            GlobalData.WebRootPath = env.WebRootPath;
        }
    }
}
