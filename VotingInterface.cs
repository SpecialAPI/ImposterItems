using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace ImposterItems
{
    class VotingInterface : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Voting Interface";
            string resourceName = "ImposterItems/Resources/busted_interface";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<VotingInterface>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Sabotage";
            string longDesc = "Allows the user to remotely turn off the lights and slip into darkness. While the lights are dimmed gain stealth.\n\nThis tablet was issued to you and the rest of your crew in tact with multiple apps, most notably ones " +
                "allowing you to anomalously vote in crew meetings, view your surroundings, and remotely control linked electronics. Luckily The Gungeon doesn't have a great firewall...";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500);
            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;
            VotingInterfaceId = item.PickupObjectId;
		}

        public override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            user.RemoveActiveItem(PickupObjectId);
            EncounterTrackable.SuppressNextNotification = true;
            PlayerItem playerItem = Instantiate(PickupObjectDatabase.GetById(ImpostersKnife.ImpostersKnifeId).gameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerItem>();
            playerItem.ForceAsExtant = true;
            playerItem.Pickup(user);
            EncounterTrackable.SuppressNextNotification = false;
            foreach(PlayerItem item in user.activeItems)
            {
                if(item is ImpostersKnife)
                {
                    item.Use(user, out _);
                }
            }
        }

        public static int VotingInterfaceId;
    }
}
