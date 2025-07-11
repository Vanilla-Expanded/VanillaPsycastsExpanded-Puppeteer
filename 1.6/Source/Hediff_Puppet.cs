﻿using RimWorld;
using VanillaPsycastsExpanded;
using Verse;

namespace VPEPuppeteer;

public class Hediff_Puppet : Hediff_PuppetBase
{
    public Pawn master;

    public override string Label => base.Label + ": " + master.LabelShort;

    public override bool ShouldRemove => 
        master.IsAliveOrTransferingMind() is false 
        || pawn.IsAliveOrTransferingMind() is false;

    public bool pawnKilled;
    public override void Notify_PawnKilled()
    {
        base.Notify_PawnKilled();
        if (pawn.health.hediffSet.hediffs.Contains(this))
        {
            pawnKilled = true;
            pawn.health.RemoveHediff(this);
            pawnKilled = false;
        }
    }

    public override void PostRemoved()
    {
        base.PostRemoved();
        if (!preventRemoveEffects)
        {
            if (!pawn.Dead && !pawnKilled)
            {
                Log.Message("Adding abasia");
                pawn.health.AddHediff(HediffDefOf.Abasia);
                var coma = pawn.health.AddHediff(VPE_DefOf.PsychicComa);
                coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = GenDate.TicksPerDay;
            }
            pawn.SpawnMoteAttached(VPEP_DefOf.VPEP_PsycastAreaEffect, 9999);
            if (master != null)
            {
                var hediff = master.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
                if (hediff != null)
                {
                    hediff.puppets.Remove(pawn);
                }
            }
            pawn.needs.AddOrRemoveNeedsAsAppropriate();
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref master, "master");
    }
}