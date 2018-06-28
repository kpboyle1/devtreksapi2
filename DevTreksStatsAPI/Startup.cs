using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevTreks.DevTreksStatsApi
{
    /// <summary>
    ///Purpose:		Configure the web app and start MVC webapi page
    ///             delivery.
    ///Author:		www.devtreks.org
    ///Date:		2018, June
    ///References:	Containers reference in the source code tutorial
    /// </summary>
    public class Startup
    {
        private static string DefaultRootFullFilePath { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //216: temp removed for config but get working
            //set the webroot full file path: C:\\DevTreks\\src\\DevTreks\\wwwroot
            DefaultRootFullFilePath = Configuration["PATH_BASE"];
            //DefaultRootFullFilePath = string.Concat(env.WebRootPath, "\\");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            bool bIsDevelopment = true;
            // Add framework services.
            services.AddMvc();
            //di the repository into the controller
            //this sets the 2 paths to script executables (RScript.exe and pythonw.exe) and the isdevelopment param
            services.AddSingleton<Models.IStatScriptRepository>(new Models.StatScriptRepository("statscript"
                , string.Empty
                , string.Empty
                , string.Empty
                , Configuration["Site:PyExecutable"]
                , Configuration["Site:RExecutable"]
                , Configuration["Site:JuliaExecutable"]
                , DefaultRootFullFilePath
                , Configuration["DebugPaths:DefaultRootWebStoragePath"]
                , Configuration["DebugPaths:DefaultWebDomain"]
                ////tests of localhost:5000 deployed app
                //, Configuration["ReleasePaths:DefaultRootWebStoragePath"]
                //, Configuration["ReleasePaths:DefaultWebDomain"]
                , bIsDevelopment));
        }

        public void ConfigureProductionServices(IServiceCollection services)
        {
            bool bIsDevelopment = false;
            // Add framework services.
            services.AddMvc();
            //di the repository into the controller
            //this sets the 2 paths to script executables (RScript.exe and pythonw.exe)
            services.AddSingleton<Models.IStatScriptRepository>(new Models.StatScriptRepository("statscript"
                , string.Empty
                , string.Empty
                , string.Empty
                , Configuration["Site:PyExecutable"]
                , Configuration["Site:RExecutable"]
                , Configuration["Site:JuliaExecutable"]
                , DefaultRootFullFilePath
                , Configuration["ReleasePaths:DefaultRootWebStoragePath"]
                , Configuration["ReleasePaths:DefaultWebDomain"]
                , bIsDevelopment));
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
