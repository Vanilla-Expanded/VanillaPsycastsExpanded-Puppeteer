using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;

namespace VPEPuppeteer
{
    public class Ability_MindJump : Ability_ShootProjectile
    {
        public MindJump firedMindJump;
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

        protected override Projectile ShootProjectile(GlobalTargetInfo target)
        {
            firedMindJump = base.ShootProjectile(target) as MindJump;
            firedMindJump.target = target.Thing as Pawn;
            return firedMindJump;
        }

        public void LaunchProjectile(GlobalTargetInfo target)
        {
            ShootProjectile(target);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref firedMindJump, "firedMindJump");
        }
    }
}
