using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace RH2_TheForceFramework
{
    public class CompAbilityEffect_MindTrick : CompAbilityEffect
    {
		public CompProperties_AbilityMindTrick Props
		{
			get
			{
				return (CompProperties_AbilityMindTrick)this.props;
			}
		}
		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			base.Apply(target, dest);
			Pawn pawn = target.Pawn;
			if (pawn != null && this.parent.pawn != pawn)
			{
				if (pawn.HostileTo(Faction.OfPlayer))
                {
					pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
					pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.PanicFlee, this.parent.def.label, true, false, null, true, false, true);
					return;
				}
				Pawn_InteractionsTracker interactions = this.parent.pawn.interactions;
				if (interactions == null)
				{
					return;
				}
				InteractionWorker_RecruitAttempt.DoRecruit(this.parent.pawn, pawn);
			}
		}

		// Token: 0x06005DA2 RID: 23970 RVA: 0x001F6FEB File Offset: 0x001F51EB
		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			return this.Valid(target, false);
		}

		// Token: 0x06005DA3 RID: 23971 RVA: 0x001FCF28 File Offset: 0x001FB128
		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn != null)
			{
				if (!this.Props.canApplyToMentallyBroken && !AbilityUtility.ValidateNoMentalState(pawn, throwMessages, this.parent))
				{
					return false;
				}
				if (!this.Props.canApplyToAsleep && !AbilityUtility.ValidateIsAwake(pawn, throwMessages, this.parent))
				{
					return false;
				}
				if (!this.Props.canApplyToUnconscious && !AbilityUtility.ValidateIsConscious(pawn, throwMessages, this.parent))
				{
					return false;
				}
			}
			return true;
		}
	}

	public class CompProperties_AbilityMindTrick : CompProperties_AbilitySocialInteraction
{
		public CompProperties_AbilityMindTrick()
        {
			this.compClass = typeof(CompAbilityEffect_MindTrick);
        }
	}
}
