using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;
using ItemAPI;

namespace ImposterItems
{
    [BepInPlugin(GUID, "Imposter Items", "1.0.5")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.imposteritems";

        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager game)
        {
            new Harmony(GUID).PatchAll();

            ItemBuilder.Init();
            LilCrewmate.Init();
            VotingInterface.Init();
            ImpostersKnife.Init();
            ImpostersSidearm.Init();
        }
    }
}
