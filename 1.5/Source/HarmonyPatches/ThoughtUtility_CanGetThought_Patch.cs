using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(ThoughtUtility), "CanGetThought")]
    public static class ThoughtUtility_CanGetThought_Patch
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
}
