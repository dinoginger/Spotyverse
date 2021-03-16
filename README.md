# SpotifyBot_
Bot, which gives useful requested data by using Spotify API.
## Instalation
List of main packages
```bash
Discord.Net 
Discord.Addons.Preconditions
SpotifyAPI.Web 
Serilog
```
Also you need to create your Spotify application, in order to get tokens for Spotify service to work.
It can be done [here](https://developer.spotify.com/dashboard/applications).

## Usage
Prefix for commands is _"<<"_.
Here is the  small list of stuff it can do : 
Command (and syntax)| What it does
------------ | -------------
<<help | Sends embed with list of commanmds
<<invite | Sends you bot invite link in DMs
<<search (string search_request) | Returns pretty embed, for artist/album/track info request.
<<listen (int n) | Invokes async command, which lasts for (int n) minutes, and checks current users Spotify discord activity, returning you simple analyzis in embed (NOTE : Your spotify must be connected to discord)
<<listen (int n, SocketUser user) | Overload, checks activity for custom @user.
