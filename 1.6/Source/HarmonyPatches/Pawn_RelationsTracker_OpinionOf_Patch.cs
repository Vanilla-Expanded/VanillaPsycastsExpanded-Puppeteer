using HarmonyLib;
using RimWorld;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(Pawn_RelationsTracker), "OpinionOf")]
public static class Pawn_RelationsTracker_OpinionOf_Patch
{
    [HarmonyPriority(int.MaxValue)]
    public static bool Prefix(Pawn_RelationsTracker __instance)
    {
        if (__instance.pawn.IsPuppet())
        {
            return false;
        }
        return true;
    }
}