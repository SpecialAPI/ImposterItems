using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace ImposterItems
{
    class ImpostersSidearm : GunBehaviour
    {
        public static void Init()
        {
            Gun gun = ETGMod.Databases.Items.NewGun("Impostor's Sidearm", "impgun");
            Game.Items.Rename("outdated_gun_mods:impostor's_sidearm", "spapi:impostors_sidearm");
            GunExt.SetShortDescription(gun, "No!! Please!");
            GunExt.SetLongDescription(gun, "\n\nNormally locked up in the weapons cache, This gun my be responsible for countless accounts of cold blooded murder. Now why would a mere crewmate need such a bulky, powerful weapon.. " +
                "as one fellow crewmate once stated \"That's kind of sus\"");
            GunExt.SetupSprite(gun, null, "impgun_idle_001", 12);
            GunExt.AddProjectileModuleFrom(gun, "klobb", true, false);
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.angleVariance = 0;
            gun.gameObject.AddComponent<ImpostersSidearm>();
            gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.MEDIUM_BULLET;
            Projectile projectile = Instantiate((PickupObjectDatabase.GetById(56) as Gun).DefaultModule.projectiles[0]);
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;
            projectile.baseData.damage = 12f;
            projectile.name = "ImposterGun_Projectile";
            gun.DefaultModule.cooldownTime = 0.5f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.reloadTime = 1.89f;
            gun.InfiniteAmmo = true;
            gun.quality = PickupObject.ItemQuality.SPECIAL;
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(56) as Gun).muzzleFlashEffects;
            gun.barrelOffset.transform.localPosition = new Vector3(1.0625f, 0.5625f, 0f);
            gun.gunClass = GunClass.PISTOL;
            for (int i = 0; i < gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames.Length; i++)
            {
                gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[i].triggerEvent = true;
                tk2dSpriteDefinition def = gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[i].spriteCollection.spriteDefinitions[gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).frames[i].spriteId];
                Vector2 offset = new Vector2(((float)fireOffsets[i].x) / 16f, ((float)fireOffsets[i].y) / 16f);
                ImpostersKnife.MakeOffset(def, offset);
            }
            for (int i = 0; i < gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames.Length; i++)
            {
                gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[i].triggerEvent = true;
                tk2dSpriteDefinition def = gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[i].spriteCollection.spriteDefinitions[gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.reloadAnimation).frames[i].spriteId];
                Vector2 offset = new Vector2(((float)reloadOffsets[i].x) / 16f, ((float)reloadOffsets[i].y) / 16f);
                ImpostersKnife.MakeOffset(def, offset);
            }
            for (int i = 0; i < gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.introAnimation).frames.Length; i++)
            {
                gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.introAnimation).frames[i].triggerEvent = true;
                tk2dSpriteDefinition def = gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.introAnimation).frames[i].spriteCollection.spriteDefinitions[gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.introAnimation).frames[i].spriteId];
                Vector2 offset = new Vector2(((float)introOffsets[i].x) / 16f, ((float)introOffsets[i].y) / 16f);
                ImpostersKnife.MakeOffset(def, offset);
            }
            for (int i = 0; i < gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames.Length; i++)
            {
                gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames[i].triggerEvent = true;
                tk2dSpriteDefinition def = gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames[i].spriteCollection.spriteDefinitions[gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.emptyAnimation).frames[i].spriteId];
                Vector2 offset = new Vector2(((float)emptyOffsets[i].x) / 16f, ((float)emptyOffsets[i].y) / 16f);
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
