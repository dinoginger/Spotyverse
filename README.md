## Status
Offline, waiting for verification
# Spotyverse
Analyses your music taste and provides information about songs and artists.

## Usage
Prefix for commands is _"<<"_.
Here is the  short list of stuff it can do : 
Command (and syntax)| What it does
------------ | -------------
<<help | Prints list of supported commands
<<invite | Sends your bot invite link in DMs
<<search (string search_request) | Returns formatted embed, for artist/album/track info request.
<<listen (int n) | Invokes async command, which lasts for (int n) minutes and checks current users Spotify discord activity. Returns music taste analyzis in embed (NOTE : Your Spotify account must be connected to Discord)
<<listen (int n, String user) | Checks the activity for a user (String user).

## Set up
1. You need to create your Spotify application
2. Get tokens for Spotify service from [here](https://developer.spotify.com/dashboard/applications).
3. Get token from Discord
4. Paste the tokens in config file.

## License
This project is licensed under the terms of the [MIT](https://opensource.org/licenses/MIT) license.


