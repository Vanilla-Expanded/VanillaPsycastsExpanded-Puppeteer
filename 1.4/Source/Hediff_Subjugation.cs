using RimWorld;
using Verse;

namespace VPEPuppeteer
{
    public class Hediff_Subjugation : HediffWithComps
    {
        public override void PostRemoved()
        {
            base.PostRemoved();
            if (this.Severity >= 0.99f) 
            {
                if (pawn.guest != null && pawn.Faction != Faction.OfPlayer)
                {
                    if (pawn.guest.Recruitable)
                    {
                        pawn.SetFaction(Faction.OfPlayer);
                    }
                    else
                    {
                        pawn.guest.Recruitable = true;
                    }
                }
            }
        }
    }
}
