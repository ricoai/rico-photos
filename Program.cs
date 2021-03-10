using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Identity;

namespace ricoai
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            //Host.CreateDefaultBuilder(args)
            //    .ConfigureWebHostDefaults(webBuilder =>
            //    {
            //        webBuilder.UseStartup<Startup>();
            //    });
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {

                var root = config.Build();
                // Verfiy if the key exist.  Only in production environment 
                // These key values should be set in the Azure Application Settings
                // The production keys will be then be found in the Azure Key Vault
                // In the appsetting.Development.json file contain the develpment key values
                if (root.GetChildren().Any(item => item.Key == "KeyVault"))
                {
                    // Setup configuration values from the Azure Key Vault
                    config.AddAzureKeyVault($"https://{root["KeyVault:Vault"]}.vault.azure.net/", root["KeyVault:ClientId"], root["KeyVault:ClientSecret"]);
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
