using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class Ability_BrainCut : Ability
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var target = targets[0].Thing as Pawn;
            var power = this.GetPowerForPawn();
            target.stances.stunner.StunFor(120, this.pawn);
            target.TakeDamage(new DamageInfo(DamageDefOf.Cut, power, hitPart: target.health.hediffSet.GetBrain()));
            MoteMaker.MakeAttachedOverlay(target, VPEP_DefOf.VPEP_BrainCutSlash, Vector3.zero);
        }
    }
}
