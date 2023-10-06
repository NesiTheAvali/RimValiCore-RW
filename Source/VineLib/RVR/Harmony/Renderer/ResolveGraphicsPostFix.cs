﻿using RimWorld;
using RVCRestructured.Graphics;
using UnityEngine;
using Verse;

namespace RVCRestructured.RVR.HarmonyPatches
{
    public static class ResolveGraphicsPostFix
    {
        public static void ResolveGraphicsPatch(PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!(pawn.def is RaceDef rDef))
                return;

            RVRComp comp = pawn.TryGetComp<RVRComp>();
            comp.GenGraphics();
            Color skinOne = pawn.story.SkinColor;
            Color skinTwo = pawn.story.SkinColor;
            Color skinThree = pawn.story.SkinColor;
            if (rDef.RaceGraphics.skinColorSet != null)
            {
                string set = rDef.RaceGraphics.skinColorSet;
                skinOne = comp[set][0];
                skinTwo = comp[set][1];
                skinThree = comp[set][2];
            }
            __instance.geneGraphics = new System.Collections.Generic.List<GeneGraphicRecord>();
            __instance.bodyTattooGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.faceTattooGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.furCoveredGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");

            __instance.desiccatedHeadGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.dessicatedGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            __instance.desiccatedHeadStumpGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");

            __instance.headStumpGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");

            __instance.nakedGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(rDef.RaceGraphics.bodyTex, rDef.RaceGraphics.bodySize, skinOne, skinTwo, skinThree);
            if (!rDef.RaceGraphics.hasHair)
            {
                __instance.beardGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
                __instance.hairGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>("RVC/Empty");
            }
            __instance.headGraphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(rDef.RaceGraphics.headTex, rDef.RaceGraphics.headSize, skinOne, skinTwo, skinThree);
            __instance.SetApparelGraphicsDirty();
            PortraitsCache.SetDirty(pawn);
        }
    }
}
