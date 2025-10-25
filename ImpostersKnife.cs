using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Alexandria.ItemAPI;
using Gungeon;

namespace ImposterItems
{
    public class ImpostersKnife : PlayerItem
    {
        private Material m_material;
        public static int ImpostersKnifeId;
        public Shader DarknessEffectShader;
        public bool isStabbing;
        public VFXPool stabVfx;

        public static void Init()
        {
            var itemName = "Imposter's Knife";
            var resourceName = "ImposterItems/Resources/imposter_knife";

            var obj = new GameObject(itemName);
            var item = obj.AddComponent<ImpostersKnife>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            var shortDesc = "Wasn't me";
            var longDesc = "On use, deliver a quick short range, high damage stab towards a direction of your choosing.\n\nSharp, quick, reliable, and most importantly never runs out of ammo. It's no wonder why the knife is such an effective killing device, held back only for it's short range..maybe there's some kind of workaround to that...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500);

            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;
            item.DarknessEffectShader = ShaderCache.Acquire("Hidden/DarknessChallengeShader");

            var stabKnifeObj = SpriteBuilder.SpriteFromResource("ImposterItems/Resources/imposter_knife_stab", new GameObject("ImposterKnifeStab"));
            stabKnifeObj.SetActive(false);
            FakePrefab.MarkAsFakePrefab(stabKnifeObj);
            DontDestroyOnLoad(stabKnifeObj);

            var animator = stabKnifeObj.AddComponent<tk2dSpriteAnimator>();
            SpriteBuilder.AddAnimation(animator, stabKnifeObj.GetComponent<tk2dBaseSprite>().Collection, [stabKnifeObj.GetComponent<tk2dBaseSprite>().spriteId], "stab", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
            animator.playAutomatically = true;
            animator.DefaultClipId = animator.GetClipIdByName("stab");

            var killer = stabKnifeObj.AddComponent<SpriteAnimatorKiller>();
            killer.fadeTime = -1f;
            killer.delayDestructionTime = -1f;
            killer.animator = animator;

            ConstructOffsetsFromAnchor(stabKnifeObj.GetComponent<tk2dBaseSprite>().GetCurrentSpriteDef(), tk2dBaseSprite.Anchor.MiddleLeft);

            item.stabVfx = new()
            {
                type = VFXPoolType.All,
                effects =
                [
                    new()
                    {
                        effects =
                        [
                            new()
                            {
                                alignment = VFXAlignment.Fixed,
                                attached = true,
                                orphaned = false,
                                persistsOnDeath = false,
                                destructible = true,
                                usesZHeight = true,
                                zHeight = -0.25f,
                                effect = stabKnifeObj
                            }
                        ]
                    }
                ]
            };

            ImpostersKnifeId = item.PickupObjectId;
            Game.Items.Rename("spapi:imposter's_knife", "spapi:imposters_knife");
        }

        public static void ConstructOffsetsFromAnchor(tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2? scale = null, bool fixesScale = false, bool changesCollider = true)
        {
            scale ??= def.position3;

            if (fixesScale)
                scale = scale.GetValueOrDefault() - def.position0.XY();

            var xOffset = ((int)anchor % 3) switch
            {
                1 => -(scale.Value.x / 2f),
                2 => -scale.Value.x,

                _ => 0
            };

            var yOffset = ((int)anchor / 3) switch
            {
                1 => -(scale.Value.y / 2f),
                2 => -scale.Value.y,

                _ => 0
            };

            MakeOffset(def, new Vector2(xOffset, yOffset), changesCollider);
        }

        public static void MakeOffset(tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
        {
            var xOffset = offset.x;
            var yOffset = offset.y;

            def.position0 += new Vector3(xOffset, yOffset, 0);
            def.position1 += new Vector3(xOffset, yOffset, 0);
            def.position2 += new Vector3(xOffset, yOffset, 0);
            def.position3 += new Vector3(xOffset, yOffset, 0);

            def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);

            def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
            def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);

            if (def.colliderVertices == null || def.colliderVertices.Length == 0 || !changesCollider)
                return;

            def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
        }

        public Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
        {
            var vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));

