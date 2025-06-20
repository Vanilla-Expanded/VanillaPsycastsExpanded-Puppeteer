using HarmonyLib;
using Verse.AI;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(MentalBreaker), "BreakMajorIsImminent", MethodType.Getter)]
public static class MentalBreaker_BreakMajorIsImminent_Patch
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