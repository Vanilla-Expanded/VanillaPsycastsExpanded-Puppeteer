

using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;

namespace VPEPuppeteer
{
    public class Ability_SummonPuppet : Ability_PuppetCastBase
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var target = targets[0].Thing as Pawn;
            this.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(target.Position, target.Map, 0.72f), target.Position, 60);
            this.AddEffecterToMaintain(VPEP_DefOf.VPEP_PsycastSkipFlashPurple.Spawn(this.pawn.Position, this.pawn.Map, 0.72f), this.pawn.Position, 60);
            target.DeSpawn();
            GenPlace.TryPlaceThing(target, pawn.Position, pawn.Map, ThingPlaceMode.Near);
        }
    }
}
