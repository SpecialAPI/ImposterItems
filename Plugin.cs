using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using ItemAPI;

namespace ImposterItems
{
    [BepInPlugin("spapi.etg.imposteritems", "Imposter Items", "1.0.3")]
    [BepInDependency(ETGModMainBehaviour.GUID)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager game)
        {
            ItemBuilder.Init();
            LilCrewmate.Init();
            VotingInterface.Init();
            ImpostersKnife.Init();
            ImpostersSidearm.Init();
        }
    }
}
