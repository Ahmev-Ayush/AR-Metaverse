# 📓 Development Log

## 🗓️ 28-03-2026

**What I Did:**
- Initial commit of the AR-Metaverse repository
- working with render streaming and its webserver
- created an android app to stream app screen on web-browser
- manualStartScript.cs for space key or touch on screen to establish connection to webserver
- used Button.onClick.RemoveListene(/action/) for exit button
- duplicated ARMetaverseScene => ARMetaverseScene1 : this will have stereoscopic rendering (two cameras) on canvasDisplay
- duplicated ARMetaverseScene => ARMetaverseScene2 : this will have stereoscopic rendering (two cameras) without canvasDisplay
- TestStreaming Scene : for unity render streaming tutorial [https://docs.unity3d.com/Packages/com.unity.renderstreaming@3.1/manual/tutorial.html]


## 🗓️ 29-03-2026

**What I Did:**
- Input Action to control the movement of the cube : for remotely controlling the cube from browser using Render Streaming
- taking input from browser using Input Receiver 
- using quad to display the laptop's screen in unity
- setup done : laptop screen display, screen capture using screenshare.html and unity render stream receiving the share to display over quad


## 🗓️ 01-04-2026

**What I Did:**
- Optimization started!
- removed all the updates from ARMetaverse Scene as right targeting optimization of that scene only.
- set target FPS to 30
- tried dynamic fps (not impressed!)
- reduced number of game object present in the ARMetaverse Scene.
- PlaceDisplayOnWallScript.cs script will turn off right after the placement of the quad (display screen)
- reducing resolution of screen to reducing heating issue of the android device!
- DPI changed to FIXED DPI = 320 (for testing!)


## 🗓️ 01-04-2026

**What I Did:**
- ARResolutionControllerscript : to reduce the resolution of the camera feed to reduce cpu usage little (testing!)
- optimizated enough to work for 5 mins (in between temp changes from 37 to 40 deg celius, on Motorola Edge 60 stylus)


## 🗓️ 08-04-2026

**What I Did:**
- fixed some issue in connection of web-browser and unity android app (ARMetaverse app)
- added realtime connection status display box in screenshare.html (public1)
- changed name of ARMetaverseScene1 and ARMetaverseScene1 to SplitScreenScene1 and SplitScreenScene2
- Created new scene ARMetaverseSceneModified : AR Session Tracking mode = Rotation Only & No plane detection! (highly optimizated for less cpu usage)

## 🗓️ 09-04-2026

**What I Did:**
- 

<!-- Keep adding entries as you work -->