using RimWorld;
using UnityEngine;
using Verse;

namespace VPEPuppeteer;

public class Hediff_BrainLeech : HediffWithComps
{
    public int lastTickCheck;
    public Pawn otherPawn;
    public bool progression;
    public bool shouldRemove;
    public int nextFleckSpawnTick;
    public bool leecher;
    private static readonly Vector3 BreathOffset = new Vector3(0f, 0f, -0.04f);
    public override bool ShouldRemove => otherPawn.IsAliveOrTransferingMind() is false || shouldRemove;
    public static bool preventRemoveEffects;

    public override string Label => base.Label + ": " + otherPawn.LabelShort;
    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);
        lastTickCheck = Find.TickManager.TicksGame;
        progression = true;
    }
    public override void PostRemoved()
    {
        base.PostRemoved();
        if (!preventRemoveEffects)
        {
            if (otherPawn != null)
            {
                var hediff = otherPawn.health.hediffSet.GetFirstHediff<Hediff_BrainLeech>();
                if (hediff != null)
                {
                    otherPawn.health.RemoveHediff(hediff);
                }
            }
        }
    }

    public override void Tick()
    {
        base.Tick();
        if (Find.TickManager.TicksGame > lastTickCheck + GenDate.TicksPerHour * 12f)
        {
            lastTickCheck = Find.TickManager.TicksGame;
            if (progression && this.CurStageIndex == this.def.stages.Count - 1)
            {
                progression = false;
            }
            if (progression)
            {
                this.Severity += 0.1f;
            }
            else
            {
                if (this.Severity > 0)
                {
                    this.Severity -= 0.1f;
                }
                else
                {
                    this.shouldRemove = true;
                }
            }
        }

        if (leecher is false && Find.TickManager.TicksGame >= nextFleckSpawnTick && pawn.Map != null)
        {
            ThrowFleck(pawn.Drawer.DrawPos + pawn.Drawer.renderer.BaseHeadOffsetAt(pawn.Rotation) + pawn.Rotation.FacingCell.ToVector3() * 0.21f + BreathOffset
                , pawn.Map, pawn.Rotation.AsAngle, pawn.Drawer.tweener.LastTickTweenedVelocity);
            nextFleckSpawnTick = Find.TickManager.TicksGame + CoughInterval.RandomInRange;
        }
    }

    public IntRange CoughInterval
    {
        get
        {
            switch (this.CurStageIndex)
            {
                case 0: return new IntRange(300, 400);
                case 1: return new IntRange(200, 300);
                case 2: return new IntRange(100, 200);
                case 3: return new IntRange(60, 120);
                default: return new IntRange(30, 60);
            }
        }
    }

    public void ThrowFleck(Vector3 loc, Map map, float throwAngle, Vector3 inheritVelocity)
    {
        if (loc.ToIntVec3().ShouldSpawnMotesAt(map))
        {
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc + new Vector3(Rand.Range(-0.005f, 0.005f),
                0f, Rand.Range(-0.005f, 0.005f)), map, FleckDefOf.AirPuff, Rand.Range(0.6f, 0.7f));
            dataStatic.rotationRate = Rand.RangeInclusive(-240, 240);
            dataStatic.velocityAngle = throwAngle + (float)Rand.Range(-10, 10);
            dataStatic.velocitySpeed = Rand.Range(0.1f, 0.8f);
            dataStatic.velocity = inheritVelocity * 0.5f;
            dataStatic.instanceColor = Color.magenta;
            dataStatic.scale = 0.9f;
            map.flecks.CreateFleck(dataStatic);
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref otherPawn, "otherPawn");
        Scribe_Values.Look(ref lastTickCheck, "lastTickCheck");
        Scribe_Values.Look(ref progression, "progression");
        Scribe_Values.Look(ref shouldRemove, "shouldRemove");
        Scribe_Values.Look(ref nextFleckSpawnTick, "nextFleckSpawnTick");
        Scribe_Values.Look(ref leecher, "leecher");
    }
}