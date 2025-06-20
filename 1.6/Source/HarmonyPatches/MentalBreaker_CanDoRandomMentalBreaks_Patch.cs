using HarmonyLib;
using System.Collections.Generic;
using Verse.AI;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(MentalBreaker), "CanDoRandomMentalBreaks", MethodType.Getter)]
public static class MentalBreaker_CanDoRandomMentalBreaks_Patch
{
    public static Dictionary<MentalBreaker, CachedResult<bool>> cachedResults = new ();
    [HarmonyPriority(int.MinValue)]
    public static void Postfix(MentalBreaker __instance, ref bool __result)
    {
        if (__result)
        {
            if (!cachedResults.TryGetValue(__instance, out var cachedResult))
            {
                cachedResults[__instance] = cachedResult = new CachedResult<bool>();
                cachedResult.Result = __instance.pawn.IsPuppet();
            }
            else if (cachedResult.CacheExpired)
            {
                cachedResult.Result = __instance.pawn.IsPuppet();
            }

            if (cachedResult.Result)
            {
                __result = false;
            }
        }
    }
}