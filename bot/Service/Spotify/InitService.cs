using System.IO;

namespace SpotifyBot.Service.Spotify
{
    public partial class SpotifyService
    {
        private static string Bot_id;  //Id of my spotify app
        private static string Bot_ids; //secret Id 

        private static readonly string configPath = @".\_config.json";
        private static readonly string t_configPath = @"/Users/marco/Desktop/Spotyverse/bot/t_config.json";
        //^ this is test config file

        private static void GetSpotifyTokens() //Method to get spotify tokens
        {
            StreamReader r = new StreamReader(configPath);
            string json = r.ReadToEnd();
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            Bot_id = data.Spotify_id;
            Bot_ids = data.Spotify_ids;

        }
    }
}