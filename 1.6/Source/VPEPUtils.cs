using RimWorld;
using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;
using VEF.Abilities;

namespace VPEPuppeteer;

public static class VPEPUtils
{
    public static bool IsPuppet(this Pawn pawn)
    {
        return IsPuppet(pawn, out _);
    }

    public static bool IsPuppet(this Pawn pawn, out Hediff_Puppet hediff_Puppet)
    {
        hediff_Puppet = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet) as Hediff_Puppet;
        return hediff_Puppet != null;
    }

    public static bool IsPuppeteer(this Pawn pawn)
    {
        return IsPuppeteer(pawn, out _);
    }

    public static bool IsPuppeteer(this Pawn pawn, out Hediff_Puppeteer hediff_Puppeteer)
    {
        hediff_Puppeteer = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
        return hediff_Puppeteer != null;
    }

    public static bool IsAliveOrTransferingMind(this Pawn pawn)
    {
        if (pawn is null)
        {
            return false;
        }
        var mindJump = pawn.GetComp<CompAbilities>().LearnedAbilities.FirstOrDefault(x => x.def == VPEP_DefOf.VPEP_MindJump) as Ability_MindJump;
        if (mindJump != null)
        {
            if (mindJump.firedMindJump.DestroyedOrNull() is false)
            {
                return true;
            }
        }
        return pawn.Dead is false && pawn.Destroyed is false;
    }

    public static void SpawnMoteAttached(this Pawn pawn, ThingDef moteDef, float scale)
    {
        if (pawn.Spawned)
        {
            MoteAttachedScaled mote = MoteMaker.MakeAttachedOverlay(pawn, moteDef, Vector3.zero) as MoteAttachedScaled;
            mote.maxScale = scale;
        }
    }
}