using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(NeedsCardUtility), "DoMoodAndThoughts")]
public static class NeedsCardUtility_DoMoodAndThoughts_Patch
{
    [HarmonyPriority(int.MaxValue)]
    public static bool Prefix(Pawn pawn)
    {
        if (pawn.IsPuppet())
        {
            return false;
        }
        return true;
    }
}