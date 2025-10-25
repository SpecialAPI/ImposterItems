using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace ImposterItems
{
    [BepInPlugin(MOD_GUID, "Imposter Items", "1.0.7")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInDependency(Alexandria.Alexandria.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public const string MOD_GUID = "spapi.etg.imposteritems";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager game)
        {
            new Harmony(MOD_GUID).PatchAll();
            ETGMod.Assets.SetupSpritesFromAssembly(typeof(Plugin).Assembly, "ImposterItems/Resources/MTGAPISpriteRoot");

            LilCrewmate.Init();
            VotingInterface.Init();
            ImpostersKnife.Init();
            ImpostersSidearm.Init();
        }
    }
}
