using BepInEx;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyDeliveryAPI.ExampleMods
{
    [BepInDependency("EasyDeliveryAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(modID, "Bouncy Ball Mod", "1.0.0")]
    public class BouncyBallPlugin : BaseUnityPlugin
    {
        private sItemManager.ItemInfo bouncyBallItem;
        public const string modID = "BouncyBall"; // Only the first ten characters of the mod ID are displayed in EasyAPI, but you can make it longer if you wish.
        private void CreateBouncyBall()
        {
            // Create a bouncy ball item.
            bouncyBallItem = new sItemManager.ItemInfo()
            {
                name = "Bouncy Ball",
                description = "A ball that bounces when thrown.", // This description never shows in the game, but who knows? Maybe a mod will show this off in the future.
                price = 9.99f, // The default price when this item is found in a shop.
                prefab = null, // Used by the inventory manager when an item is held or displayed.
                itemInfoIndex = 0 // This value is ignored -- Easy API doesn't care what it is set to so we'll use it for our own reference instead.
            };

            // Register the item without a prefab first because we need to know the item's ID in order to create the prefab.
            // We can skip this step if we don't need to know the identity of the item for it to work.
            bouncyBallItem.itemInfoIndex = EasyAPI.AddItem(modID, bouncyBallItem);

            // Make a real bouncy ball.
            bouncyBallItem.prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bouncyBallItem.prefab.name = "Bouncy Ball";
            bouncyBallItem.prefab.transform.localScale = Vector3.one * 0.25f;
            SphereCollider collider = bouncyBallItem.prefab.GetComponent<SphereCollider>();
            collider.material = new PhysicsMaterial()
            {
                bounciness = 0.95f,
                bounceCombine = PhysicsMaterialCombine.Maximum,
                dynamicFriction = 0.5f,
                staticFriction = 0.5f,
                frictionCombine = PhysicsMaterialCombine.Average
            };

            // Change the color to yellow.
            Renderer renderer = bouncyBallItem.prefab.GetComponent<Renderer>();

            // Normally you'd load your own material here, but since this is a demo I'll use dark magic I'm not going to explain to fetch one from the game.
            renderer.material = Resources.LoadAll<Material>("")[8];
            renderer.material.color = Color.yellow;

            // Normally you'd load your own impact sounds here, but I'll just borrow a few sounds from the game. These come from the large box pickups which are, thankfully, loaded in the title screen.
            AudioClip[] impactSounds = Array.FindAll(Resources.FindObjectsOfTypeAll<AudioClip>(), clip => clip.name.Length == 11 && clip.name.Substring(0, 10)  == "fullImpact");
            AudioClip pickupSound = Array.Find(impactSounds, clip => clip.name == "fullImpact3");
            AudioSource audioSource = bouncyBallItem.prefab.AddComponent<AudioSource>();
            audioSource.name = pickupSound.name;
            audioSource.clip = pickupSound;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.1f; // The sounds are pretty loud, so let's turn it down a lot.
            ImpactSFX impactSFX = bouncyBallItem.prefab.AddComponent<ImpactSFX>();
            impactSFX.impactSounds = impactSounds;
            impactSFX.maxVelocity = 5f; // The velocity required to play sound at its fullest. Volume is scaled between the max and min so a larger max velocity results in quieter sounds overall.
            impactSFX.minVelocity = 0.1f; // The minimum velocity required for an impact sound to play. Cannot be zero.

            // Make the item leave trails when thrown in the snow.
            bouncyBallItem.prefab.AddComponent<StaticSnowDepression>();

            // Turn the item into a throwable item.
            ThrowableItem throwableComponent = bouncyBallItem.prefab.AddComponent<ThrowableItem>();
            throwableComponent.itemInfoIndex = bouncyBallItem.itemInfoIndex; // This is why we needed to register the item first; ThrowableItem needs the correct item ID in order to give the correct item when picked up.

            // Now that we have a functional prefab we register the same item again.
            EasyAPI.AddItem(modID, bouncyBallItem);
            Logger.LogInfo($"Registered bouncy ball item with ID {bouncyBallItem.itemInfoIndex}.");
        }
        private void SpawnBouncyBall(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SnowyPeaks")
            {
                // Let's spawn our item in MK's cabin. No fancy checks, it will always spawn here when the scene loads.
                Vector3 spawnPosition = new Vector3(-185.5811f, 162.373f, 268.1982f);
                GameObject bouncyBall = Instantiate(bouncyBallItem.prefab, spawnPosition, Quaternion.identity);

                // Make the item recognized by the game as an item that can be picked up.
                ItemID id = bouncyBall.AddComponent<ItemID>();
                id.id = bouncyBallItem.itemInfoIndex;
                Logger.LogInfo("Spawned a bouncy ball in MK's cabin.");
            }
        }
        private void Awake()
        {
            EasyAPI.OnLoad += (_) => CreateBouncyBall(); // EasyAPI.AddItem() will fail if it is not run inside this event.
            SceneManager.sceneLoaded += SpawnBouncyBall; // We can spawn our bouncy ball item once the scene has loaded.
        }
    }
}
