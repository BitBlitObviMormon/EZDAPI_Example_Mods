using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyDeliveryAPI.ExampleMods
{

    [BepInDependency("EasyDeliveryAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("ExampleMod.BouncyBall", "Bouncy Ball Mod", "1")]
    public class BouncyBallPlugin : BaseUnityPlugin
    {
        private int id = 0;
        private void CreateBouncyBall()
        {
            // Create a bouncy ball item.
            sItemManager.ItemInfo bouncyBallItem = new sItemManager.ItemInfo()
            {
                name = "Bouncy Ball",
                description = "A ball that bounces when thrown.", // This description never shows in the game, but it's nice to have one anyway.
                price = 9.99f, // The default price when this item is found in a shop.
                prefab = null // Used by the inventory manager when an item is created.
            };

            // Register the item without a prefab first because we need to know the item's ID in order to create the prefab.
            // We can skip this step if we don't need to know the identity of the item for it to work.
            const string modID = "ExampleMod.BouncyBall";
            id = EasyAPI.AddItem(modID, bouncyBallItem);

            // Make a real bouncy ball.
            bouncyBallItem.prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SphereCollider collider = bouncyBallItem.prefab.AddComponent<SphereCollider>();
            collider.material = new PhysicsMaterial()
            {
                bounciness = 0.9f,
                bounceCombine = PhysicsMaterialCombine.Maximum,
                dynamicFriction = 0.5f,
                staticFriction = 0.5f,
                frictionCombine = PhysicsMaterialCombine.Average
            };

            // Change the color to yellow.
            Renderer renderer = bouncyBallItem.prefab.GetComponent<Renderer>();
            renderer.material.color = Color.yellow;

            // Turn the item into a throwable item.
            ThrowableItem throwableComponent = bouncyBallItem.prefab.AddComponent<ThrowableItem>();
            throwableComponent.itemInfoIndex = id; // This is why we needed to register the item first; ThrowableItem needs the correct item ID in order to give the correct item when picked up.

            // Now that we have a functional prefab we register the same item again.
            EasyAPI.AddItem(modID, bouncyBallItem);
        }
        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SnowyPeaks")
            {
                if (id == 0)
                {
                    // Wait for the save to finish loading.
                }
                else
                {
                    // We already know the ID so let's spawn our item.
                }
            }
        }
        private void Awake()
        {
            EasyAPI.OnLoad += (_) => CreateBouncyBall(); // Run this code during save load. EasyAPI.AddItem() will fail if not done here.
            SceneManager.sceneLoaded += OnSceneLoad; // We can spawn our bouncy ball item once the scene has loaded.
        }
    }
}
