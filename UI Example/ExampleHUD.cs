using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyDeliveryAPI.ExampleMods
{
    // A HUD that displays the current weather and alarms the player of approaching snow storms.
    public class WeatherHUD : MonoBehaviour
    {
        private static FieldInfo RField = typeof(sHUD).GetField("R", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo WeatherTargetField = typeof(sDayNightCycle).GetField("weatherTarget", BindingFlags.NonPublic | BindingFlags.Instance);
        private DateTime TimeLastFrame = DateTime.Now;
        private float WarningBlinkOnSpeed = 1.0f;
        private float WarningBlinkOffSpeed = 0.5f;
        private double WarningBlinkTimer = 0;
        private int MessageX = 68;
        private int MessageY = 12;
        private const int CharWidth = 8;
        private int SpriteX = 98;
        private int SpriteY = 19;
        private int SpriteWidth = 32;
        private int SpriteHeight = 32;
        private int SpriteScaledWidth = 32;
        private int SpriteScaledHeight = 32;
        private bool FlipSpriteHorizontally = false;

        public enum WeatherType : int
        {
            Clear = 0,
            LightSnow = 1,
            Snow = 2,
            Blizzard = 3
        }

        // This function is called every frame the HUD is drawn.
        public void FrameUpdate(sHUD hud)
        {
            if (FindFirstObjectByType<CarLessMode>() != null) return; // Don't render anything if the scene is in carless mode (because it lacks weather).

            // Fetch the renderer -- the object that is responsible for drawing our HUD onto the screen.
            MiniRenderer R = RField.GetValue(hud) as MiniRenderer;

            // Get the current weather
            sWeatherSystem WeatherSystem = FindFirstObjectByType<sWeatherSystem>();
            if (WeatherSystem == null) return; // If the weather system has not initialized yet then wait patiently.
            float currentIntensity = WeatherSystem.intensity;
            WeatherType currentWeather = (WeatherType)Mathf.Min(currentIntensity * 4, 3.999f);

            // And the weather that will yet be.
            float futureIntensity = (float)WeatherTargetField.GetValue(hud.dayNightCycle);
            WeatherType futureWeather = (WeatherType)Mathf.Min(futureIntensity * 4, 3.999f);
            bool stormApproaching = futureWeather == WeatherType.Blizzard && currentWeather != WeatherType.Blizzard;

            // If there is a storm approaching then warn the player by blinking a warning message.
            // Blizzards are impossible in Mountain Town, even if the day/night cycle wants to spawn one.
            bool blizzardsArePossible = SceneManager.GetActiveScene().name != "MountainTown";
            if (stormApproaching && blizzardsArePossible)
            {
                // Progress the blink timer
                DateTime currentTime = DateTime.Now;
                WarningBlinkTimer += (currentTime - TimeLastFrame).TotalSeconds;
                TimeLastFrame = currentTime;
                WarningBlinkTimer %= WarningBlinkOnSpeed + WarningBlinkOffSpeed;
                bool renderWarning = WarningBlinkTimer < WarningBlinkOnSpeed;

                if (renderWarning)
                {
                    string message = "STORM WARNING";
                    R.put(message, R.width - (MessageX + message.Length * CharWidth), MessageY);
                }
            }
            // If no storm is approaching, just display the current weather.
            else
            {
                string message = currentWeather.ToString().Replace("LightSnow", "Light Snow").ToLower();
                R.put(message, R.width - (MessageX + message.Length * CharWidth), MessageY);
            }

            // Display the current weather using the game's own sprite sheet.
            int spriteIndex = (int)currentWeather + 1;
            Texture spriteSheet = R.spriteSheet;
            R.spr(spriteSheet, spriteIndex * SpriteWidth, 0, R.width - SpriteX, SpriteY, SpriteWidth, SpriteHeight, FlipSpriteHorizontally, SpriteScaledWidth, SpriteScaledHeight);
        }
    }
}