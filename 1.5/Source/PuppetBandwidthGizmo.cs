using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VPEPuppeteer
{
    [StaticConstructorOnStartup]
    public class PuppetBandwidthGizmo : Gizmo
    {
        public const int InRectPadding = 6;

        private static readonly Color EmptyBlockColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        private static readonly Color FilledBlockColor = Color.magenta;

        private static readonly Color ExcessBlockColor = ColorLibrary.Red;

        private Hediff_Puppeteer hediff;

        public override bool Visible => Find.Selector.SelectedPawns.Count == 1;

        public PuppetBandwidthGizmo(Hediff_Puppeteer hediff)
        {
            this.hediff = hediff;
            Order = -90f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            int totalBandwidth = hediff.puppetCapacity;
            int usedBandwidth = hediff.puppets.Count;
            string text = usedBandwidth.ToString("F0") + " / " + totalBandwidth.ToString("F0");
            TaggedString taggedString = "VPEP.Puppets".Translate().Colorize(ColoredText.TipSectionTitleColor) + ": " + text + "\n\n" 
                + "VPEP.PuppetsGizmoTip".Translate(hediff.GetComaDuration().ToStringTicksToPeriod(), totalBandwidth);
            int usedBandwidthFromSubjects = hediff.puppets.Count;
            if (usedBandwidthFromSubjects > 0)
            {
                IEnumerable<string> entries = from p in hediff.puppets select p.LabelCap;
                taggedString += "\n\n" + "VPEP.PuppetsUsage".Translate() + "\n" + entries.ToLineList(" - ");
            }

            TooltipHandler.TipRegion(rect, taggedString);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, 20f);
            Widgets.Label(rect3, "VPEP.Puppets".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperRight;
            Widgets.Label(rect3, text);
            Text.Anchor = TextAnchor.UpperLeft;
            int num = Mathf.Max(usedBandwidth, totalBandwidth);
            Rect rect4 = new Rect(rect2.x, rect3.yMax + 6f, rect2.width, rect2.height - rect3.height - 6f);
            int num2 = 2;
            int num3 = Mathf.FloorToInt(rect4.height / (float)num2);
            int num4 = Mathf.FloorToInt(rect4.width / (float)num3);
            int num5 = 0;
            while (num2 * num4 < num)
            {
                num2++;
                num3 = Mathf.FloorToInt(rect4.height / (float)num2);
                num4 = Mathf.FloorToInt(rect4.width / (float)num3);
                num5++;
                if (num5 >= 1000)
                {
                    Log.Error("Failed to fit bandwidth cells into gizmo rect.");
                    return new GizmoResult(GizmoState.Clear);
                }
            }
            int num6 = Mathf.FloorToInt(rect4.width / (float)num3);
            int num7 = num2;
            float num8 = (rect4.width - (float)(num6 * num3)) / 2f;
            int num9 = 0;
            int num10 = ((num7 <= 2) ? 4 : 2);
            for (int i = 0; i < num7; i++)
            {
                for (int j = 0; j < num6; j++)
                {
                    num9++;
                    Rect rect5 = new Rect(rect4.x + (float)(j * num3) + num8, rect4.y + (float)(i * num3), num3, num3).ContractedBy(2f);
                    if (num9 <= num)
                    {
                        if (num9 <= usedBandwidth)
                        {
                            Widgets.DrawRectFast(rect5, (num9 <= totalBandwidth) ? FilledBlockColor : ExcessBlockColor);
                        }
                        else
                        {
                            Widgets.DrawRectFast(rect5, EmptyBlockColor);
                        }
                    }
                }
            }
            return new GizmoResult(GizmoState.Clear);
        }

        public override float GetWidth(float maxWidth)
        {
            return 136f;
        }
    }
}
