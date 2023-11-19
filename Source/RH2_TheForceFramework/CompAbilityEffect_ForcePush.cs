using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace RH2_TheForceFramework
{
    public class CompAbilityEffect_ForcePush : CompAbilityEffect
    {
        private Pawn Pawn
        {
            get
            {
                return this.parent.pawn;
            }
        }
        public CompProperties_AbilityForcePush Props => props as CompProperties_AbilityForcePush;
        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            foreach (IntVec3 c in this.AffectedCells(Pawn.Position, Pawn.Map))
            {
                foreach (var targetThing in c.GetThingList(Pawn.Map).ToList())
                {
                    if (targetThing is Pawn targetPawn && targetThing != Pawn)
                    {
                        JumpUtility.DoJump(targetPawn, GetDestination(targetPawn.Position), null, this.parent.VerbProperties.First());
                        targetPawn.stances.stunner.StunFor(Props.stunTime, Pawn);
                    }
                }
            }
            //if (this.Props.sprayEffecter != null)
            //{
            //    this.Props.sprayEffecter.Spawn(this.parent.pawn.Position, target.Cell, this.parent.pawn.Map, 1f).Cleanup();
            //}
        }
        private IntVec3 GetDestination(IntVec3 targetLoc)
        {
            //Vector2 vector = new Vector2(0, 3);
            //var angle = (new Vector2(targetLoc.x, targetLoc.z) - (new Vector2(Pawn.Position.x, Pawn.Position.z))).ToAngle();
            //vector = vector.RotatedBy(angle);
            //Log.Message($"{vector}, {angle}");
            IntVec3 vector = (targetLoc - Pawn.Position);
            var distanceTo = targetLoc.DistanceTo(Pawn.Position);
            var throwBackDistance = Mathf.Max(Mathf.Round(Props.throwDistnace.RandomInRange - (vector.LengthHorizontal * Props.decreaseBy)), 1f);
            var multiplier = distanceTo / throwBackDistance;
            Log.Message($"{vector}, {vector.LengthHorizontal * Props.decreaseBy}, {distanceTo}, {throwBackDistance}, {multiplier}");
            vector = new IntVec3(Mathf.RoundToInt(vector.x / multiplier), 0, Mathf.RoundToInt(vector.z / multiplier));
            Log.Message(vector.ToString());
            return targetLoc + vector;
        }
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            return true;
        }
        public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
        {
            return true;
        }
        //Modified CompAbilityEffect_SprayLiquid.AffectedCells
        private IEnumerable<IntVec3> AffectedCells(IntVec3 cell, Map map)
        {
            foreach (IntVec3 intVec in GenRadial.RadialCellsAround(cell, this.parent.def.EffectRadius, true))
            {
                if (intVec.InBounds(map) /*&& GenSight.LineOfSightToEdges(target.Cell, intVec, map, true, null)*/)
                {
                    yield return intVec;
                }
            }
        }

        private List<Pair<IntVec3, float>> tmpCellDots = new List<Pair<IntVec3, float>>();


        private List<IntVec3> tmpCells = new List<IntVec3>();

    }
    public class CompProperties_AbilityForcePush : AbilityCompProperties
    {
        public CompProperties_AbilityForcePush()
        {
            compClass = typeof(CompAbilityEffect_ForcePush);
        }
        public int stunTime = 600;
        public IntRange throwDistnace = new IntRange(3, 3);
        public float decreaseBy = 0.2f;
    }
}


