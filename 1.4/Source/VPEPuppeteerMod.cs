﻿using HarmonyLib;
using System.Linq;
using UnityEngine;
using Verse;

namespace VPEPuppeteer
{
    public class VPEPuppeteerMod : Mod
    {
        public VPEPuppeteerMod(ModContentPack pack) : base(pack)
        {
			new Harmony("VPEPuppeteerMod").PatchAll();
            Log.Message("Color.magenta: " + Color.magenta);
        }
    }
}
