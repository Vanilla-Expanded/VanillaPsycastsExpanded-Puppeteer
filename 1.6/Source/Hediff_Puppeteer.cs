using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using VanillaPsycastsExpanded;
using Verse;
using Verse.Sound;
using VEF.Abilities;

namespace VPEPuppeteer;

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
                if (pawn.MapHeld != null)
                {
                    var availablePuppet = puppets.Where(x => x.Map == pawn.MapHeld && mindJump.CanHitTarget(x))
                        .OrderBy(x => x.Position.DistanceTo(pawn.PositionHeld)).FirstOrDefault();
                    if (availablePuppet != null)
                    {
                        mindJump.LaunchProjectile(new GlobalTargetInfo(availablePuppet));
                        return true;
                    }
                }
                else
                {
                    var parentHolder = pawn.ParentHolder;
                    if (parentHolder != null)
                    {
                        var thingOwner = parentHolder.GetDirectlyHeldThings();
                        if (thingOwner != null)
                        {
                            if (thingOwner.OfType<Pawn>().Where(x => puppets.Contains(x))
                                .TryRandomElement(out var puppet))
                            {
                                MindJump.TransferMind(puppet, pawn);
                                return true;
                            }
                        }
                    }
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

    public void AddComa()
    {
        var coma = HediffMaker.MakeHediff(VPE_DefOf.PsychicComa, pawn);
        var compDisappears = coma.TryGetComp<HediffComp_Disappears>();
        compDisappears.ticksToDisappear = GetComaDuration();
        pawn.health.AddHediff(coma);
    }

    public int GetComaDuration()
    {
        var comaDuration = (GenDate.TicksPerHour * 3);
        int puppetCountPrevTime = this.puppets.Count - 1;
        if (puppetCountPrevTime > 0)
        {
            comaDuration += (GenDate.TicksPerHour * puppetCountPrevTime);
        }
        return comaDuration;
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