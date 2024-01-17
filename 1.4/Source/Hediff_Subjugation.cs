using RimWorld;
using Verse;

namespace VPEPuppeteer
{
    public class Hediff_Subjugation : HediffWithComps
    {
        public override void PostRemoved()
        {
            base.PostRemoved();
            if (this.Severity >= 0.95f)
            {
                if (pawn.guest != null && pawn.Faction != Faction.OfPlayer)
                {
                    if (pawn.guest.Recruitable)
                    {
                        pawn.SetFaction(Faction.OfPlayer);
                        pawn.guest.SetGuestStatus(null);
                        pawn.guest.Released = true;
                    }
                    else
                    {
                        pawn.guest.Recruitable = true;
                    }
                }
                else if (pawn.IsPrisoner)
                {
                    pawn.guest.SetGuestStatus(null);
                    pawn.guest.Released = true;
                }
            }
        }
    }
}
