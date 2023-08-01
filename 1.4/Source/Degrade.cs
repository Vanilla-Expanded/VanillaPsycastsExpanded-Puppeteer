using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;
using Verse.Sound;

namespace VPEPuppeteer
{
    public class Degrade : Bullet
    {
        public MoteAttachedScaled warpMote;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            warpMote = MoteMaker.MakeAttachedOverlay(this, VPEP_DefOf.VPEP_DegradePsychicWarp, Vector3.zero) as MoteAttachedScaled;
            warpMote.maxScale = 1.5f;
            warpMote.exactScale = new Vector3(warpMote.maxScale, 0, warpMote.maxScale);
        }

        public override void Tick()
        {
            base.Tick();
            warpMote?.Maintain();
        }

        public override int DamageAmount => 0;

        public override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (hitThing is Pawn pawn)
            {
                if (pawn.needs?.mood != null)
                {
                    pawn.equipment.DropAllEquipment(pawn.Position);
                    VPEP_DefOf.RunWild.Worker.TryStart(pawn, "VPEP.DegradePsycast".Translate(), false);
                    var firstApparel = pawn.apparel.WornApparel.FirstOrDefault();
                    if (firstApparel != null)
                    {
                        pawn.jobs.StartJob(JobMaker.MakeJob(VPEP_DefOf.VPEP_StripGear, firstApparel));
                    }
                }
            }
            FleckMaker.Static(Position, Map, FleckDefOf.PsycastAreaEffect, 2);
            base.Impact(hitThing, blockedByShield);
        }
    }
}
