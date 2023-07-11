using RimWorld;
using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;

namespace VPEPuppeteer
{
    public abstract class Hediff_PuppetBase : HediffWithComps
    {
        public static bool preventRemoveEffects;

        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            pawn.health.RemoveHediff(this);
        }
    }
}
