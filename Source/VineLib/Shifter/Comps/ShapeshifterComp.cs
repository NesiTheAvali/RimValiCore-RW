﻿using RVCRestructured.Defs;
using RVCRestructured.RVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.Shifter
{
    public class ShapeshifterComp : ThingComp
    {
        private ThingDef currentForm;
        public ThingDef CurrentForm
        {
            get
            {
                if(currentForm == null)
                {
                    currentForm = parent.def;
                }
                return currentForm;
            }
        }

        public void SetForm(ThingDef def)
        {
            currentForm = def;
            Pawn pawn = parent as Pawn;
            Log.Message($"{pawn.Name.ToStringShort} became {currentForm}");
            
        }

        public void RevertForm()
        {
            SetForm(parent.def);
        }

        public override void PostExposeData()
        {
            Scribe_Defs.Look(ref currentForm, nameof(currentForm));
            base.PostExposeData();
        }
    }
}
