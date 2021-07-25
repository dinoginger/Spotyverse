using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Genius;

using Microsoft.VisualBasic;

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
     /// <returns>song lyrics in a string</returns>
        public static string Get(string request) 
            
        {
            try
            {
                GeniusClient geniusClient = new GeniusClient("uXdLEYWbhLVh8vVTykbuqDVGpJ_18rcKbszqWUxZTGk4CSjK_bZUUaZ82nFzF_DL");
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
                
                return result;
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