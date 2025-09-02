# QuantumLauncher
Third party launcher for the game Quantum League which will inject a helper dll enabling console and executing host / connect commands.

# **Installation**
Download the zip file, extract it to whatever folder you want
Run quantumJumper.exe
Find your instance of ```TimeWatch-Win64-Shipping.exe``` if it doesn't automatically find where game is. - usually C:\Program Files (x86)\Steam\steamapps\common\TimeWatch\TimeWatch\Binaries\Win64\WimeWatch-Win64-Shipping.exe
choose if host or connecting
click launch

NOTE: Host needs to forward their port.

NOTE: Client launches game first, once in main menu, then the host launches. (just weird unreal engine things)

# **How it Works**


Open the launcher, select where your QuantumLeague exe is, it's actually called ```TimeWatch-Win64-Shipping.exe``` and can usually be found at ```C:\Program Files (x86)\Steam\steamapps\common\TimeWatch\TimeWatch\Binaries\Win64``` or wherever you may have your games installed.
Next choose if you're host or client, host chooses map whereas client selects game from list - click refresh.


HOST NEEDS TO FORWARD THEIR PORT

**Note for now the CLIENT must attempt connection to the host BEFORE the host is hosting. This can be the client launching first, sitting in menu and then having the host launch (just as precaution) or use console commands appropriately**


# **Known Issues**


- The game IS buggy, it relies on the host to process the game and sometimes it does have issues

- When a match finishes, the hosting user's game will crash*
  - Probably can listen for final point scored
- Not all maps are available at this time**
  - Unsure why they won't load.
- Not all Game modes are available at this time**
  - Haven't tested domination
  - Tutorial/solo play is limited to TutorialArena
- 2v2 is unavailable at this time*
  -  Likely can be in-game flags for player count
- When client uses minimap game crashes
    -  Not sure of cause, when using local clients the host cannot open minimap but game runs, and client can use minimap
- Player character model is the host's**
  -  Need to figure out client server communication
- user names in-game are numbers (usually around 257)**
  -  Need to figure out client server communication
- No Cosmetics
  -  Need to figure out client server communication 

* indicates there's a likely solution
** indicates it may be a while

- ~~When the game launches there's an error with EAC* (will not be able to enable EAC but should be able to clear error)~~


# **Maps**




**IN**


CargoShip

ContainerYard

MuseumArena

NordicArena

Overpass

Overpass_Domination

QuantumArena

QuantumArenaNight

QuantumStadium

QuantumStadiumNight

TutorialArena (SP)

MainMenu (the main menu, but allows host to be first and then use serverTravel MapName)



**NOT IN: **(Are map files associated with the game, but do not load from console command ```open map```)


Chronos

FPSTutorial

MuseumArenaDay

QuantumArena_AltLayout

ShootingRange

TutorialDM




# **Goals**


**Next Goal**
- Solve double bullet for client


**Later Goals**
- Fix character selection
- Find out about 2v2 potential
- Automatically detect end of game and server travel before game crash

**Future Goals**
- create proxy server for game to connect to
-     Re-enable main-menu if not locked trying to connect?
-     Allow in-game mm?
- Skin selection
- Figuring out the non-functional maps
- Changing server settings
- in-game overlay
- anti-cheat???




# **Thanks**




Shoutout to Gwog on discord for being my inspiration for this project after creating a side-loading hack to get console up and running with p2p connection
Shoutout to the discord server for so much encouragement / still being alive after all this time https://discord.gg/Ttd7Y7XW8N
Thank you to Nimble Giant for the amazing game, and leaving it unsecured enough to make a novice with determination able to get to where we are now.

I used Claude AI to assist with the boilerplate and learning key concepts I needed to make this project possible.
