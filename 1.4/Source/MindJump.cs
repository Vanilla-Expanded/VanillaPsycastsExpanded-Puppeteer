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

        public override Quaternion ExactRotation => Quaternion.LookRotation(this.LookTowards);

        public override int DamageAmount => 0;

        public override void Draw()
        {
            float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction);
            Vector3 drawPos = DrawPos;
            Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
            Graphics.DrawMesh(MeshPool.GridPlane(DrawSize), position, ExactRotation, DrawMat, 0);
            Comps_PostDraw();
        }

        public override Vector2 DrawSize
        {
            get
            {
                var drawSize = base.DrawSize;
                if (DistanceCoveredFraction > 0.5f)
                {
                    drawSize *= 1f - (DistanceCoveredFraction);
                }
                else
                {
                    drawSize *= DistanceCoveredFraction;
                }
                drawSize *= 2f;
                return drawSize;
            }
        }

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

                source.health.RemoveHediff(sourcePsylink);
                source.health.RemoveHediff(sourcePsycasts);
                sourcePsylink.pawn = pawn;
                sourcePsycasts.pawn = pawn;
                pawn.health.hediffSet.hediffs.Add(sourcePsylink);
                pawn.health.hediffSet.hediffs.Add(sourcePsycasts);

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

                var mindJump = pawnCompAbilities.LearnedAbilities.OfType<Ability_MindJump>().FirstOrDefault();
                Rot4 rotation = ((pawn.GetPosture() != 0) ? pawn.Drawer.renderer.LayingFacing() : Rot4.North);
                var offset = pawn.Drawer.renderer.BaseHeadOffsetAt(rotation).RotatedBy(pawn.Drawer.renderer.BodyAngle());
                mindJump.AddEffecterToMaintain(SpawnEffecter(VPEP_DefOf.VPEP_PsycastSkipFlashPurple, pawn, pawn.Map, offset, 0.3f), pawn.Position, 60);
                FleckMaker.Static(source.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
                FleckMaker.Static(pawn.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 1.5f);

            }
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
