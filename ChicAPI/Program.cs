#define USE_EPICGAMES

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
using ChicAPI.Controllers;
using ChicAPI.Models;
using EpicGames.NET.Models;
using System.Linq;
using System.Reactive.Concurrency;
using Fortnite_API;
using SkiaSharp;
using System.Net.Http;

namespace ChicAPI
{
    public class Program
    {
        public static string Root = "/home/runner/ChicAPI/";

        public static EpicServices Epic;
        public static FortniteApiClient FortniteApi;

        public static void Main(string[] args)
        {
            FortniteApi = new FortniteApiClient(Environment.GetEnvironmentVariable("APIKEY"));


#if USE_EPICGAMES
            try
            {
                if (Database.TryGetValue("AccessToken", out string token) &&
                    Database.TryGetValue("RefreshToken", out string refreshToken))
                {
                    try
                    {
                        Epic = new EpicServices(token, refreshToken);
                    }
                    catch (EpicGamesException e)
                    {
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
            Database.SetValue("AccessToken", Epic.AccessToken);
            Database.SetValue("RefreshToken", Epic.RefreshToken);

            Console.WriteLine($"Authed as {Epic.Account.DisplayName}");

            Scheduler.Default.Schedule(Epic.ExpiresAt.AddSeconds(-10), reschedule =>
            {
                Epic.RefreshSession();
                
                Database.SetValue("AccessToken", Epic.AccessToken);
                Database.SetValue("RefreshToken", Epic.RefreshToken);
                
                reschedule(Epic.ExpiresAt.AddSeconds(-10));
            });
#else
#warning The Epic Games auth is disabled
#endif

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

        public static bool IsInCache(string fileName) => ListCache().Any(x => Path.GetFileName(x) == fileName);

        public static string LoadFromCache(string fileName)
        {
            using (StreamReader reader = new StreamReader($"{Root}Cache/{fileName}"))
            {
                return reader.ReadToEnd();
            }
        }

        public static SKBitmap LoadBitmapFromCache(string fileName)
            => SKBitmap.Decode($"{Root}Cache/{fileName}.png");

        public static void SaveToCache(string data, string fileName)
        {
            using (StreamWriter writer = new StreamWriter($"{Root}Cache/{fileName}"))
            {
                writer.Write(data);
            }
        }

        public static void SaveToCache(SKBitmap bitmap, string fileName, bool dispose = true)
        {
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite($"{Root}Cache/{fileName}.png"))
                data.SaveTo(stream);
        }

        public static void ClearCache()
            => ListCache().ToList().ForEach(file => File.Delete(file));

        public static SKBitmap BitmapFromUrl(Uri url, string name = "eek")
        {
            if (url == null) return null;

            using (var client = new HttpClient())
            using (var stream = client.GetStreamAsync(url).Result)
            {
                var bitmap = SKBitmap.Decode(stream);

                SaveToCache(bitmap, name, false);

                return bitmap;
            }
        }

        public static bool IsAuthed()
        {
            return !(Epic == null || !Epic.Authenticated);
        }
    }
}