using RimWorld;
using RimWorld.Planet;
using VanillaPsycastsExpanded;
using Verse;
using Verse.Sound;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class Ability_PuppetSwarm : Ability
    {
        public override bool IsEnabledForPawn(out string reason)
        {
            if (!pawn.IsPuppeteer(out var hediff) || hediff.puppets.Count == 0)
            {
                reason = "VPEP.NoPuppets".Translate();
                return false;
            }
            return base.IsEnabledForPawn(out reason);
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
            foreach (var puppet in hediff.puppets)
            {
                if (puppet.Spawned)
                {
                    this.AddEffecterToMaintain(EffecterDefOf.Skip_Entry.Spawn(puppet.Position, puppet.Map, 0.72f), puppet.Position, 60);
                }
                this.AddEffecterToMaintain(VPEP_DefOf.VPEP_PsycastSkipFlashPurple.Spawn(this.pawn.Position, this.pawn.Map, 0.72f), this.pawn.Position, 60);
                if (puppet.Spawned)
                {
                    puppet.DeSpawn();
                }
                SoundDefOf.Psycast_Skip_Entry.PlayOneShot(new TargetInfo(puppet.Position, pawn.Map));
                GenPlace.TryPlaceThing(puppet, pawn.Position, pawn.Map, ThingPlaceMode.Near);
            }
            SoundDefOf.Psycast_Skip_Exit.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
        }
    }
}
