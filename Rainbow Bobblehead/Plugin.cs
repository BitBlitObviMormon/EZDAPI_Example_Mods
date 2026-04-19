using BepInEx;

namespace EasyDeliveryAPI.ExampleMods
{

    [BepInDependency("EasyDeliveryAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("ExampleMod.RainbowBobblehead", "Rainbow Bobblehead Mod", "1")]
    public class ProudGusPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            
        }
    }
}
