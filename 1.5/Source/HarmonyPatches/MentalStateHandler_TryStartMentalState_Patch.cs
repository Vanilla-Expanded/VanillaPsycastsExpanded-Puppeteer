using HarmonyLib;
using Verse;
using Verse.AI;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    public static class MentalStateHandler_TryStartMentalState_Patch
    {
        public static bool shouldStartMentalState;

        [HarmonyPriority(int.MaxValue)]
        public static bool Prefix(MentalStateHandler __instance, Pawn ___pawn, MentalStateDef stateDef, string reason = null, bool forced = false, bool forceWake = false, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false, bool causedByDamage = false, bool causedByPsycast = false)
        {
            if (___pawn.IsPuppeteer(out var hediff))
            {
                shouldStartMentalState = true;
                foreach (var puppet in hediff.puppets)
                {
                    puppet.mindState.mentalStateHandler.TryStartMentalState(stateDef, reason, forced, forceWake, causedByMood, otherPawn, transitionSilently, causedByDamage, causedByPsycast);
                }
                shouldStartMentalState = false;
            }
            if (!shouldStartMentalState && ___pawn.IsPuppet())
            {
                return false;
            }
            return true;
        }
    }
}
