using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;
using DirectionType = DirectionalAnimation.DirectionType;
using AnimationType = ItemAPI.CompanionBuilder.AnimationType;
using FlipType = DirectionalAnimation.FlipType;
using MonoMod.RuntimeDetour;

namespace ImposterItems
{
    public class LilCrewmate
    {
        public static GameObject prefab;

        public static void Init()
        {
            var itemName = "Lil' Crewmate";
            var resourceName = "ImposterItems/Resources/lilcrewmate";

            var obj = new GameObject(itemName);
            var item = obj.AddComponent<CompanionItem>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            var shortDesc = "Just look at him!";
            var longDesc = "This little crewmate went with the imposter when they where both ejected off the ship.\n\nHe doesn’t talk, he doesn’t fight, he honestly doesn’t do much of anything aside from being cute. At least the company must be nice.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            Game.Items.Rename("spapi:lil'_crewmate", "spapi:lil_crewmate");

            item.quality = PickupObject.ItemQuality.SPECIAL;
            item.CompanionGuid = "lil_crewmate";
            item.Synergies = [];
            BuildPrefab();
        }

        public static void BuildPrefab()
        {
            if (prefab != null || CompanionBuilder.companionDictionary.ContainsKey("lil_crewmate"))
                return;

            prefab = CompanionBuilder.BuildPrefab("Lil Crewmate", "lil_crewmate", "ImposterItems/Resources/Crewmate/IdleRight/lilguy_idle_right_001", new IntVector2(0, 0), new IntVector2(7, 9));

            var companion = prefab.AddComponent<CompanionController>();
            companion.CanBePet = true;
            companion.CanInterceptBullets = true;

            var rigidbody = companion.specRigidbody;
            var collider = new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.EnemyCollider,
                ManualWidth = 7,
                ManualHeight = 9,
                ManualOffsetX = 0,
                ManualOffsetY = 0
            };
            var hitbox = new PixelCollider
            {
                ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Manual,
                CollisionLayer = CollisionLayer.PlayerHitBox,
                ManualWidth = 7,
                ManualHeight = 9,
                ManualOffsetX = 0,
                ManualOffsetY = 0
            };
            rigidbody.PixelColliders = new()
            {
                collider,
                hitbox
            };
            rigidbody.CollideWithOthers = true;

            var aiactor = companion.aiActor;
            aiactor.IsNormalEnemy = false;
            aiactor.CollisionDamage = 0f;
            aiactor.MovementSpeed = 7.2f;
            aiactor.CanDropCurrency = false;

            prefab.AddAnimation("idle_right", "ImposterItems/Resources/Crewmate/IdleRight", 5, AnimationType.Idle, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("idle_left", "ImposterItems/Resources/Crewmate/IdleLeft", 5, AnimationType.Idle, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("run_right", "ImposterItems/Resources/Crewmate/MoveRight", 16, AnimationType.Move, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("run_left", "ImposterItems/Resources/Crewmate/MoveLeft", 16, AnimationType.Move, DirectionType.TwoWayHorizontal);
            prefab.AddAnimation("pet_right", "ImposterItems/Resources/Crewmate/PetRight", 5, AnimationType.Move, DirectionType.TwoWayHorizontal, assignAnimation: false);
            prefab.AddAnimation("pet_left", "ImposterItems/Resources/Crewmate/PetLeft", 5, AnimationType.Move, DirectionType.TwoWayHorizontal, assignAnimation: false);

            var petAnim = new DirectionalAnimation()
            {
                AnimNames = ["pet_right", "pet_left"],
                Flipped = [FlipType.None, FlipType.None],
                Type = DirectionType.TwoWayHorizontal,
                Prefix = string.Empty
            };
            companion.aiAnimator.AssignDirectionalAnimation("pet", petAnim, AnimationType.Other);

            var behavior = prefab.GetComponent<BehaviorSpeculator>();
            behavior.MovementBehaviors.Add(new CompanionFollowPlayerBehavior()
            {
                IdleAnimations = ["idle"],
                DisableInCombat = false
            });

            var petOffet = prefab.AddComponent<PetOffsetHolder>();
            petOffet.petOffsetLeft = new(-0.5f, -0.0625f);
            petOffet.petOffsetRight = new(0.5f, -0.0625f);
        }
    }
}
