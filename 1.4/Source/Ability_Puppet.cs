﻿using RimWorld;
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

            if (target.IsPrisonerOfColony || target.IsSlaveOfColony)
            {
                target.SetFaction(Faction.OfPlayer);
            }
            target.Notify_DisabledWorkTypesChanged();
            var coma = HediffMaker.MakeHediff(VPE_DefOf.PsychicComa, pawn);
            pawn.health.AddHediff(coma);
            PawnComponentsUtility.AddAndRemoveDynamicComponents(target);
        }
    }
}
