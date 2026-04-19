using UnityEngine;

namespace EasyDeliveryAPI.ExampleMods
{
    // This class is NOT USED and development is PUT ON HOLD until I learn how to keep EasyAPI.AddProgram() from bricking the game.
    public class WeatherGame : ScreenProgram
    {
        private GamepadNavigation nav;
        private AudioSource audioSource;
        public AudioClip hoverSound;
        private int SetupFuncCalls = 0;
        private int ResumeFuncCalls = 0;


        // This function is called once when the program is first launched.
        public override void Setup()
        {
            SetupFuncCalls++;

            // Since we want to the hover sound to be the same as the rest of the desktop, we'll fetch our hover sound from DesktopDoExe.
            // audioSource = gameObject.AddComponent<AudioSource>();
            // hoverSound = audioSource.clip;
            


            // nav = new GamepadNavigation(this, audioSource, hoverSound);
        }
        public override void Resume()
        {
            ResumeFuncCalls++;
            UIExamplePlugin.Logger.LogInfo("Resume func called. Total: " + ResumeFuncCalls);
        }
        public override void Draw()
        {
            // UIUtil Util = new UIUtil(R, this, nav);
        }
    }
}