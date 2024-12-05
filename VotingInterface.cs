using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace ImposterItems
{
    public class VotingInterface : PlayerItem
    {
        public static int VotingInterfaceId;

        public static void Init()
        {
            var itemName = "Voting Interface";
            var resourceName = "ImposterItems/Resources/busted_interface";

            var obj = new GameObject(itemName);
            var item = obj.AddComponent<VotingInterface>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            var shortDesc = "Sabotage";
            var longDesc = "Allows the user to remotely turn off the lights and slip into darkness. While the lights are dimmed gain stealth.\n\nThis tablet was issued to you and the rest of your crew in tact with multiple apps, most notably ones " +
                "allowing you to anomalously vote in crew meetings, view your surroundings, and remotely control linked electronics. Luckily The Gungeon doesn't have a great firewall...";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500);

            item.consumable = false;
            item.quality = ItemQuality.SPECIAL;
            VotingInterfaceId = item.PickupObjectId;
		}

        public override void DoEffect(PlayerController user)
        {
            user.RemoveActiveItem(PickupObjectId);

            EncounterTrackable.SuppressNextNotification = true;
            var knife = Instantiate(PickupObjectDatabase.GetById(ImpostersKnife.ImpostersKnifeId).gameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerItem>();
            knife.ForceAsExtant = true;
            knife.Pickup(user);
            EncounterTrackable.SuppressNextNotification = false;

            foreach(var item in user.activeItems)
            {
                if (item is not ImpostersKnife)
                    continue;

                item.Use(user, out _);
                break;
            }
        }
    }
}
