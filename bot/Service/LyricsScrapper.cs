using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Genius;

using Microsoft.VisualBasic;
using SpotifyBot.Service.Spotify;

namespace SpotifyBot.Service
{
    //TODO : Work on error handling (especially line 34 here).
    
    //TODO : New type of error :  Message content is too long, length must be less or equal to 2000. (Parameter 'Content'). Fix it as well

    //
    public class LyricsScrapper
    {/// <summary>
     /// Gets url from genius and later scraps lyrics div class from downloaded uri's html
     /// </summary>
     /// <param name="request"></param>
     /// <returns>Item1 : formatted lyrics
     /// Item2 : thumbnail lyrics</returns>
        public static Tuple<string, string> Get(string request) 
            
        {
            try
            {
                GeniusClient geniusClient = new GeniusClient(SpotifyService.genius_app_key);
                string result = null;
                // Getting url for lyrics
                request = CleanRequest(request);
                Console.WriteLine("final request --- " + request);
                var return_data = geniusClient.SearchClient.Search(request);
                string url = return_data.Result.Response.Hits[0].Result.Url;

                // declaring & loading doc
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc = web.Load(url);

                result = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'lyrics')]").InnerText;
                
                string formatted = Regex.Replace(result, @"[\n  ]{2,}", "\n");
                Console.WriteLine(formatted);
                Console.WriteLine(formatted.Length);
                return new Tuple<string, string>(formatted, return_data.Result.Response.Hits[0].Result.HeaderImageUrl);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
     /// <summary>
     /// This is supposed to clean request from stuff which can cause search failure.
     /// Examples : Remastered (2011), Live at Boston, (Bad Computer Mix)
     /// </summary>
     /// <param name="request"></param>

        private static string CleanRequest(string request)
     {
         string[] words = new[] {"REMASTERED", "REMIX", "LIVE AT", "MIX", "COVER", };
         request = request.ToUpper();
         foreach (var word in words)
         {
             if (request.Contains(word))
             {
                 request = request.TrimEnd(word.ToCharArray());
                 Console.WriteLine(request);
                 return request;
             }
         }

         return request;
     }
    }
}