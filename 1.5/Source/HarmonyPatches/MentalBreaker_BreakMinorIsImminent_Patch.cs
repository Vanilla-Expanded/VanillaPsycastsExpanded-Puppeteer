using HarmonyLib;
using Verse.AI;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(MentalBreaker), "BreakMinorIsImminent", MethodType.Getter)]
    public static class MentalBreaker_BreakMinorIsImminent_Patch
    {
        [HarmonyPriority(int.MinValue)]
        public static void Postfix(MentalBreaker __instance, ref bool __result)
        {
            if (__instance.pawn.IsPuppet())
            {
                __result = false;
            }
        }
    }
}
