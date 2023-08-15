using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;
using VFECore.Abilities;

namespace VPEPuppeteer
{
    public class Hediff_Puppeteer : Hediff_PuppetBase
    {
        public List<Pawn> puppets = new List<Pawn>();
        public int puppetCapacity = 1;
        public PuppetBandwidthGizmo bandwidthGizmo;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref puppets, "puppets", LookMode.Reference);
            Scribe_Values.Look(ref puppetCapacity, "puppetCapacity", 1);
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            if (!preventRemoveEffects)
            {
                if (pawn.IsAliveOrTransferingMind() is false)
                {
                    ClearPuppets();
                }
            }
        }

        public bool TryTransferMind()
        {
            var mindJump = pawn.GetComp<CompAbilities>().LearnedAbilities
                .FirstOrDefault(x => x.def == VPEP_DefOf.VPEP_MindJump) as Ability_MindJump;
            if (mindJump != null)
            {
                if (mindJump.firedMindJump.DestroyedOrNull())
                {
                    var availablePuppet = puppets.Where(x => x.Map == pawn.MapHeld
                        && x.Position.DistanceTo(pawn.Position) <= mindJump.GetRangeForPawn())
                            .OrderBy(x => x.Position.DistanceTo(pawn.Position)).FirstOrDefault();
                    if (availablePuppet != null)
                    {
                        mindJump.LaunchProjectile(new GlobalTargetInfo(availablePuppet));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private void ClearPuppets()
        {
            pawn.SpawnMoteAttached(VPEP_DefOf.VPEP_PsycastAreaEffect, 9999);
            foreach (var puppet in puppets)
            {
                var hediff = puppet.health.hediffSet.GetFirstHediffOfDef(VPEP_DefOf.VPEP_Puppet);
                if (hediff != null)
                {
                    puppet.health.RemoveHediff(hediff);
                }
            }
            VPEP_DefOf.VPEP_Puppet_Master_Death.PlayOneShot(pawn);
            puppets.Clear();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (this.pawn.IsColonistPlayerControlled)
            {
                if (bandwidthGizmo == null)
                {
                    bandwidthGizmo = new PuppetBandwidthGizmo(this);
                }
                yield return bandwidthGizmo;
            }
        }
    }
}
