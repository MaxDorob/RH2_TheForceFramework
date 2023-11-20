using HarmonyLib;
using RimWorld;
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
            harmony.Patch(AccessTools.Method(typeof(ThingWithComps), nameof(ThingWithComps.PreApplyDamage)), postfix: new HarmonyMethod(typeof(HediffComp_ForceShield), nameof(PostPreApplyDamage)));

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

        public static void PostPreApplyDamage(ThingWithComps __instance, ref DamageInfo dinfo, ref bool absorbed)
        {
            if (absorbed || !(__instance is Pawn pawn)) return;
            foreach (var shield in pawn.health.hediffSet.hediffs.OfType<HediffWithComps>().SelectMany(hediff => hediff.comps).OfType<HediffComp_ForceShield>())
            {
                shield.PreApplyDamage(ref dinfo, ref absorbed);
                if (absorbed) break;
            }
        }

        #endregion
        public static List<HediffComp_ForceShield> hediffs = new List<HediffComp_ForceShield>();
        private void PreApplyDamage(ref DamageInfo dinfo, ref bool absorbed)
        {
            var impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
            var loc = Pawn.TrueCenter() + impactAngleVect.RotatedBy(180f) * 0.5f;
            var num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
            if (Props.absorbedFleck != null) FleckMaker.Static(loc, Pawn.Map, Props.absorbedFleck, Props.absorbedFleckScale ?? num);
            if (Props.doDust)
            {
                var num2 = (int)num;
                for (var i = 0; i < num2; i++) FleckMaker.ThrowDustPuff(loc, Pawn.Map, Rand.Range(0.8f, 1.2f));
            }
            AbsorbDamage(ref dinfo);
            absorbed = true;
        }
        private bool AbsorbDamage(ref DamageInfo dinfo)
        {
            dinfo.SetAmount(0f);
            return true;
        }
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
        public FleckDef absorbedFleck;
        public float? absorbedFleckScale;
        public bool doDust;
    }
}
