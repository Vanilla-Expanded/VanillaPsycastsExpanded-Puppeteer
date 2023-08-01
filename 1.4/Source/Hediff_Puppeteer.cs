using System.Collections.Generic;
using Verse;
using Verse.Sound;

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
