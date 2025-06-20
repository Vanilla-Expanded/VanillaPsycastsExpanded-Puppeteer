using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(InspectPaneFiller), "DrawMood")]
public static class InspectPaneFiller_DrawMood_Patch
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