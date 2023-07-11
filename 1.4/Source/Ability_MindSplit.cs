using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class Ability_MindSplit : Ability
    {
        public override void Init()
        {
            base.Init();
            var masterHediff = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
            if (masterHediff is null)
            {
                masterHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_Puppeteer, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Puppeteer;
                pawn.health.AddHediff(masterHediff);
            }
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var masterHediff = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
            if (masterHediff is null)
            {
                masterHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_Puppeteer, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Puppeteer;
                pawn.health.AddHediff(masterHediff);
            }
            var coma = pawn.health.AddHediff(VPE_DefOf.PsychicComa);
            coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = GenDate.TicksPerDay;

            MoteAttachedScaled mote = MoteMaker.MakeAttachedOverlay(pawn, VPEP_DefOf.VPEP_PsycastAreaEffect, Vector3.zero) as MoteAttachedScaled;
            mote.maxScale = 9999;
            foreach (var puppet in masterHediff.puppets)
            {
                coma = puppet.health.AddHediff(VPE_DefOf.PsychicComa);
                coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = GenDate.TicksPerDay;
                if (puppet.Spawned)
                {
                    mote = MoteMaker.MakeAttachedOverlay(puppet, VPEP_DefOf.VPEP_PsycastAreaEffect, Vector3.zero) as MoteAttachedScaled;
                    mote.maxScale = 9999;
                }
            }
            masterHediff.puppetCapacity += 1;
        }
    }
}
