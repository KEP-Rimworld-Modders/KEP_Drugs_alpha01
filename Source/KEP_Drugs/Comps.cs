using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace KEP_Drugs
{
	public class KEPHediffCompProperties_DermalRegeneration : HediffCompProperties
	{
		public List<BodyPartDef> exclusionList = new List<BodyPartDef>();
		public int timeToHeal;
		public KEPHediffCompProperties_DermalRegeneration()
		{
			compClass = typeof(KEPHediffComp_DermalRegeneration);
		}
	}
	public class KEPHediffComp_DermalRegeneration : HediffComp
	{
		private int ticksToHeal;
		public new KEPHediffCompProperties_DermalRegeneration Props => (KEPHediffCompProperties_DermalRegeneration)props;
		private void ResetTicksToHeal()
		{
			ticksToHeal = Rand.Range(15, 30) * Props.timeToHeal;
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			ticksToHeal--;
			if (ticksToHeal <= 0)
			{
				TryHealRandomDermalWound();
				ResetTicksToHeal();
			}
		}
		public void TryHealRandomDermalWound()
		{
			if (base.Pawn.health.hediffSet.hediffs.Where((Hediff hd) => hd.IsPermanent()|| hd.def.chronic).TryRandomElement(out Hediff result))
			{
				foreach (BodyPartDef bodyPart in Props.exclusionList)
					if (result.Part.def == bodyPart)
						return;
				HealthUtility.CureHediff(result);
				if (PawnUtility.ShouldSendNotificationAbout(base.Pawn))
				{
					Messages.Message("MessagePermanentWoundHealed".Translate(parent.LabelCap, base.Pawn.LabelShort, result.Label, base.Pawn.Named("PAWN")), base.Pawn, MessageTypeDefOf.PositiveEvent);
				}
			}
		}
    }
}
