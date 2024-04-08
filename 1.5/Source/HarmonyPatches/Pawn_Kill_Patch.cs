using HarmonyLib;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(Pawn), "Kill")]
    public static class Pawn_Kill_Patch
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
        }
    }
}
