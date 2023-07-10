using System.Collections.Generic;
using Verse;

namespace VPEPuppeteer
{
    public class Hediff_Puppeteer : HediffWithComps
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
