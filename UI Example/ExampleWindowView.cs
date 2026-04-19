using UnityEngine;
using System.Reflection;

namespace EasyDeliveryAPI.ExampleMods
{
    // A WindowView program that draws the weather forecast onto the screen.
    public class WeatherApp : MonoBehaviour
    {
        private static FieldInfo WeatherTargetField = typeof(sDayNightCycle).GetField("weatherTarget", BindingFlags.NonPublic | BindingFlags.Instance);
        private UIUtil Util = new UIUtil();
        private Rect P;
        private Vector2 DesiredWindowSize = new Vector2(13f, 9f);
        private float ForecastTimeIntervalHours = 1f;
        private int HeaderOffsetY = 2;
        private int SpriteSize = 32;
        private string CurrentLabel = "Now";
        private int CurrentLabelX = 10;
        private int CurrentSpriteX = 10;
        private int CurrentTemperatureLabelX = 10;
        private string FutureLabel = "Next";
        private int XOffset = 50;
        private int LabelY = 16;
        private int SpriteY = 24;
        private int TemperatureLabelY = 56;
        private bool FlipSpriteHorizontally = false;
        private int TopBarThickness = 8;
        private int TopBarYOffset = -6;
        private bool BeingDragged = false;
        private Vector2 DragOffset;

        // This is called each frame the app is drawn
        public void FrameUpdate(DesktopDotExe.WindowView window)
        {
            // Initialize the UIUtil helper class
            Util.M = window.M;
            Util.R = window.R;
            Util.nav = window.M.nav;

            // Resize the window to be smaller than the default size and make it draggable.
            window.size = DesiredWindowSize;
            DoWindowDrag(window);

            // Create the bounds of our UI based on the size of the window.
            P = new Rect(window.position * 8f, window.size * 8f);
            P.position += new Vector2(8f, 8f);

            // Draw the weather forecast onto the screen.
            DrawWeatherForecast(window);
        }

        // Replicates sDayNightCycle.Temperature() but manipulates the time to predict the temperature in the future.
        private float PredictTemperature(sDayNightCycle dayNightCycle, float hoursInFuture, float intensity)
        {
            return Mathf.Lerp(dayNightCycle.lowTemp, dayNightCycle.highTemp, dayNightCycle.tempOverTime.Evaluate((dayNightCycle.time + hoursInFuture / 24f) % 1f))
                + dayNightCycle.windChill * intensity + (Mathf.PerlinNoise1D(Time.time * dayNightCycle.noiseSpeed) * dayNightCycle.tempFluctuation - dayNightCycle.tempFluctuation / 2f);
        }

        // Draws the current weather and the weather that will yet be.
        private void DrawWeatherForecast(DesktopDotExe.WindowView window)
        {
            sDayNightCycle dayNightCycle = FindFirstObjectByType<sDayNightCycle>();
            sWeatherSystem weatherSystem = dayNightCycle.gameObject.GetComponent<sWeatherSystem>();

            // Predict the future's weather. If there is a CarLessMode object in the scene then we can assume that there is no weather.
            bool hasWeather = dayNightCycle != null && FindFirstObjectByType<CarLessMode>() == null;
            float futureWeatherIntensity = hasWeather ? (float)WeatherTargetField.GetValue(dayNightCycle) : 1f;
            WeatherHUD.WeatherType futureWeather = (WeatherHUD.WeatherType)Mathf.Min(futureWeatherIntensity * 4, 3.999f);
            float futureTemperature = hasWeather ? PredictTemperature(dayNightCycle, ForecastTimeIntervalHours, futureWeatherIntensity) : -99f;

            // Get the current weather
            float currentWeatherIntensity = hasWeather ? weatherSystem.intensity : 1f;
            WeatherHUD.WeatherType currentWeather = (WeatherHUD.WeatherType)Mathf.Min(currentWeatherIntensity * 4, 3.999f);
            float currentTemperature = hasWeather ? dayNightCycle.Temperature() : -99f;

            // Draw the weather forecast label
            string forecastLabel = hasWeather ? "Forecast" : "NO SIGNAL";
            Util.R.fontOptions.alignment = sFancyText.FontOptions.Alignment.center;
            Util.R.fontOptions.mono = false;
            Util.R.fput(forecastLabel, P.center.x, P.yMin + HeaderOffsetY);

            // Prepare to draw the current weather and the future weather
            Texture spriteSheet = window.M.altSpriteSheet;
            int currentSpriteIndex = (int)currentWeather + 1;
            int futureSpriteIndex = (int)futureWeather + 1;

            // Draw the labels
            Util.R.put(CurrentLabel, CurrentLabelX + P.x, LabelY + P.y);
            Util.R.put(FutureLabel, CurrentLabelX + XOffset + P.x, LabelY + P.y);

            // Draw the weather icons.
            Util.R.spr(spriteSheet, currentSpriteIndex * SpriteSize, 0f, CurrentSpriteX + P.x, SpriteY + P.y, SpriteSize, SpriteSize, FlipSpriteHorizontally, SpriteSize, SpriteSize);
            Util.R.spr(spriteSheet, futureSpriteIndex * SpriteSize, 0f, CurrentSpriteX + XOffset + P.x, SpriteY + P.y, SpriteSize, SpriteSize, FlipSpriteHorizontally, SpriteSize, SpriteSize);

            // Draw the temperature labels.
            Util.R.put((int)currentTemperature, CurrentTemperatureLabelX + P.x, TemperatureLabelY + P.y);
            Util.R.put((int)futureTemperature, CurrentTemperatureLabelX + XOffset + P.x, TemperatureLabelY + P.y);
        }

        // Logic that allows the user to drag the window around by holding the top bar and moving the mouse or gamepad.
        private void DoWindowDrag(DesktopDotExe.WindowView window)
        {
            if (!BeingDragged)
            {
                // If the user holds the mouse's left button while hovering over the top bar of the window then start dragging the window.
                if (Util.MouseOver(P.xMin, P.yMin + TopBarYOffset, P.width, TopBarThickness))
                {
                    if (Util.M.mouseButton)
                    {
                        BeingDragged = true;
                        DragOffset = new Vector2(P.x - Util.M.mouse.x - 8, P.y - Util.M.mouse.y - 8); // Needed to add -8 or it'd be off.
                        Util.M.mouseIcon = ScreenProgram.HAND_CLOSED;
                    }
                    // If the user is hovering over the top bar but isn't holding the mouse's left button then change the mouse icon to indicate that the window can be dragged.
                    else
                    {
                        Util.M.mouseIcon = ScreenProgram.HAND_OPEN;
                    }
                }
                else
                {
                    // Set the cursor to an arrow when it is not hovering over anything interactable.
                    Util.M.mouseIcon = ScreenProgram.ARROW;
                }
            }
            else
            {
                Util.M.mouseIcon = ScreenProgram.HAND_CLOSED;

                // If the user lets go of the mouse's left button or the controller's primary button then stop dragging the window.
                if (!Util.M.mouseButton)
                {
                    BeingDragged = false;
                }
                // Drag the window by setting its position to the mouse's position plus the offset of when the user first started dragging the window.
                else
                {
                    window.targetPosition = (Util.M.mouse + DragOffset) / 8f;
                }
            }
        }
    }
}