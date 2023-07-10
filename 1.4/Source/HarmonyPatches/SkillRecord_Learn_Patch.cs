using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEPuppeteer
{
    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public static class SkillRecord_Learn_Patch
    {
        public static void Postfix(SkillRecord __instance, Pawn ___pawn, float xp, bool direct = false)
        {
            if (___pawn.IsPuppet(out var hediff_Puppet))
            {
                var skillRecord = hediff_Puppet.master.skills.GetSkill(__instance.def);
                skillRecord.Learn(xp, direct);
            }
        }
    }
}
