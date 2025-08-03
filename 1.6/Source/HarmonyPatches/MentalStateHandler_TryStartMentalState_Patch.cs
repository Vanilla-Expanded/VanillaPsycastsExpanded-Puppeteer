using HarmonyLib;
using Verse;
using Verse.AI;

namespace VPEPuppeteer;

[HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
public static class MentalStateHandler_TryStartMentalState_Patch
{
    public static bool shouldStartMentalState;

    [HarmonyPriority(int.MaxValue)]
    public static bool Prefix(Pawn ___pawn)
    {
        return shouldStartMentalState || !___pawn.IsPuppet();
    }

    public static void Postfix(Pawn ___pawn, MentalStateDef stateDef, string reason, bool forced, bool forceWake, bool causedByMood, Pawn otherPawn, bool transitionSilently, bool causedByDamage, bool causedByPsycast, bool __result)
    {
        if (__result && ___pawn.IsPuppeteer(out var hediff))
        {
            try
            {
                shouldStartMentalState = true;
                foreach (var puppet in hediff.puppets)
                {
                    puppet.mindState.mentalStateHandler.TryStartMentalState(stateDef, reason, forced, forceWake, causedByMood, otherPawn, transitionSilently, causedByDamage, causedByPsycast);
                }
            }
            finally
            {
                shouldStartMentalState = false;
            }
        }
    }
}