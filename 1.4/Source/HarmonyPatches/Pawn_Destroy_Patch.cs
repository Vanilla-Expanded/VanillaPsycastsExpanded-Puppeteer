using HarmonyLib;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(Pawn), "Destroy")]
    public static class Pawn_Destroy_Patch
    {
        private static void Prefix(Pawn __instance)
        {
            var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet);
            if (hediff != null)
            {
                __instance.health.RemoveHediff(hediff);
            }
        }
    }
}
