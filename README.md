# Language Understanding in Mixed Reality with LUIS

## About 
The aim of the project is to provide a sample Unity project demonstating how to leverage LUIS Cognitive Services as another method of input with MR applications. While it is possible to add voice commands and dication as input as part of the MR Toolkit, there is no language understanding as part of that input out of the box. By adding support for LUIS in Unity you can use natural language to control Unity game objects. This project was initially developed during an internal Microsoft Mixed Reality hack.

## Jump To: 
- [Minimum requirements](#minimum-requirements)
- [Running the demo scene](#running-the-demo-scene)
- [Adding these components to your project](#setting-up-components)
- [What's next](#next-steps) 

## Minimum Requirements
- [Unity MRTP5](http://beta.unity3d.com/download/a07ad30bae31/download.html)
- Enable `.NET 4.6 (experimental)` under player settings
- Enable `.NET backend` under player settings

## Running the Demo Scene ##
1. Set up a [LUIS account](https://www.luis.ai/home). Under 'My Apps', select 'Import App' and choose the LuisApp.json file from this repository. Make sure to train the model and publish it to get the App ID and App Key (which you'll need later).  
2. In Unity, open the LUIS folder in this repository as your project. Click on the LUIS-Sample scene in Assets to open the scene. 
3. Click on `LUISDictationManager` in the Hierarchy and enter your [App ID and App Key](https://docs.microsoft.com/en-us/azure/cognitive-services/luis/manage-keys) for the LUIS model you set up in the first step. 
4. Go to the File menu -> Build Settings -> Add Open Scenes. Change the platform to Universal Windows Platform and click 'Switch Platform'. Make sure the build type is D3D and Unity C# Projects is checked. 
5. Click Build, create a separate folder called 'App' and select that folder. 
6. After it Builds, go into your 'App' folder and open the .sln file in Visual Studio. In Visual Studio on the second top toolbar, change the drop down menus from 'Debug' to 'Release', 'ARM' to 'x64', and deploy to 'Local Machine'. You'll want to deploy without debugging, which you can do in Debug -> Start Without Debugging. 
7. Make sure your headset is plugged in and you have at least one motion controller. Click on the Record Button, allow the app to use your microphone, and say a command into your mic. 
Commands include: 
- change the box/ball to \*color\*
- make the box/ball big/medium/small 


## Setting Up Components ##
Here's what you need to do in order to use these tools in your project. 
### UNITY ###
- In Assets -> LUISAssets-> Prefabs, add the `LUISDictationManager` prefab to your scene. After you complete the LUIS steps below, make sure to add your App ID and App Key to `LUISDictationManager`. 
- Make sure you have some way to start/stop dictation. In the sample application, we used a "Record Button" that called `StartRecordingDictation()` and `StopRecordingDictation()` from the `DictationToLUIS` script. 
### LUIS ###
> Note: When training the LUIS model for Unity we reserved one set of entities for object identification and selection. Then we added additional Entity sets for altering the game object's properties (such as color or size).

In order to implement in Unity:  
- Attach the `LuisBehavior` script onto the Unity GameObject you wish to control.
- Enter the Entity type and Entity name used for object identification purposes. eg. The Entity type might be "Shape" and the Entity name id "box".


## Next Steps ##
These are potential ideas for improving this project.  
- Implement raycasting to determine what the player is looking in addition to explicitly stating which GameObject to affect. 
- Create an easy to use UI for `LUISBehavior` so you don't have to hook everything up on the script.  
- Auto import LUIS Intents and Entity catagories so users don't have to explicitly use strings on the `LUISBehavior` script. 
- Investigate use of ScriptableoOjects to hold LUIS behaviors. 
