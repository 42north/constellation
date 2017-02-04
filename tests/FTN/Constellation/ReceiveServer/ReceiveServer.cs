using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FTN.Constellation.Test
{

    public class ReceiveServer
    {
        public static void Start()
        {
            var host = new WebHostBuilder()
            .UseUrls("http://localhost:23897")
            .UseKestrel(options =>
            {
                options.NoDelay = true;
                options.UseConnectionLogging();
            })
            .UseStartup<Startup>()
            .Build();
            host.Start();
        }
    }

    internal class Startup
    {
        public Startup(IHostingEnvironment host) {

        }
        
        public void ConfigureServices(IServiceCollection svc) 
        {
            svc.AddLogging();
            svc.AddMvc();
            Log.Logger = new LoggerConfiguration()
            .WriteTo.LiterateConsole(outputTemplate: 
                "{Timestamp:dd-MM-yyyy HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();
        }
        
        public void Configure(IApplicationBuilder app, 
            ILoggerFactory loggerFactory, IHostingEnvironment env) 
        {   
            loggerFactory.AddSerilog();
            app.UseMvc();
        }
        

    }   
}