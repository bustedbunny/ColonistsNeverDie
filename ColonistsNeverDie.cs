using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistsNeverDie
{
    public class ColonistsNeverDie
    {

        [StaticConstructorOnStartup]
        public static class StartUp
        {
            static StartUp()
            {
                var harmony = new Harmony("com.example.patch");
                harmony.PatchAll();
            }
        }

        [HarmonyPatch(typeof(Pawn), "Kill")]
        class TryGetAttackVerbPatch
        {
            public static bool Prefix(Pawn __instance)
            {
                if (__instance.IsColonist)
                {
                    return false;
                }
                return true;
            }
        }

        [RimWorld.DefOf]
        public static class DefOf
        {
            public static BodyPartGroupDef Waist;
            public static BodyPartGroupDef Neck;
            public static BodyPartDef Ear;


            static DefOf()
            {
                DefOfHelper.EnsureInitializedInCtor(typeof(DefOf));
            }
        }


        [HarmonyPatch(typeof(HediffSet), "GetPartHealth")]
        class GetPartHealthPatch
        {
            private static readonly List<BodyPartGroupDef> vitalgroups = new List<BodyPartGroupDef>() { DefOf.Neck, BodyPartGroupDefOf.Torso, DefOf.Waist, BodyPartGroupDefOf.UpperHead};

            public static void Postfix(ref float __result, BodyPartRecord part, HediffSet __instance)
            {
                if (__instance.pawn.IsColonist)
                {
                    foreach (BodyPartGroupDef group in vitalgroups)
                    {
                        if (part.IsInGroup(group))
                        {
                            float num = Mathf.RoundToInt(Mathf.Max(__result, 1f));
                            __result = num;
                            return;
                        }
                    }
                    if (part.def == DefOf.Ear || part.LabelShort == "Right lung" || part.LabelShort == "Right kidney")
                    {
                        float num = Mathf.RoundToInt(Mathf.Max(__result, 1f));
                        __result = num;
                    }
                }
            }
        }
    }
}
