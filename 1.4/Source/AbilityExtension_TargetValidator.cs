
using RimWorld;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace VPEPuppeteer
{
    public class AbilityExtension_TargetValidator : AbilityExtension_AbilityMod
    {
        public bool notOfCasterFaction;
        public bool ofCasterFaction;
        public override bool ValidateTarget(LocalTargetInfo target, Ability ability, bool throwMessages = false)
        {
            if (notOfCasterFaction && target.Thing.Faction == ability.pawn.Faction)
            {
                if (throwMessages)
                {
                    Messages.Message("VPEP.TargetCannotBeSameFactionAsCaster".Translate(), MessageTypeDefOf.CautionInput);
                }
                return false;
            }
            if (ofCasterFaction && target.Thing.Faction != ability.pawn.Faction)
            {
                if (throwMessages)
                {
                    Messages.Message("VPEP.TargetMustBeSameFactionAsCaster".Translate(), MessageTypeDefOf.CautionInput);
                }
                return false;
            }
            return base.ValidateTarget(target, ability, throwMessages);
        }
    }
}