            return new Vector4(vector.x, vector.y, 0f, 0f);
        }

        public void LateUpdate()
        {
            if (m_material == null || !m_isCurrentlyActive)
                return;

            float p1Dir = GameManager.Instance.PrimaryPlayer.FacingDirection;

            if (p1Dir > 270f)
                p1Dir -= 360f;
            if (p1Dir < -270f)
                p1Dir += 360f;

            m_material.SetFloat("_ConeAngle", 0f);

            var pos1 = GetCenterPointInScreenUV(GameManager.Instance.PrimaryPlayer.CenterPosition);
            pos1.z = p1Dir;

            var pos2 = pos1;
            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
            {
                var p2Dir = GameManager.Instance.SecondaryPlayer.FacingDirection;

                if (p2Dir > 270f)
                    p2Dir -= 360f;
                if (p2Dir < -270f)
                    p2Dir += 360f;

                pos2 = GetCenterPointInScreenUV(GameManager.Instance.SecondaryPlayer.CenterPosition);
                pos2.z = p2Dir;
            }

            m_material.SetVector("_Player1ScreenPosition", pos1);
            m_material.SetVector("_Player2ScreenPosition", pos2);
        }

        public override void OnPreDrop(PlayerController user)
        {
            StopAllCoroutines();
            if (m_isCurrentlyActive)
            {
                if (Pixelator.Instance)
                    Pixelator.Instance.AdditionalCoreStackRenderPass = null;

                user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
                user.ChangeSpecialShaderFlag(1, 0f);
                user.SetIsStealthed(false, "voting interface");
                user.SetCapableOfStealing(false, "VotingInterface", null);
            }

            EncounterTrackable.SuppressNextNotification = true;
            var votingIntf = Instantiate(PickupObjectDatabase.GetById(VotingInterface.VotingInterfaceId).gameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerItem>();
            votingIntf.ForceAsExtant = true;
            votingIntf.Pickup(user);
            EncounterTrackable.SuppressNextNotification = false;

            foreach (var item in user.activeItems)
            {
                if (item is not VotingInterface)
                    continue;

                item.ForceApplyCooldown(user);
                user.DropActiveItem(item);
                break;
            }

            user.RemoveActiveItem(PickupObjectId);
            Destroy(gameObject);
        }

        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_ENV_puddle_zap_01", user.gameObject);

            m_material = new Material(DarknessEffectShader);
            Pixelator.Instance.AdditionalCoreStackRenderPass = m_material;

            user.ChangeSpecialShaderFlag(1, 1f);
            user.SetIsStealthed(true, "voting interface");
            user.SetCapableOfStealing(true, "VotingInterface", null);
            user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));

            StartCoroutine(ItemBuilder.HandleDuration(this, 6.5f, user, EndEffect));
        }

        public void EndEffect(PlayerController user)
        {
            if (Pixelator.Instance)
                Pixelator.Instance.AdditionalCoreStackRenderPass = null;

            user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
            user.ChangeSpecialShaderFlag(1, 0f);
            user.SetIsStealthed(false, "voting interface");
            user.SetCapableOfStealing(false, "VotingInterface", null);

            user.RemoveActiveItem(PickupObjectId);

            EncounterTrackable.SuppressNextNotification = true;
            var votingIntf = Instantiate(PickupObjectDatabase.GetById(VotingInterface.VotingInterfaceId).gameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerItem>();
            votingIntf.ForceAsExtant = true;
            votingIntf.Pickup(user);
            EncounterTrackable.SuppressNextNotification = false;

            foreach (var item in user.activeItems)
            {
                if (item is not VotingInterface)
                    continue;

                item.ForceApplyCooldown(user);
            }
        }

        public override void DoActiveEffect(PlayerController user)
        {
            base.DoActiveEffect(user);

            if (isStabbing)
                return;

            var dir = user.unadjustedAimPoint.XY() - user.CenterPosition;
            var zRotation = dir.ToAngle();
            stabVfx.SpawnAtPosition(user.CenterPosition, zRotation, user.transform, null, null, 1f, false, null, user.sprite, true);

            var rayDamage = 300f;
            var rayLength = 1.6875f;
            user.StartCoroutine(HandleSwing(user, dir, rayDamage, rayLength));
        }

        public IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamagePerSecond, float rayLength)
        {
            isStabbing = true;
            var elapsed = 0f;

            while (elapsed < 1f)
            {
                elapsed += BraveTime.DeltaTime;

                var hitRigidbody = IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
                if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
                    hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamagePerSecond * BraveTime.DeltaTime, aimVec, "Imposter's Knife", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);

                yield return null;
            }

            isStabbing = false;
        }

        public SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
        {
            var hitRB = 0;

            while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out var raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, null, ignoreRigidbody))
            {
                hitRB++;

                var speculativeRigidbody = raycastResult.SpeculativeRigidbody;
                if (hitRB < 3 && speculativeRigidbody != null)
                {
                    var breakable = speculativeRigidbody.GetComponent<MinorBreakable>();
                    if (breakable != null)
                    {
                        breakable.Break(rayDirection.normalized * 3f);
                        RaycastResult.Pool.Free(ref raycastResult);
                        continue;
                    }
                }

                RaycastResult.Pool.Free(ref raycastResult);
                return speculativeRigidbody;
            }

            return null;
        }
    }
}
