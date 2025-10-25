using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace ImposterItems
{
    [BepInPlugin(GUID, "Imposter Items", "1.0.6")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInDependency(Alexandria.Alexandria.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.imposteritems";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager game)
        {
            new Harmony(GUID).PatchAll();

            LilCrewmate.Init();
            VotingInterface.Init();
            ImpostersKnife.Init();
            ImpostersSidearm.Init();
        }
    }
}
