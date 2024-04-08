using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace VPEPuppeteer
{
    public class JobDriver_StripGear : JobDriver
    {
        private int duration;

        private Apparel Apparel => (Apparel)job.GetTarget(TargetIndex.A).Thing;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref duration, "duration", 0);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            duration = (int)(Apparel.GetStatValue(StatDefOf.EquipDelay) * 60f);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            var waitToil = Toils_General.Wait(duration).WithProgressBarToilDelay(TargetIndex.A);
            var stripApparel = Toils_General.Do(delegate
            {
                if (pawn.apparel.WornApparel.Contains(Apparel))
                {
                    pawn.apparel.TryDrop(Apparel, out var resultingAp);
                }
                var firstApparel = pawn.apparel.WornApparel.FirstOrDefault();
                if (firstApparel != null)
                {
                    job.SetTarget(TargetIndex.A, firstApparel);
                    duration = (int)(Apparel.GetStatValue(StatDefOf.EquipDelay) * 60f);
                }
                else
                {
                    job.SetTarget(TargetIndex.A, null);
                }
            });
            yield return waitToil;
            yield return stripApparel;
            yield return Toils_Jump.JumpIf(waitToil, () => Apparel != null);
        }
    }
}
