using System;

// Unity Specific for serialization and debugging
using Debug = UnityEngine.Debug;
using Tooltip = UnityEngine.TooltipAttribute;
using Range = UnityEngine.RangeAttribute;
using Min = UnityEngine.MinAttribute;

namespace BreathLib.Core
{
	/// <summary>
	/// A Correlator that is used to track the users progress in a specific breath. This breath is static through the lifetime of this class, but new Progressive Breaths can be created.
	/// </summary>
	public class ProgressiveCorrelator : PatternCorrelator
	{
		[Tooltip("Cutoff threshold for progression. If likelihood of breath correlation is below this value, the progression will not continue.")]
		[Range(0, 1)]
		public float LikenessThreshold = 0.5f;

		[Tooltip("This scales the correlation value to make it more or less sensitive. 1 = normal, 0.5 = half sensitive, 2 = twice sensitive, etc.")]
		[Min(0.01f)]
		public float ScalePower = 1f;

		public override void Begin(bool syncWithHistory = false)
		{
			base.Begin(syncWithHistory);
			
			Debug.Assert(LikenessThreshold >= 0 && LikenessThreshold <= 1, "likenessThreshold must be between 0 and 1.");
			Debug.Assert(ScalePower >= 0.01f, "scalePower must be greater than 0.01f.");
		}

		protected override void SyncCompareUpdate(BreathSample detectedSample)
		{
			base.SyncCompareUpdate(detectedSample);
			if (Holding) return;

			float likeness = detectedSample.SampleLikeness(PatternSample);
			float cutoffLikeness = likeness < LikenessThreshold ? 0 : likeness;
			float scaledLikeness = (float)Math.Pow(cutoffLikeness, ScalePower);

			correlation.Set(scaledLikeness);

			normBreathPosition += scaledLikeness * DetectedBreath.SamplingRate / TargetPattern.length;

			while (normBreathPosition >= 1)
				normBreathPosition -= 1;
		}
	}
}