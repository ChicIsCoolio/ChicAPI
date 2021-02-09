using EpicGames.NET;
using EpicGames.NET.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using ChicAPI.Chic;
using EpicGames.NET.Models;
using System.Reactive.Concurrency;
using Fortnite_API;

namespace ChicAPI
{
    public class Program
    {
        public static string Root = "/home/runner/ChicAPI/";

        public static EpicServices Epic;
        public static FortniteApi FortniteApi;

        public static void Main(string[] args)
        {
            FortniteApi = new FortniteApi(Environment.GetEnvironmentVariable("APIKEY"));

            try
            {
                Console.WriteLine("1");

                if (Database.TryGetValue("AccessToken", out string token) &&
                    Database.TryGetValue("RefreshToken", out string refreshToken) &&
                    Database.TryGetValue("ExpiresAt", out string expiresAt))
                {
                    try
                    {
                        Epic = new EpicServices(token, refreshToken, DateTime.Parse(expiresAt));
                        Console.WriteLine("5A");
                    }
                    catch (EpicGamesException e)
                    {
                        Console.WriteLine("5B");
                        Console.WriteLine(e.ErrorCode);
                        Console.WriteLine(e.ErrorMessage);
                        Epic = new EpicServices(GetSid(), OAuthService.AuthTokenType.LAUNCHER);
                    }
                }
                else Epic = new EpicServices(GetSid(), OAuthService.AuthTokenType.LAUNCHER);
            }
            catch (EpicGamesException e)
            {
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.ErrorMessage);
                Epic = new EpicServices(GetSid(), OAuthService.AuthTokenType.LAUNCHER);
            }

            Console.WriteLine(Epic == null);

            Database.SetValue("AccessToken", Epic.AccessToken);
            Database.SetValue("RefreshToken", Epic.RefreshToken);
            Database.SetValue("ExpiresAt", Epic.ExpiresAt.ToString("o"));

            Console.WriteLine($"Authed as {Epic.Account.DisplayName}");

            Scheduler.Default.Schedule(Epic.ExpiresAt.AddSeconds(-10), reschedule =>
            {
                Epic.RefreshSession();
                reschedule(Epic.ExpiresAt.AddSeconds(-10));
            });

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
            Console.WriteLine(OAuthService.LoginURL);

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

        public static string[] ListCache() => Directory.GetFiles($"{Root}Cache");

        public static string LoadFromCache(string fileName)
        {
            using (StreamReader reader = new StreamReader($"{Root}Cache/{fileName}"))
            {
                return reader.ReadToEnd();
            }
        }

        public static void SaveToCache(string data, string fileName)
        {
            using (StreamWriter writer = new StreamWriter($"{Root}Cache/{fileName}"))
            {
                writer.Write(data);
            }
        }
    }
}