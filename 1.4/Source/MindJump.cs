using RimWorld;
using System.Collections.Generic;
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

        public Pawn target;

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
            var puppetToMaster = hitThing as Pawn;
            if (puppetToMaster is null)
            {
                if (target != null && target.Map == this.Map && target.Position.DistanceTo(this.Position) <= 1.5f)
                {
                    puppetToMaster = target;
                }
            }

            if (puppetToMaster != null && !puppetToMaster.Dead && !puppetToMaster.Destroyed 
                && puppetToMaster.IsPuppet(out var hediff) && hediff.master == this.launcher)
            {
                var masterToPuppet = this.launcher as Pawn;
                TransferMind(puppetToMaster, masterToPuppet);
                var pawnCompAbilities = puppetToMaster.GetComp<CompAbilities>();
                var mindJump = pawnCompAbilities.LearnedAbilities.OfType<Ability_MindJump>().FirstOrDefault();
                Rot4 rotation = ((puppetToMaster.GetPosture() != 0) ? puppetToMaster.Drawer.renderer.LayingFacing() : Rot4.North);
                var offset = puppetToMaster.Drawer.renderer.BaseHeadOffsetAt(rotation).RotatedBy(puppetToMaster.Drawer.renderer.BodyAngle());
                mindJump.AddEffecterToMaintain(SpawnEffecter(VPEP_DefOf.VPEP_PsycastSkipFlashPurple, puppetToMaster, puppetToMaster.Map, offset, 0.3f), puppetToMaster.Position, 60);
                FleckMaker.Static(masterToPuppet.Position, puppetToMaster.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
                FleckMaker.Static(puppetToMaster.Position, puppetToMaster.Map, FleckDefOf.PsycastAreaEffect, 1.5f);
            }
            base.Impact(hitThing, blockedByShield);
        }

        public static void TransferMind(Pawn puppetToMaster, Pawn masterToPuppet)
        {
            var coma = HediffMaker.MakeHediff(VPE_DefOf.PsychicComa, puppetToMaster);
            coma.TryGetComp<HediffComp_Disappears>().ticksToDisappear = (int)((GenDate.TicksPerDay * 2) / puppetToMaster.GetStatValue(StatDefOf.PsychicSensitivity));
            puppetToMaster.health.AddHediff(coma);
            var sourcePsylink = masterToPuppet.GetMainPsylinkSource();
            var sourcePsycasts = masterToPuppet.Psycasts();
            var pawnPsylink = puppetToMaster.GetMainPsylinkSource();
            var pawnPsycasts = puppetToMaster.Psycasts();
            if (pawnPsylink != null)
            {
                puppetToMaster.health.RemoveHediff(pawnPsylink);
            }
            if (pawnPsycasts != null)
            {
                puppetToMaster.health.RemoveHediff(pawnPsycasts);
            }

            masterToPuppet.health.RemoveHediff(sourcePsylink);
            masterToPuppet.health.RemoveHediff(sourcePsycasts);
            sourcePsylink.pawn = puppetToMaster;
            sourcePsycasts.pawn = puppetToMaster;
            puppetToMaster.health.hediffSet.hediffs.Add(sourcePsylink);
            puppetToMaster.health.hediffSet.hediffs.Add(sourcePsycasts);

            puppetToMaster.psychicEntropy.currentEntropy = masterToPuppet.psychicEntropy.currentEntropy;
            puppetToMaster.psychicEntropy.currentPsyfocus = masterToPuppet.psychicEntropy.currentPsyfocus;
            puppetToMaster.psychicEntropy.limitEntropyAmount = masterToPuppet.psychicEntropy.limitEntropyAmount;
            puppetToMaster.psychicEntropy.targetPsyfocus = masterToPuppet.psychicEntropy.targetPsyfocus;
            masterToPuppet.psychicEntropy = new Pawn_PsychicEntropyTracker(masterToPuppet);

            var puppeteer = masterToPuppet.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
            var puppet = puppetToMaster.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet) as Hediff_Puppet;
            Hediff_PuppetBase.preventRemoveEffects = true;
            masterToPuppet.health.RemoveHediff(puppeteer);
            puppetToMaster.health.RemoveHediff(puppet);
            Hediff_PuppetBase.preventRemoveEffects = false;

            puppet.master = puppetToMaster;
            puppet.pawn = masterToPuppet;
            puppeteer.pawn = puppetToMaster;
            if (!masterToPuppet.Dead && !masterToPuppet.Destroyed)
            {
                masterToPuppet.health.AddHediff(puppet);
            }
            puppetToMaster.health.AddHediff(puppeteer);
            var leeching = masterToPuppet.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Leeching) as Hediff_BrainLeech;
            if (leeching != null)
            {
                Hediff_BrainLeech.preventRemoveEffects = true;
                masterToPuppet.health.RemoveHediff(leeching);
                Hediff_BrainLeech.preventRemoveEffects = false;
                puppetToMaster.health.AddHediff(leeching);
                var brainLeech = leeching.otherPawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_BrainLeech) as Hediff_BrainLeech;
                brainLeech.otherPawn = puppetToMaster;
            }

            puppeteer.puppets.Remove(puppetToMaster);
            if (!masterToPuppet.Dead)
            {
                puppeteer.puppets.Add(masterToPuppet);
            }

            puppeteer.puppets.RemoveAll(x => x.Dead || x.Destroyed);

            foreach (var otherPuppet in puppeteer.puppets)
            {
                var puppetHediff = otherPuppet.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet) as Hediff_Puppet;
                puppetHediff.master = puppetToMaster;
            }

            var sourceCompAbilities = masterToPuppet.GetComp<CompAbilities>();
            var pawnCompAbilities = puppetToMaster.GetComp<CompAbilities>();
            foreach (var ability in sourceCompAbilities.LearnedAbilities.ToList())
            {
                if (ability.def.GetModExtension<AbilityExtension_Psycast>() != null)
                {
                    sourceCompAbilities.LearnedAbilities.Remove(ability);
                    pawnCompAbilities.GiveAbility(ability.def);
                }
            }

            Find.LetterStack.ReceiveLetter("VPEP.MindJumpTitle".Translate(),
                "VPEP.MindJumpText".Translate(masterToPuppet.Named("Puppeteer"), puppetToMaster.Named("NewPuppeteer")), LetterDefOf.NeutralEvent, new List<Pawn>
                {
                    masterToPuppet, puppetToMaster
                });
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref target, "target");
        }
    }
}
