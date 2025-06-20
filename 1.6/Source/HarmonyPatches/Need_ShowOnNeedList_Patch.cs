using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(Need), "ShowOnNeedList", MethodType.Getter)]
public static class Need_ShowOnNeedList_Patch
{
    [HarmonyPriority(int.MaxValue)]
    public static bool Prefix(Need __instance, Pawn ___pawn)
    {
        if (__instance is Need_Mood && ___pawn.IsPuppet())
        {
            return false;
        }
        return true;
    }
}