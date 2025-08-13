# QuantumLauncher
Third party launcher for the game Quantum League which will inject a helper dll enabling console and executing host / connect commands.

# **Installation**
Download the zip file, extract it to whatever folder you want
Run quantumJumper.exe
Find your instance of ```TimeWatch-Win64-Shipping.exe``` if it doesn't automatically find where game is.
choose if host or connecting
click launch

NOTE: Host needs to forward their port.

NOTE: Client launches game first, once in main menu, then the host launches. (just weird unreal engine things)

# **How it Works**


Open the launcher, select where your QuantumLeague exe is, it's actually called ```TimeWatch-Win64-Shipping.exe``` and can usually be found at ```C:\Program Files (x86)\Steam\steamapps\common\TimeWatch\TimeWatch\Binaries\Win64``` or wherever you may have your games installed.
Next choose if you're host or client, host chooses map whereas client enters their target IP address.


HOST NEEDS TO FORWARD THEIR PORT

**Note for now the CLIENT must attempt connection to the host BEFORE the host is hosting. This can be the client launching first, sitting in menu and then having the host launch (just as precaution) or use console commands appropriately**




# **Console**




To Host:
```open MapName?Listen```
To bring other user with you to other map
```serverTravel MapName```
To Connect:
```open IP:Port```
To Change level locally
```travel MapName```

Command to be explored:
```open MapName?server```


# **Known Issue**


- The game IS buggy, it relies on the host to process the game and sometimes it does have issues

- When a match finishes, the hosting user's game will crash*
- Not all maps are available at this time**
- Not all Game modes are available at this time**
- 2v2 is unavailable at this time**
- When the game launches there's an error with EAC* (will not be able to enable EAC but should be able to clear error)
- Player character model is the host's*
- user names in-game are numbers (usually around 257)*
- No Cosmetics**

* indicates there's a likely solution
** indicates it may be a while




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




**Near Goals**
- Have host sit in mainmenu while waiting for client to connect, then automatically switch maps - enabling the host to launch / host first.
- Make UI better

**Later Goals**
- Fix character selection
- Find out about 2v2 potential
- Find if ```?server``` flag can actually allow two other users to play (would be HUGE)
- Add host/server list to launcher
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

Claude AI for accelerating my final stretch and providing excellent support for an MVP.
Big thanks to Encryqed for the SDK dumping tools making this task much easier than it'd be otherwise https://github.com/Encryqed/Dumper-7
