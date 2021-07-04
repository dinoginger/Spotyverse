using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.VisualBasic;

namespace SpotifyBot.Service
{
    public class LyricsScrapper
    {
        public static string Get(string request)
        {
            try
            {
                string result = null;
                // Forming url for lyrics
                string query = request.Replace(" ", "-");
                string url = $"https://genius.com/{query}-lyrics";

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
    }
}