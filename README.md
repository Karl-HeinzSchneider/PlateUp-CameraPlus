# PlateUp-CameraPlus

![](screenshots/CameraPlus.png)

Mod for the PC Game 'PlateUp!'

View on the Steam Workshop: https://steamcommunity.com/sharedfiles/filedetails/?id=3235121178

## CameraPlus

CameraPlus enhances the default camera to improve vision on bigger maps (KitchenDesigner etc).
You can choose between two modes:

## 1) Static Camera

Set a static camera to the current player position. It stays centered on that spot even if you move out of view.

## 2) Follow Mode

Set the camera to always follow your character.
Toggle follow mode with F, or hold the Set Static Camera Position control to follow temporarily. Release it to leave the camera centered on your last position.

In both modes you can adjust the camera distance/zoom with scroll.

## Key Bindings - Keyboard

- Toggle CameraPlus = **Q**
- Set Static Camera Position = **E**
- Temporary Follow Mode = **hold E**
- Toggle Follow Mode = **F**
- Adjust Zoom = **Mouse Wheel**

## Key Bindings - Gamepad

- Toggle CameraPlus = **Right Stick Press**
- Set Static Camera Position = **'RB' / Right Shoulder**
- Temporary Follow Mode = hold **'RB' / Right Shoulder**
- Toggle Follow Mode = **F** (keyboard only)
- Adjust Zoom = Right Stick Y

## Build and test locally

Requires the .NET Framework 4.7.2 targeting pack and the installed PlateUp! game.
Build against the game assemblies in your Steam library with PowerShell:

```powershell
dotnet build -c Release "-p:PlateUpManagedDir=C:\path\to\PlateUp\PlateUp_Data\Managed"
```

The build produces `bin\Release\CameraPlus.dll`. Copy that file to
`Mods\CameraPlus\content\CameraPlus.dll` beside `PlateUp.exe`, then restart the
game. Use a single copy of CameraPlus during testing. The checked-in
`content\CameraPlus.dll` is the Workshop package binary.

The release build was checked against PlateUp! 1.5.3-854F (Steam build 26000811).
To test in game, enter a kitchen with a local player and check the following:

- Toggle CameraPlus, walk away, and confirm the camera remains fixed.
- Tap E or RB to recenter. Hold it while moving and confirm the view stays
  centered on the last spot after release.
- Toggle continuous follow with F and check zoom with the mouse wheel and
  right stick in both modes.
- Toggle CameraPlus off and confirm the game resumes its normal camera.
  Return to headquarters with CameraPlus active and check the next kitchen.
