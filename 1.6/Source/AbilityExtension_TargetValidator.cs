﻿
using RimWorld;
using Verse;
using VEF.Abilities;
using Ability = VEF.Abilities.Ability;

namespace VPEPuppeteer;

public class AbilityExtension_TargetValidator : AbilityExtension_AbilityMod
{
    public bool notOfCasterFaction;
    public bool ofCasterFaction;
    public bool allowPrisoners;
    public HediffDef requiredHediffOnTarget;
    public override bool ValidateTarget(LocalTargetInfo target, Ability ability, bool throwMessages = false)
    {
        if (target.HasThing)
        {
            if (notOfCasterFaction && target.Thing.Faction == ability.pawn.Faction)
            {
                if (throwMessages)
                {
                    Messages.Message("VPEP.TargetCannotBeSameFactionAsCaster".Translate(), MessageTypeDefOf.CautionInput);
                }
                return false;
            }
            if (ofCasterFaction)
            {
                if (target.Thing.Faction != ability.pawn.Faction && (!allowPrisoners || target.Pawn.IsPrisonerOfColony is false && target.Pawn.IsSlaveOfColony is false))
                {
                    if (throwMessages)
                    {
                        Messages.Message("VPEP.TargetMustBeSameFactionAsCaster".Translate(), MessageTypeDefOf.CautionInput);
                    }
                    return false;
                }
            }
            if (target.Pawn != null)
            {
                if (allowPrisoners && target.Pawn.IsPrisonerOfColony is false && target.Pawn.IsSlaveOfColony is false)
                {
                    if (throwMessages)
                    {
                        Messages.Message("VPEP.TargetMustBePrisonerOrSlave".Translate(), MessageTypeDefOf.CautionInput);
                    }
                    return false;
                }
            }

        }
        return base.ValidateTarget(target, ability, throwMessages);
    }
}