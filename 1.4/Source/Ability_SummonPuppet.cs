

using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class Ability_SummonPuppet : Ability
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
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            var targetPawn = target.Pawn;
            if (targetPawn != null && (targetPawn.IsPuppet(out var puppetHediff) is false || puppetHediff.master != this.pawn))
            {
                if (showMessages)
                {
                    if (showMessages)
                    {
                        Messages.Message("VPEP.TargetMustBePuppetOfCaster".Translate(), MessageTypeDefOf.CautionInput);
                    }
                }
                return false;
            }
            return base.ValidateTarget(target, showMessages);
        }

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
