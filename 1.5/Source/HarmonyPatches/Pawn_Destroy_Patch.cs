using HarmonyLib;
using System.Linq;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(Pawn), "Destroy")]
    public static class Pawn_Destroy_Patch
    {
        private static void Prefix(Pawn __instance)
        {
            if (__instance.IsPuppeteer(out var puppeteer))
            {
                if (puppeteer.TryTransferMind() is false)
                {
                    __instance.health.RemoveHediff(puppeteer);
                }
            }

            var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet);
            if (hediff != null)
            {
                __instance.health.RemoveHediff(hediff);
            }
        }
    }
}
