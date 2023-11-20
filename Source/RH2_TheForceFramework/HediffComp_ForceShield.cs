using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RH2_TheForceFramework
{
    [StaticConstructorOnStartup]
    public class HediffComp_ForceShield : HediffComp
    {
        #region harmony
        public HediffComp_ForceShield()
        {
            var harmony = new Harmony("RH2_TheForceFramework");
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.DrawAt)), postfix: new HarmonyMethod(typeof(HediffComp_ForceShield), nameof(PawnPostDrawAt)));
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.SpawnSetup)), postfix: new HarmonyMethod(typeof(HediffComp_ForceShield), nameof(PostSpawn)));
            harmony.Patch(AccessTools.Method(typeof(Pawn), nameof(Pawn.DeSpawn)), postfix: new HarmonyMethod(typeof(HediffComp_ForceShield), nameof(PostDespawn)));

        }
        public static void PostSpawn(Pawn __instance)
        {
            var hediff = __instance.health.hediffSet.GetFirstHediffOfDef(RH2_DefOf.RH2ForceShield)?.TryGetComp<HediffComp_ForceShield>();
            if (hediff != null)
                hediffs.AddDistinct(hediff);
        }
        public static void PostDespawn(Pawn __instance)
        {
            var hediff = hediffs.FirstOrDefault(x => x.parent.pawn == __instance);
            if (hediff != null)
                hediffs.Remove(hediff);
        }
        public static void PawnPostDrawAt(Pawn __instance, Vector3 drawLoc)
        {
            hediffs.FirstOrDefault(x => x.parent.pawn == __instance)?.DrawAt(drawLoc);
        }
        #endregion
        public static List<HediffComp_ForceShield> hediffs = new List<HediffComp_ForceShield>();
        public virtual void DrawAt(Vector3 drawPos)
        {
            drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
            drawPos += Props.graphic.drawOffset;
            Graphics.DrawMesh(MeshPool.plane10,
                Matrix4x4.TRS(drawPos, Quaternion.AngleAxis(0f, Vector3.up),
                    new Vector3(Props.graphic.drawSize.x, 1f, Props.graphic.drawSize.y)),
                Graphic.MatSingleFor(Pawn), 0);
        }


        public HediffCompProperties_ForceShield Props => (HediffCompProperties_ForceShield)props;
        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            hediffs.AddDistinct(this);
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            hediffs.Remove(this);
        }

        public Graphic Graphic => Props.graphic?.Graphic;
    }
    public class HediffCompProperties_ForceShield : HediffCompProperties
    {
        public HediffCompProperties_ForceShield()
        {
            compClass = typeof(HediffComp_ForceShield);
        }
        public GraphicData graphic;

    }
}
