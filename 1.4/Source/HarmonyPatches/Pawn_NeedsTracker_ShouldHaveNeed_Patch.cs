using HarmonyLib;
using RimWorld;
using System.Linq;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "ShouldHaveNeed")]
    public static class Pawn_NeedsTracker_ShouldHaveNeed_Patch
    {
        [HarmonyPriority(int.MinValue)]
        public static void Postfix(Pawn ___pawn, NeedDef nd, ref bool __result)
        {
            if (___pawn.IsPuppet())
            {
                if (nd != NeedDefOf.Food && nd != NeedDefOf.Rest)
                {
                    __result = false;
                }
            }
        }
    }
}
