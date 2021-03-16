# SpotifyBot_test
This soon will become first deployed version of SpotifyAnalyzer bot, which main goal is doing funny thingies with music.
## Usage
Prefix for commands is _"<"_.
Here is the  small list of stuff it can do : 
Command (and syntax)| What it does
------------ | -------------
<help | Sends embed with list of commanmds
<invite | Sends you bot invite link in DMs
<search (string search_request) | Returns pretty embed, for artist/album/track info request.
<listen (int n) | Invokes async command, which lasts for (int n) minutes, and checks current users Spotify discord activity, returning you simple analyzis in embed (NOTE : Your spotify must be connected to discord)
<listen (int n, SocketUser user) | Overload, checks activity for custom @user.
