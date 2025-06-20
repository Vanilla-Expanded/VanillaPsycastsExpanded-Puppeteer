using HarmonyLib;
using RimWorld;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(SkillRecord), "LearnRateFactor")]
public static class SkillRecord_LearnRateFactor_Patch
{
    public static void Postfix(SkillRecord __instance, ref float __result)
    {
        if (__instance.pawn.IsPuppet())
        {
            __result *= 0.2f;
        }
    }
}