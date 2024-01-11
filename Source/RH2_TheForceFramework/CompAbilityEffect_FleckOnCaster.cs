using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RH2_TheForceFramework
{
    public class CompAbilityEffect_FleckOnCaster : CompAbilityEffect
    {
		public new CompProperties_AbilityFleckOnTarget Props
		{
			get
			{
				return (CompProperties_AbilityFleckOnTarget)this.props;
			}
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (this.Props.preCastTicks <= 0)
			{
				SoundDef sound = this.Props.sound;
				if (sound != null)
				{
					sound.PlayOneShot(new TargetInfo(parent.pawn.Position, this.parent.pawn.Map, false));
				}
				this.SpawnAll(parent.pawn.Position);
			}
		}
		public override IEnumerable<PreCastAction> GetPreCastActions()
		{
			if (this.Props.preCastTicks > 0)
			{
				yield return new PreCastAction
				{
					action = delegate (LocalTargetInfo t, LocalTargetInfo d)
					{
						this.SpawnAll(parent.pawn.Position);
						SoundDef sound = this.Props.sound;
						if (sound == null)
						{
							return;
						}
						sound.PlayOneShot(new TargetInfo(this.parent.pawn.Position, this.parent.pawn.Map, false));
					},
					ticksAwayFromCast = this.Props.preCastTicks
				};
			}
			yield break;
		}

		private void SpawnAll(LocalTargetInfo target)
		{
			if (!this.Props.fleckDefs.NullOrEmpty<FleckDef>())
			{
				for (int i = 0; i < this.Props.fleckDefs.Count; i++)
				{
					this.SpawnFleck(target, this.Props.fleckDefs[i]);
				}
				return;
			}
			this.SpawnFleck(target, this.Props.fleckDef);
		}

		private void SpawnFleck(LocalTargetInfo target, FleckDef def)
		{
			if (target.HasThing)
			{
				FleckMaker.AttachedOverlay(target.Thing, def, Vector3.zero, this.Props.scale, -1f);
				return;
			}
			FleckMaker.Static(target.Cell, this.parent.pawn.Map, def, this.Props.scale);
		}
	}
	public class CompProperties_AbilityFleckOnCaster : CompProperties_AbilityFleckOnTarget
    {
		public CompProperties_AbilityFleckOnCaster()
        {
			compClass = typeof(CompAbilityEffect_FleckOnCaster);
        }
	}
}
