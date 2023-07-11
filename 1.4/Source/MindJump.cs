using RimWorld;
using System.Linq;
using UnityEngine;
using VanillaPsycastsExpanded;
using VanillaPsycastsExpanded.Graphics;
using Verse;
using VFECore.Abilities;

namespace VPEPuppeteer
{
    public class MindJump : Bullet
    {
        private Vector3 LookTowards =>
    new(this.destination.x - this.origin.x, this.def.Altitude, this.destination.z - this.origin.z +
                                                               this.ArcHeightFactor * (4 - 8 * this.DistanceCoveredFraction));

        private new float ArcHeightFactor
        {
            get
            {
                float num = this.def.projectile.arcHeightFactor;
                float num2 = (this.destination - this.origin).MagnitudeHorizontalSquared();
                if (num * num > num2 * 0.2f * 0.2f) num = Mathf.Sqrt(num2) * 0.2f;

                return num;
            }
        }

        public override Quaternion ExactRotation => Quaternion.LookRotation(this.LookTowards);

        public override int DamageAmount => 0;

        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
            if (hitThing is Pawn pawn && pawn.IsPuppet(out var hediff) && hediff.master == this.launcher)
            {
                var coma = HediffMaker.MakeHediff(VPE_DefOf.PsychicComa, pawn);
                coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = (int)((GenDate.TicksPerDay * 2) / pawn.GetStatValue(StatDefOf.PsychicSensitivity));
                pawn.health.AddHediff(coma);

                var source = this.launcher as Pawn;
                var sourcePsylink = source.GetMainPsylinkSource();
                var sourcePsycasts = source.Psycasts();
                var pawnPsylink = pawn.GetMainPsylinkSource();
                var pawnPsycasts = pawn.Psycasts();
                if (pawnPsylink != null)
                {
                    pawn.health.RemoveHediff(pawnPsylink);
                }
                if (pawnPsycasts != null)
                {
                    pawn.health.RemoveHediff(pawnPsycasts);
                }

                Log.Message("1: " + sourcePsycasts.GetHashCode() + " - " + string.Join(", ", sourcePsycasts.unlockedPaths));
                source.health.RemoveHediff(sourcePsylink);
                source.health.RemoveHediff(sourcePsycasts);
                Log.Message("2: " + sourcePsycasts.GetHashCode() + " - " + string.Join(", ", sourcePsycasts.unlockedPaths));
                sourcePsylink.pawn = pawn;
                sourcePsycasts.pawn = pawn;
                pawn.health.hediffSet.hediffs.Add(sourcePsylink);
                pawn.health.hediffSet.hediffs.Add(sourcePsycasts);
                Log.Message("3: " + sourcePsycasts.GetHashCode() + " - " + string.Join(", ", sourcePsycasts.unlockedPaths));
                pawnPsycasts = pawn.Psycasts();
                Log.Message("4: " + pawnPsycasts.GetHashCode() + " - " + string.Join(", ", pawnPsycasts.unlockedPaths));

                pawn.psychicEntropy.currentEntropy = source.psychicEntropy.currentEntropy;
                pawn.psychicEntropy.currentPsyfocus = source.psychicEntropy.currentPsyfocus;
                pawn.psychicEntropy.limitEntropyAmount = source.psychicEntropy.limitEntropyAmount;
                pawn.psychicEntropy.targetPsyfocus = source.psychicEntropy.targetPsyfocus;
                source.psychicEntropy = new Pawn_PsychicEntropyTracker(source);

                var puppeteer = source.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
                var puppet = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet) as Hediff_Puppet;

                Hediff_PuppetBase.preventRemoveEffects = true;
                source.health.RemoveHediff(puppeteer);
                pawn.health.RemoveHediff(puppet);
                Hediff_PuppetBase.preventRemoveEffects = false;

                puppet.master = pawn;
                puppet.pawn = source;
                puppeteer.pawn = pawn;
                source.health.AddHediff(puppet);
                pawn.health.AddHediff(puppeteer);

                var sourceCompAbilities = source.GetComp<CompAbilities>();
                var pawnCompAbilities = pawn.GetComp<CompAbilities>();

                foreach (var ability in sourceCompAbilities.LearnedAbilities.ToList())
                {
                    if (ability.def.GetModExtension<AbilityExtension_Psycast>() != null)
                    {
                        sourceCompAbilities.LearnedAbilities.Remove(ability);
                        pawnCompAbilities.GiveAbility(ability.def);
                    }
                }
            }
        }
    }
}
