using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;

namespace ImposterItems
{
    class ImpostersKnife : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Imposter's Knife";
            string resourceName = "ImposterItems/Resources/imposter_knife";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<ImpostersKnife>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Wasn't me";
            string longDesc = "On use, deliver a quick short range, high damage stab towards a direction of your choosing.\n\nSharp, quick, reliable, and most importantly never runs out of ammo. It's no wonder why the knife is such an effective " +
                "killing device, held back only for it's short range..maybe there's some kind of workaround to that...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500);
            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;
			var cm = Instantiate((GameObject)BraveResources.Load("Global Prefabs/_ChallengeManager", ".prefab"));
			item.DarknessEffectShader = (cm.GetComponent<ChallengeManager>().PossibleChallenges.Where(c => c.challenge is DarknessChallengeModifier).First().challenge as DarknessChallengeModifier).DarknessEffectShader;
			VFXPool stabVFX = new VFXPool { type = VFXPoolType.All };
			VFXComplex complex = new VFXComplex();
			VFXObject vfxObj = new VFXObject
			{
				alignment = VFXAlignment.Fixed,
				attached = true,
				orphaned = false,
				persistsOnDeath = false,
				destructible = true,
				usesZHeight = true,
				zHeight = -0.25f
			};
			GameObject stabKnifeObj = SpriteBuilder.SpriteFromResource("ImposterItems/Resources/imposter_knife_stab", new GameObject("ImposterKnifeStab"));
			stabKnifeObj.SetActive(false);
			FakePrefab.MarkAsFakePrefab(stabKnifeObj);
			DontDestroyOnLoad(stabKnifeObj);
			tk2dSpriteAnimator animator = stabKnifeObj.AddComponent<tk2dSpriteAnimator>();
			SpriteBuilder.AddAnimation(animator, stabKnifeObj.GetComponent<tk2dBaseSprite>().Collection, new List<int> { stabKnifeObj.GetComponent<tk2dBaseSprite>().spriteId }, "stab", tk2dSpriteAnimationClip.WrapMode.Once).fps = 1;
			animator.playAutomatically = true;
			animator.DefaultClipId = animator.GetClipIdByName("stab");
			SpriteAnimatorKiller killer = stabKnifeObj.AddComponent<SpriteAnimatorKiller>();
			killer.fadeTime = -1f;
			killer.delayDestructionTime = -1f;
			killer.animator = animator;
			ConstructOffsetsFromAnchor(stabKnifeObj.GetComponent<tk2dBaseSprite>().GetCurrentSpriteDef(), tk2dBaseSprite.Anchor.MiddleLeft);
			vfxObj.effect = stabKnifeObj;
			complex.effects = new VFXObject[] { vfxObj };
			stabVFX.effects = new VFXComplex[] { complex };
			item.stabVfx = stabVFX;
			Destroy(cm);
			ImpostersKnifeId = item.PickupObjectId;
			Game.Items.Rename("spapi:imposter's_knife", "spapi:imposters_knife");
		}

		public static void ConstructOffsetsFromAnchor(tk2dSpriteDefinition def, tk2dBaseSprite.Anchor anchor, Vector2? scale = null, bool fixesScale = false, bool changesCollider = true)
		{
			if (!scale.HasValue)
			{
				scale = new Vector2?(def.position3);
			}
			if (fixesScale)
			{
				Vector2 fixedScale = scale.Value - def.position0.XY();
				scale = new Vector2?(fixedScale);
			}
			float xOffset = 0;
			if (anchor == tk2dBaseSprite.Anchor.LowerCenter || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.UpperCenter)
			{
				xOffset = -(scale.Value.x / 2f);
			}
			else if (anchor == tk2dBaseSprite.Anchor.LowerRight || anchor == tk2dBaseSprite.Anchor.MiddleRight || anchor == tk2dBaseSprite.Anchor.UpperRight)
			{
				xOffset = -scale.Value.x;
			}
			float yOffset = 0;
			if (anchor == tk2dBaseSprite.Anchor.MiddleLeft || anchor == tk2dBaseSprite.Anchor.MiddleCenter || anchor == tk2dBaseSprite.Anchor.MiddleLeft)
			{
				yOffset = -(scale.Value.y / 2f);
			}
			else if (anchor == tk2dBaseSprite.Anchor.UpperLeft || anchor == tk2dBaseSprite.Anchor.UpperCenter || anchor == tk2dBaseSprite.Anchor.UpperRight)
			{
				yOffset = -scale.Value.y;
			}
			MakeOffset(def, new Vector2(xOffset, yOffset), changesCollider);
		}

		public static void MakeOffset(tk2dSpriteDefinition def, Vector2 offset, bool changesCollider = false)
		{
			float xOffset = offset.x;
			float yOffset = offset.y;
			def.position0 += new Vector3(xOffset, yOffset, 0);
			def.position1 += new Vector3(xOffset, yOffset, 0);
			def.position2 += new Vector3(xOffset, yOffset, 0);
			def.position3 += new Vector3(xOffset, yOffset, 0);
			def.boundsDataCenter += new Vector3(xOffset, yOffset, 0);
			def.boundsDataExtents += new Vector3(xOffset, yOffset, 0);
			def.untrimmedBoundsDataCenter += new Vector3(xOffset, yOffset, 0);
			def.untrimmedBoundsDataExtents += new Vector3(xOffset, yOffset, 0);
			if (def.colliderVertices != null && def.colliderVertices.Length > 0 && changesCollider)
			{
				def.colliderVertices[0] += new Vector3(xOffset, yOffset, 0);
			}
		}

		private Vector4 GetCenterPointInScreenUV(Vector2 centerPoint)
		{
			Vector3 vector = GameManager.Instance.MainCameraController.Camera.WorldToViewportPoint(centerPoint.ToVector3ZUp(0f));
			return new Vector4(vector.x, vector.y, 0f, 0f);
		}

		public void LateUpdate()
		{
			if (m_material != null && m_isCurrentlyActive)
			{
				float num = GameManager.Instance.PrimaryPlayer.FacingDirection;
				if (num > 270f)
				{
					num -= 360f;
				}
				if (num < -270f)
				{
					num += 360f;
				}
				m_material.SetFloat("_ConeAngle", 0f);
				Vector4 centerPointInScreenUV = GetCenterPointInScreenUV(GameManager.Instance.PrimaryPlayer.CenterPosition);
				centerPointInScreenUV.z = num;
				Vector4 value = centerPointInScreenUV;
				if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER)
				{
					num = GameManager.Instance.SecondaryPlayer.FacingDirection;
					if (num > 270f)
					{
						num -= 360f;
					}
					if (num < -270f)
					{
						num += 360f;
					}
					value = GetCenterPointInScreenUV(GameManager.Instance.SecondaryPlayer.CenterPosition);
					value.z = num;
				}
				m_material.SetVector("_Player1ScreenPosition", centerPointInScreenUV);
				m_material.SetVector("_Player2ScreenPosition", value);
			}
		}

		public override void OnPreDrop(PlayerController user)
		{
			StopAllCoroutines();
			if (m_isCurrentlyActive)
			{
				if (Pixelator.Instance)
				{
					Pixelator.Instance.AdditionalCoreStackRenderPass = null;
				}
				user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
				user.ChangeSpecialShaderFlag(1, 0f);
				user.SetIsStealthed(false, "voting interface");
				user.SetCapableOfStealing(false, "VotingInterface", null);
			}
			user.RemoveActiveItem(PickupObjectId);
			EncounterTrackable.SuppressNextNotification = true;
			PlayerItem playerItem = Instantiate(PickupObjectDatabase.GetById(VotingInterface.VotingInterfaceId).gameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerItem>();
			playerItem.ForceAsExtant = true;
			playerItem.Pickup(user);
			EncounterTrackable.SuppressNextNotification = false;
			foreach (PlayerItem item in user.activeItems)
			{
				if (item is VotingInterface)
				{
					item.ForceApplyCooldown(user);
					user.DropActiveItem(item);
				}
			}
			Destroy(gameObject);
		}

		public override void DoEffect(PlayerController user)
		{
			AkSoundEngine.PostEvent("Play_ENV_puddle_zap_01", user.gameObject);
			m_material = new Material(DarknessEffectShader);
			Pixelator.Instance.AdditionalCoreStackRenderPass = m_material; user.ChangeSpecialShaderFlag(1, 1f);
			user.SetIsStealthed(true, "voting interface");
			user.SetCapableOfStealing(true, "VotingInterface", null);
			user.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
			StartCoroutine(ItemBuilder.HandleDuration(this, 6.5f, user, EndEffect));
		}

		public void EndEffect(PlayerController user)
		{
			if (Pixelator.Instance)
			{
				Pixelator.Instance.AdditionalCoreStackRenderPass = null;
			}
			user.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider));
			user.ChangeSpecialShaderFlag(1, 0f);
			user.SetIsStealthed(false, "voting interface");
			user.SetCapableOfStealing(false, "VotingInterface", null);
			user.RemoveActiveItem(PickupObjectId);
			EncounterTrackable.SuppressNextNotification = true;
			PlayerItem playerItem = Instantiate(PickupObjectDatabase.GetById(VotingInterface.VotingInterfaceId).gameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerItem>();
			playerItem.ForceAsExtant = true;
			playerItem.Pickup(user);
			EncounterTrackable.SuppressNextNotification = false;
			foreach (PlayerItem item in user.activeItems)
			{
				if (item is VotingInterface)
				{
					item.ForceApplyCooldown(user);
				}
			}
		}

		public override void DoActiveEffect(PlayerController user)
        {
            base.DoActiveEffect(user);
            if (isStabbing)
            {
				return;
            }
			Vector2 vector = user.unadjustedAimPoint.XY() - user.CenterPosition;
			float zRotation = BraveMathCollege.Atan2Degrees(vector);
			float rayDamage = 300f;
			float rayLength = 1.6875f;
			stabVfx.SpawnAtPosition(user.CenterPosition, zRotation, user.transform, null, null, new float?(1f), false, null, user.sprite, true);
			user.StartCoroutine(HandleSwing(user, vector, rayDamage, rayLength));
		}

		private IEnumerator HandleSwing(PlayerController user, Vector2 aimVec, float rayDamagePerSecond, float rayLength)
		{
			isStabbing = true;
			float elapsed = 0f;
			while (elapsed < 1f)
			{
				elapsed += BraveTime.DeltaTime;
				SpeculativeRigidbody hitRigidbody = IterativeRaycast(user.CenterPosition, aimVec, rayLength, int.MaxValue, user.specRigidbody);
				if (hitRigidbody && hitRigidbody.aiActor && hitRigidbody.aiActor.IsNormalEnemy)
				{
					hitRigidbody.aiActor.healthHaver.ApplyDamage(rayDamagePerSecond * BraveTime.DeltaTime, aimVec, "Imposter's Knife", CoreDamageTypes.None, DamageCategory.Normal, false, null, false);
				}
				yield return null;
			}
			isStabbing = false;
			yield break;
		}

		protected SpeculativeRigidbody IterativeRaycast(Vector2 rayOrigin, Vector2 rayDirection, float rayDistance, int collisionMask, SpeculativeRigidbody ignoreRigidbody)
		{
			int num = 0;
			while (PhysicsEngine.Instance.Raycast(rayOrigin, rayDirection, rayDistance, out var raycastResult, true, true, collisionMask, new CollisionLayer?(CollisionLayer.Projectile), false, null, ignoreRigidbody))
			{
				num++;
				SpeculativeRigidbody speculativeRigidbody = raycastResult.SpeculativeRigidbody;
				if (num < 3 && speculativeRigidbody != null)
				{
					MinorBreakable component = speculativeRigidbody.GetComponent<MinorBreakable>();
					if (component != null)
					{
						component.Break(rayDirection.normalized * 3f);
						RaycastResult.Pool.Free(ref raycastResult);
						continue;
					}
				}
				RaycastResult.Pool.Free(ref raycastResult);
				return speculativeRigidbody;
			}
			return null;
		}

		private Material m_material;
		public static int ImpostersKnifeId;
		public Shader DarknessEffectShader;
		public bool isStabbing;
		public VFXPool stabVfx;
	}
}
