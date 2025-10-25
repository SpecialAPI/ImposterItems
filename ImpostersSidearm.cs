using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Gungeon;

namespace ImposterItems
{
    public class ImpostersSidearm : GunBehaviour
    {
        public static void Init()
        {
            var gun = ETGMod.Databases.Items.NewGun("Impostor's Sidearm", "impgun");
            Game.Items.Rename("outdated_gun_mods:impostor's_sidearm", "spapi:impostors_sidearm");

            gun.SetShortDescription("No!! Please!");
            gun.SetLongDescription("\n\nNormally locked up in the weapons cache, This gun my be responsible for countless accounts of cold blooded murder. Now why would a mere crewmate need such a bulky, powerful weapon.. as one fellow crewmate once stated \"That's kind of sus\"");
            gun.SetupSprite(null, "impgun_idle_001", 12);

            gun.AddProjectileModuleFrom("klobb", true, false);
            var projectile = Instantiate((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 12f;
            projectile.name = "ImposterGun_Projectile";

            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 8;

            gun.gameObject.AddComponent<ImpostersSidearm>();
            gun.reloadTime = 1.89f;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(56) as Gun).muzzleFlashEffects;
            gun.barrelOffset.transform.localPosition = new Vector3(1.0625f, 0.5625f, 0f);
            gun.gunClass = GunClass.PISTOL;

            var animator = gun.GetComponent<tk2dSpriteAnimator>();
            var shootAnim = animator.GetClipByName(gun.shootAnimation);
            var reloadAnim = animator.GetClipByName(gun.reloadAnimation);
            var introAnim = animator.GetClipByName(gun.introAnimation);
            var emptyAnim = animator.GetClipByName(gun.emptyAnimation);

            for (var i = 0; i < shootAnim.frames.Length; i++)
            {
                var frame = shootAnim.frames[i];
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                var offset = new Vector2(fireOffsets[i].x / 16f, fireOffsets[i].y / 16f);

                ImpostersKnife.MakeOffset(def, offset);
            }
            for (var i = 0; i < reloadAnim.frames.Length; i++)
            {
                var frame = reloadAnim.frames[i];
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                var offset = new Vector2(reloadOffsets[i].x / 16f, reloadOffsets[i].y / 16f);

                ImpostersKnife.MakeOffset(def, offset);
            }
            for (var i = 0; i < introAnim.frames.Length; i++)
            {
                var frame = introAnim.frames[i];
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                var offset = new Vector2(introOffsets[i].x / 16f, introOffsets[i].y / 16f);

                ImpostersKnife.MakeOffset(def, offset);
            }
            for (var i = 0; i < emptyAnim.frames.Length; i++)
            {
                var frame = emptyAnim.frames[i];
                var def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
                var offset = new Vector2(emptyOffsets[i].x / 16f, emptyOffsets[i].y / 16f);

                ImpostersKnife.MakeOffset(def, offset);
            }

            ETGMod.Databases.Items.Add(gun, null, "ANY");
        }

        public override void Update()
        {
            if (gun != null)
            {
                if (!gun.PreventNormalFireAudio)
                {
                    gun.PreventNormalFireAudio = true;
                }
                if (gun.OverrideNormalFireAudioEvent != "Play_WPN_sniperrifle_shot_01")
                {
                    gun.OverrideNormalFireAudioEvent = "Play_WPN_sniperrifle_shot_01";
                }
            }
        }

        public static List<IntVector2> fireOffsets = new List<IntVector2>
        {
            new IntVector2(-4, 1),
            new IntVector2(-3, 0),
            new IntVector2(-2, 0),
            new IntVector2(-1, 0),
            new IntVector2(0, 0)
        };
        public static List<IntVector2> reloadOffsets = new List<IntVector2>
        {
            new IntVector2(-4, 0),
            new IntVector2(-4, 2),
            new IntVector2(-4, 7),
            new IntVector2(-4, 8),
            new IntVector2(-4, -3),
            new IntVector2(-4, 0),
            new IntVector2(4, 0),
            new IntVector2(2, 0),
            new IntVector2(1, 0)
        };
        public static List<IntVector2> introOffsets = new List<IntVector2>
        {
            new IntVector2(0, 3),
            new IntVector2(-1, 0),
            new IntVector2(2, -2),
            new IntVector2(3, -8),
            new IntVector2(3, -3),
            new IntVector2(3, 6),
            new IntVector2(3, 5),
            new IntVector2(0, -5),
            new IntVector2(2, -7),
            new IntVector2(4, -6),
            new IntVector2(4, -7)
        };
        public static List<IntVector2> emptyOffsets = new List<IntVector2>
        {
            new IntVector2(-4, 0)
        };
    }
}
