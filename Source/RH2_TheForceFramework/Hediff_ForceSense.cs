using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RH2_TheForceFramework
{
    public class Hediff_ForceSense : HediffWithComps
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            pawn.abilities.GainAbility(AbilityDefOf.ForceJump);
            pawn.abilities.GainAbility(AbilityDefOf.ForcePush);
            pawn.abilities.GainAbility(AbilityDefOf.MindTrick);
        }
    }
}
