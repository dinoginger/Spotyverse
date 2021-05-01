using System;
using System.Collections.Generic;

namespace SpotifyBot.Service.ForCooldown
{
    
    /// <summary>
    /// This thingy checks if command "listen" is still running for specified user.
    /// this will add every user's (who has program listen running) ID to list.
    /// </summary>
    public class ListenUsersList
    {
        /// <summary>
        /// First Key : user.id,
        /// first tuple item : Time span listen minutes, requested by user
        /// second tuple item : utcNow of command start
        /// </summary>
        public Dictionary<ulong, Tuple<TimeSpan,DateTime>> _UsrDict = new Dictionary<ulong,Tuple<TimeSpan,DateTime>>();
    }
}