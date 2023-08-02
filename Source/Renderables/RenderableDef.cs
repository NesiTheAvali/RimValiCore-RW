﻿using RimWorld;
using RVCRestructured.RVR;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RVCRestructured.Defs
{
    /// <summary>
    /// Used to render the graphics of a pawn into the world.
    /// </summary>
    public class RenderableDef : Def, IRenderable
    {
        public List<BaseTex> textures = new List<BaseTex>();
        public bool hiddenInBed = false;
        public BodyPartGraphicPos east;
        public BodyPartGraphicPos west = null;
        public BodyPartGraphicPos south;
        public BodyPartGraphicPos north;

        public RenderableDef linkTexWith;
        public RenderableDef linkPosWith;

        public string colorSet;

        public bool showsInBed = true;

        public string bodyPart;

        bool flipLayerEastWest = true;
        bool flipYPos = false;
        public BodyPartGraphicPos this[int i] => GetBodyPartGraphicPosFromIntRot(i);

        public override IEnumerable<string> ConfigErrors()
        {
            RVCLog.Log($"{defName} has a null east GraphicPos.", RVCLogType.Error, east == null);
            RVCLog.Log($"{defName} has a null south GraphicPos.", RVCLogType.Error, south == null);
            RVCLog.Log($"{defName} has a null north GraphicPos.", RVCLogType.Error, north == null);
            RVCLog.Log($"{defName} has no textures.", RVCLogType.Error, textures.EnumerableNullOrEmpty());
            return base.ConfigErrors();
        }


        public bool CanDisplay(Pawn pawn, bool portrait = false)
        {
            IEnumerable<BodyPartRecord> bodyParts = pawn.health.hediffSet.GetNotMissingParts();
            bool bodyIsHiding = bodyPart == null || bodyParts.Any(x => x.def.defName.ToLower() == bodyPart.ToLower() || x.Label.ToLower() == bodyPart.ToLower());
            return (portrait && !bodyIsHiding) || ((!pawn.InBed() || (pawn.CurrentBed().def.building.bed_showSleeperBody) || showsInBed) && bodyIsHiding);
        }
        public BodyPartGraphicPos GetPos(Rot4 rot)
        {
            if (west == null)
            {
                west = new BodyPartGraphicPos()
                {
                    position = -east.position,
                    size = east.size
                };
                if (!flipLayerEastWest)
                    west.position.y = east.position.y;
                if (!flipYPos)
                    west.position.z = east.position.z;
            }

            return GetBodyPartGraphicPosFromIntRot(rot.AsInt);
        }

        private BodyPartGraphicPos GetBodyPartGraphicPosFromIntRot(int rot)
        {
            switch (rot)
            {
                case 0:
                    if (linkPosWith != null)
                    {
                        return new BodyPartGraphicPos()
                        {
                            position = north.position + linkPosWith.north.position,
                            size=north.size
                        };
                    }
                    return north;
                case 2:
                    if (linkPosWith != null)
                    {
                        return new BodyPartGraphicPos()
                        {
                            position = south.position + linkPosWith.south.position,
                            size = south.size
                        };
                    }
                    return south;
                case 1:
                    if (linkPosWith != null)
                    {
                        return new BodyPartGraphicPos()
                        {
                            position = east.position + linkPosWith.east.position,
                            size = east.size
                        };
                    }
                    return east;
                case 3:
                    if (linkPosWith != null)
                    {
                        return new BodyPartGraphicPos()
                        {
                            position = west.position + linkPosWith.west.position,
                            size = west.size
                        };
                    }
                    return west;
                default:
                    return null;
            }
        }

       

        public BodyPartGraphicPos GetPos(Pawn pawn)
        {
            return GetPos(pawn.Rotation);
        }


        public string GetTexPath(Pawn pawn)
        {
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            return comp.GetTexPath(this);
        }

        public string GetMaskPath(Pawn pawn)
        {
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            return comp.GetMaskPath(this, pawn);
        }

        public bool ShowsInBed()
        {
            return showsInBed;
        }

        public TriColorSet ColorSet(Pawn pawn)
        {
            RVRComp comp = pawn.TryGetComp<RVRComp>();
            TriColorSet set = null;
            if (colorSet != null)
                set = comp[colorSet];
            if (set == null)
            {
                set = new TriColorSet(Color.red, Color.green, Color.blue, true);
            }
            return set;
        }
    }


    public class BodyPartGraphicPos
    {
        public Vector3 position;
        public Vector2 size;


    }

    public class HediffTex : BaseTex
    {
        public HediffDef hediffDef;

        public override bool CanApply(Pawn pawn)
        {
            return pawn.health.hediffSet.HasHediff(hediffDef);
        }
    }

    public class BackstoryTex : BaseTex
    {
        public string backstoryTitle;

        public override bool CanApply(Pawn p)
        {
            return p.story.Adulthood.identifier == backstoryTitle || p.story.Childhood.identifier == backstoryTitle;
        }
    }
    public class HediffStoryTex : BaseTex
    {
        public string backstoryTitle;
        public HediffDef hediffDef;
        public override bool CanApply(Pawn p)
        {
            return (p.story.Adulthood.identifier == backstoryTitle || p.story.Childhood.identifier == backstoryTitle) && p.health.hediffSet.HasHediff(hediffDef);
        }
    }

    public class BaseTex
    {
        public string texPath;
        public string femaleTexPath;

        public List<string> alternateMaskPaths = new List<string>();
        public List<string> alternateFemaleMaskPaths = new List<string>();


        /// <summary>
        /// Can the texture be applied to a pawn?
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public virtual bool CanApply(Pawn pawn)
        {
            return true;
        }

        /// <summary>
        /// Checks if the pawn can have alternative masks.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public bool HasAlternateMasks(Pawn pawn)
        {
            return (alternateFemaleMaskPaths.Count > 0 && pawn.gender == Gender.Female) || alternateMaskPaths.Count > 0;
        }

        /// <summary>
        /// Gets all possible masks for a pawn.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public List<string> GetMasks(Pawn pawn)
        {
            return HasAlternateMasks(pawn) ? pawn.gender == Gender.Female ? alternateFemaleMaskPaths : alternateMaskPaths : new List<string>();
        }
    }
}
