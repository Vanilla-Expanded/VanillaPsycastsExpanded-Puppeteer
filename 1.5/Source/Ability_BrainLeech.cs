using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class Ability_BrainLeech : Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var target = targets[0].Thing as Pawn;
            var leechingHediff = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Leeching) as Hediff_BrainLeech;
            if (leechingHediff != null)
            {
                pawn.health.RemoveHediff(leechingHediff);
            }
            var brainLeechHediff = target.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_BrainLeech) as Hediff_BrainLeech;
            if (brainLeechHediff != null)
            {
                target.health.RemoveHediff(brainLeechHediff);
            }
            leechingHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_Leeching, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_BrainLeech;
            leechingHediff.otherPawn = target;
            leechingHediff.leecher = true;
            brainLeechHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_BrainLeech, target, target.health.hediffSet.GetBrain()) as Hediff_BrainLeech;
            brainLeechHediff.otherPawn = pawn;
            pawn.health.AddHediff(leechingHediff, pawn.health.hediffSet.GetBrain());
            target.health.AddHediff(brainLeechHediff, target.health.hediffSet.GetBrain());
            Rot4 rotation = ((target.GetPosture() != 0) ? pawn.Drawer.renderer.LayingFacing() : Rot4.North);
            var offset = pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).RotatedBy(pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
            this.AddEffecterToMaintain(SpawnEffecter(VPEP_DefOf.VPEP_PsycastSkipFlashPurple, target, this.pawn.Map, offset, 0.3f), target.Position, 60);
        }

        public Effecter SpawnEffecter(EffecterDef effecterDef, Thing target, Map map, Vector3 offset, float scale)
        {
            Effecter effecter = new Effecter(effecterDef);
            effecter.offset = offset;
            effecter.scale = scale;
            TargetInfo targetInfo = new TargetInfo(target.Position, map);
            effecter.Trigger(targetInfo, targetInfo);
            return effecter;
        }
    }
}
