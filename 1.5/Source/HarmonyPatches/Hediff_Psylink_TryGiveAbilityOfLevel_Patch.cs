using HarmonyLib;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(Hediff_Psylink), "TryGiveAbilityOfLevel")]
    public static class Hediff_Psylink_TryGiveAbilityOfLevel_Patch
    {
        public static bool suppress;
        public static bool Prefix()
        {
            if (suppress)
                return false;
            return true;
        }
    }
}
