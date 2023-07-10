using RimWorld;
using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;

namespace VPEPuppeteer
{
    public class Hediff_Puppet : HediffWithComps
    {
        public Pawn master;

        public override string Label => base.Label + ": " + master.LabelShort;

        public override bool ShouldRemove => master is null || master.Dead || pawn.Dead;

        public override void PostRemoved()
        {
            base.PostRemoved();
            pawn.health.AddHediff(HediffDefOf.Abasia);
            pawn.health.AddHediff(VPE_DefOf.PsychicComa);
            SpawnMoteAttached(VPEP_DefOf.VPEP_PsycastAreaEffect, 9999);
            if (master != null)
            {
                var hediff = master.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
                if (hediff != null)
                {
                    hediff.puppets.Remove(pawn);
                }
            }
        }

        public Mote SpawnMoteAttached(ThingDef moteDef, float scale)
        {
            MoteAttachedScaled mote = MoteMaker.MakeAttachedOverlay(pawn, moteDef, Vector3.zero) as MoteAttachedScaled;
            mote.maxScale = scale;
            return mote;
        }

        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            pawn.health.RemoveHediff(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref master, "master");
        }
    }
}
