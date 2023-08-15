using RimWorld;
using RimWorld.Planet;
using System.Linq;
using VanillaPsycastsExpanded;
using Verse;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class Ability_Puppet : Ability
    {
        public override void Init()
        {
            base.Init();
            var masterHediff = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
            if (masterHediff is null)
            {
                masterHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_Puppeteer, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Puppeteer;
                pawn.health.AddHediff(masterHediff);
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (target.Pawn is null)
            {
                return false;
            }
            if (target.Pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet) != null)
            {
                if (showMessages)
                {
                    Messages.Message("VPEP.TargetIsPuppetAlready".Translate(), MessageTypeDefOf.CautionInput);
                }
                return false;
            }
            if (target.Pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_BrainLeech) 
                is not Hediff_BrainLeech brainLeech || brainLeech.otherPawn != this.pawn)
            {
                if (showMessages)
                {
                    Messages.Message("VPEP.TargetMustHaveBrainLeech".Translate(), MessageTypeDefOf.CautionInput);
                }
                return false;
            }
            return base.ValidateTarget(target, showMessages);
        }

        public override bool IsEnabledForPawn(out string reason)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) is Hediff_Puppeteer hediffMaster
                    && hediffMaster.puppets.Count >= hediffMaster.puppetCapacity)
            {
                reason = "VPEP.CannotHaveMorePuppets".Translate();
                return false;
            }
            return base.IsEnabledForPawn(out reason);
        }

        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var target = targets[0].Thing as Pawn;

            target.story.Adulthood = pawn.story.Adulthood;
            target.story.Childhood = pawn.story.Childhood;
            var traits = target.story.traits.allTraits.ToList();
            foreach (var t in traits)
            {
                target.story.traits.RemoveTrait(t);
            }
            foreach (var t in pawn.story.traits.allTraits)
            {
                target.story.traits.GainTrait(new Trait(t.def, t.degree));
            }
            foreach (var skill in pawn.skills.skills)
            {
                var targetSkill = target.skills.GetSkill(skill.def);
                targetSkill.xpSinceLastLevel = skill.xpSinceLastLevel;
                targetSkill.xpSinceMidnight = skill.xpSinceMidnight;
                targetSkill.Level = skill.Level;
                targetSkill.passion = skill.passion;
            }
            target.relations.ClearAllRelations();
            var masterHediff = pawn.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppeteer) as Hediff_Puppeteer;
            if (masterHediff is null)
            {
                masterHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_Puppeteer, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Puppeteer;
                pawn.health.AddHediff(masterHediff);
            }
            var puppetHediff = HediffMaker.MakeHediff(VPEP_DefOf.VPEP_Puppet, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Puppet;
            puppetHediff.master = pawn;
            masterHediff.puppets.Add(target);
            target.health.AddHediff(puppetHediff);

            if (target.Faction != pawn.Faction && pawn.Faction != null)
            {
                target.SetFaction(pawn.Faction, pawn);
            }

            if (target.ideo.ideo != null && pawn.Ideo != target.ideo.ideo)
            {
                target.ideo.SetIdeo(pawn.Ideo);
                target.ideo.Certainty = 0.5f;
            }

            target.Notify_DisabledWorkTypesChanged();
            var coma = HediffMaker.MakeHediff(VPE_DefOf.PsychicComa, pawn);
            var compDisappears = coma.TryGetComp<HediffComp_Disappears>();
            compDisappears.ticksToDisappear = (GenDate.TicksPerHour * 3);
            int puppetCountPrevTime = masterHediff.puppets.Count - 1;
            if (puppetCountPrevTime > 0)
            {
                compDisappears.ticksToDisappear += (GenDate.TicksPerHour * puppetCountPrevTime);
            }
            pawn.health.AddHediff(coma);
            PawnComponentsUtility.AddAndRemoveDynamicComponents(target);
        }
    }
}
