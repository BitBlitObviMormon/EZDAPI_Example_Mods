using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace EasyDeliveryAPI.ExampleMods
{

    [BepInDependency("EasyDeliveryAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("ExampleMod.UIExample", "Hello UI", "1.0.0")]
    public class UIExamplePlugin : BaseUnityPlugin
    {
        const string modID = "ExampleMod.UIExample";
        internal static new ManualLogSource Logger;
        private void Awake()
        {
            Logger = base.Logger;

            // Add our weather HUD to the game's HUD.
            EasyAPI.AddsHUDListener<WeatherHUD>(modID + "_WeatherHUD");

            // Easy Delivery Co mainly uses two types of UIs: WindowView (mail.exe, options.exe) and ScreenProgram (map, desktop itself)
            // Choosing which type of UI to use is mainly a matter of personal preference, I haven't found any benefits to using one over the other.
            // Our weather app is going to be a WindowView and our weather game is going to be a ScreenProgram.

            // Create a file for our weather app.
            DesktopDotExe.File weatherAppFile = EasyAPI.InstantiateFile();
            weatherAppFile.type = DesktopDotExe.FileType.exe;
            weatherAppFile.name = "Weather";

            // EasyAPI does not currently support custom icons, so we'll just use a default-looking file icon.
            weatherAppFile.icon = (int)FileIcon.File;
            weatherAppFile.iconHover = (int)FileIcon.FileSlanted;

            // Add our app to the game.
            // weatherFile.data MUST be set to "listener_" + the name of the listener that was added to EasyAPI. It won't work otherwise.
            weatherAppFile.data = "listener_" + weatherAppFile.name;
            EasyAPI.AddListener<WeatherApp>(weatherAppFile.name);
            EasyAPI.AddFile(EasyAPI.DesktopLocation.Main, weatherAppFile);

            /* I can't figure out how to prevent EasyAPI.AddProgram() from softlocking the title screen.
            // Create a file for our weather game. I tried using a component also.
            // GameObject obj = new GameObject("Weather Game");
            // WeatherGame weatherGame = obj.AddComponent<WeatherGame>();
            WeatherGame weatherGame = new WeatherGame();
            DesktopDotExe.File weatherGameFile = EasyAPI.InstantiateFile();
            weatherGameFile.type = DesktopDotExe.FileType.exe;
            weatherGameFile.name = "Weather Game";

            // EasyAPI does not currently support custom icons, so we'll just use the truck icon.
            weatherGameFile.icon = (int)FileIcon.Truck;
            weatherGameFile.iconHover = (int)FileIcon.TruckBumped;

            // Add our game to the game.
            weatherGameFile.data = "modded_" + weatherGame.title;
            EasyAPI.AddFile(EasyAPI.DesktopLocation.Main, weatherGameFile);
            EasyAPI.AddProgram(weatherGame);
            */
        }
    }
}
