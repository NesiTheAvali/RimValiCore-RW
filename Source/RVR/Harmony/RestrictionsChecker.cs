﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RVCRestructured.RVR.Harmony
{
    public static class RestrictionsChecker
    {
        private static HashSet<Def> restrictedDefs = new HashSet<Def>();

        public static void AddRestriction(Def def)
        {
            if (restrictedDefs.Contains(def))
                return;
            restrictedDefs.Add(def);
        }

        public static void AddRestrictions<T>(List<T> defs) where T : Def
        {
            foreach(Def def in defs)
                AddRestriction(def);
        }

        public static bool IsRestricted(Def def)
        {
            return restrictedDefs.Contains(def);
        }
    }
}
