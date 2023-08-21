using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker), "AddNeed")]
    public static class Pawn_NeedsTracker_AddNeed_Patch
    {
        [HarmonyPriority(int.MaxValue)]
        public static bool Prefix(Pawn ___pawn, NeedDef nd)
        {
            if (___pawn.IsPuppet())
            {
                if (VPEP_DefOf.VPEP_PuppetSettings.needsForPuppets.Contains(nd.defName) is false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
