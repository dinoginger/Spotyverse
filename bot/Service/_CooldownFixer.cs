using System;
using System.Collections.Generic;
using System.Linq;

namespace SpotifyBot.Service
{
    /// <summary>
    /// This class will be later injected in our service.
    /// Created in order to close issue  ---- https://github.com/mad-redhead/SpotifyBot_testbot/issues/5 --- issue context
    /// 
    /// </summary>
    public class _CooldownFixer
    {
        //User name - list<command name, bool (suceeded or not)>
        public Dictionary<string, Dictionary<string, bool>> ifFailed = new Dictionary<string, Dictionary<string, bool>>();

        public _CooldownFixer()
        {
            ifFailed.Clear();
        }
    }
}