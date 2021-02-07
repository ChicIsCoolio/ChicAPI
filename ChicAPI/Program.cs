using EpicGames.NET;
using EpicGames.NET.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ChicAPI
{
    public class Program
    {
        public static string Root = "/home/runner/ChicAPI/";

        public static EpicServices Epic;

        public static void Main(string[] args)
        {
            Console.WriteLine(OAuthService.LoginURL);
            Epic = new EpicServices(GetSid(), OAuthService.AuthTokenType.LAUNCHER);

            Console.WriteLine(Epic.Account.DisplayName);

            Epic.KillSession();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:8080");
                });
    
        private static string GetSid()
        {
            using (var stream = File.CreateText($"{Root}input"))
            {
                stream.WriteLine("sid=[INSERT SID HERE]");
            }

            Console.WriteLine("Insert sid and press enter");
            Console.ReadKey();

            string sid = File.ReadAllText($"{Root}input").Split('=')[1];
            File.Delete($"{Root}input");

            return sid;
        }
    }
}