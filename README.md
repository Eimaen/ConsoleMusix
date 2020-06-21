# ConsoleMusix
Console-based media player. The project is under development and is sometimes updated. This app was originally intended for personal use.

## Features
ConsoleMusix currently supports:
* Standard features of a media player (Pause, Play, Stop, Position Changing, etc.)
* Discord RPC
* Musixmatch **Unofficial** lyrics API
* Direct download from Soundcloud / Youtube
* Music waveform visualization
* Perfect console interface *(supports mouse ¯\\\_(ツ)\_/¯ but can work without it)*

## Screenshot
*On screenshot there is a bug that is already fixed. Bug: When position changed, first line of lyrics was marked as previous.*
*I just don't want to make screenshot again :P*
![Robin Hustin x Tobimorrow - Light it Up (NCS)](https://github.com/Eimaen/ConsoleMusix/blob/master/Screenshot.png)

## TODO
* Playlists!
* ~Make~ Fix Discord party join to listen together with friends
* Try to fasten console I/O
* Redesign of controls *(btw they look like shit now)*
* Save `.lyrlib` files automatically when lyrics from Musixmatch recieved
* Clean up
* Add more settings
* TV streaming support *(probably DLNA)*
* Add DirectX hooking to visualize music in-game *(or use overlay)*

## Disclaimer
Musixmatch API is unofficial and **is taken** from original Musixmatch app for PC. Musixmatch user token can be extracted from desktop app *(Maybe there is another way)*.
I published it just **for educational purposes**.

## Known bugs
* Soundcloud download works good, but not always. Some songs aren't currently downloadable. For those ones you can use YouTube *(pre-releases currently don't support YT download)*.

#### Your CPU must be fast enough to render console visualization and prevent FPS drops. 
All the libraries used in this project are open-source, this app can be also built for Linux *(probably I guess)*.  

