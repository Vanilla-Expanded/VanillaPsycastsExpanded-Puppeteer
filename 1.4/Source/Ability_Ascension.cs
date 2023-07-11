using RimWorld;
using RimWorld.Planet;
using System.Linq;
using UnityEngine;
using VanillaPsycastsExpanded;
using Verse;

namespace VPEPuppeteer
{
    public class Ability_Ascension : Ability_PuppetCastBase
    {
        public override void Cast(params GlobalTargetInfo[] targets)
        {
            base.Cast(targets);
            var target = targets[0].Thing as Pawn;

            var totalXP = target.skills.skills.Sum(x => x.XpTotalEarned);
            pawn.Psycasts().GainExperience(totalXP / 100f);
            var cells = GenRadial.RadialCellsAround(target.Position, 5f, true).ToList();
            foreach (var cell in cells)
            {
                var roof = cell.GetRoof(pawn.Map);
                if (roof != null)
                {
                    pawn.Map.roofGrid.SetRoof(cell, null);
                }
                var terrain = cell.GetTerrain(pawn.Map);
                if (terrain != null && pawn.Map.terrainGrid.CanRemoveTopLayerAt(cell))
                {
                    pawn.Map.terrainGrid.Notify_TerrainDestroyed(cell);
                }
            }
            var pos = target.DrawPos;
            var things = GenRadial.RadialDistinctThingsAround(target.Position, target.Map, 5f, true).ToList();
            foreach (var thing in things)
            {
                thing.Destroy();
            }

            FleckMaker.Static(target.Position, pawn.Map, FleckDefOf.PsycastAreaEffect, 5);
            this.AddEffecterToMaintain(SpawnEffecter(VPEP_DefOf.VPEP_PsycastSkipFlashPurple, 
                new TargetInfo(target.Position, target.Map), Vector3.zero, 5f), target.Position, 60);
        }

        public Effecter SpawnEffecter(EffecterDef effecterDef, TargetInfo targetInfo, Vector3 offset, float scale)
        {
            Effecter effecter = new Effecter(effecterDef);
            effecter.offset = offset;
            effecter.scale = scale;
            effecter.Trigger(targetInfo, targetInfo);
            return effecter;
        }
    }
}
