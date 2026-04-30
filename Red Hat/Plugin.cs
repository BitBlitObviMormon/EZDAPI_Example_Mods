using BepInEx;

namespace EasyDeliveryAPI.ExampleMods
{

    [BepInDependency("EasyDeliveryAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("ExampleMod.RedHatLinux", "Red Hat Mod", "1.0.0")]
    public class RedHatPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            
        }
    }
}
