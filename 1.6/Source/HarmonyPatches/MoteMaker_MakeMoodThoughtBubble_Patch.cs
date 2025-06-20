using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(MoteMaker), "MakeMoodThoughtBubble")]
public static class MoteMaker_MakeMoodThoughtBubble_Patch
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