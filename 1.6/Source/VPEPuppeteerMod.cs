using HarmonyLib;
using Verse;

namespace VPEPuppeteer;

public class VPEPuppeteerMod : Mod
{
    public VPEPuppeteerMod(ModContentPack pack) : base(pack)
    {
        new Harmony("VPEPuppeteerMod").PatchAll();
    }
}