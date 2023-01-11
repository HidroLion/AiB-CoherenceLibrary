using System;
using BreathLib.Util;

// Unity Specific for serialization and debugging
using Debug = UnityEngine.Debug;
using Tooltip = UnityEngine.TooltipAttribute;
using Range = UnityEngine.RangeAttribute;
using Min = UnityEngine.MinAttribute;

namespace BreathLib.Core
{
	[Serializable]
	/// <summary>
    /// Correlator where the correlation is how well the detected breath matches the target breath in realtime.
    /// The target breath moved at a constant (independent) speed, regardless of correlation to the detected breath.
    /// 
    /// This mechanism is useful for following a breath pattern which is visible to the user.
    /// For example, matching the tempo of a song or the waves of an ocean.
    /// (The song plays with or without the user, but the user is still trying to match the song.)
    /// </summary>
	public class FollowCorrelator : PatternCorrelator
	{
		#region Config

		[Tooltip("Exponential scale to the correlation value to make it more or less sensitive. 1 = normal, 0.5 = half sensitive, 2 = twice sensitive, etc.")]
		[Min(0.01f)]
		public float ScalePower = 1f;

		#endregion

		public override void Begin(bool syncWithHistory = false) 
		{
			base.Begin(syncWithHistory);

			Debug.Assert(ScalePower >= 0.01f, "scalePower must be greater than 0.01f.");
		}

		protected override void SyncCompareUpdate(BreathSample detectedSample)
		{
			base.SyncCompareUpdate(detectedSample);
			if (Holding) return;

			float likeness = detectedSample.SampleLikeness(PatternSample);
			float scaledLikeness = (float)Math.Pow(likeness, ScalePower);

			correlation.Set(scaledLikeness);

			normBreathPosition += DetectedBreath.SamplingRate / TargetPattern.length;

			while (normBreathPosition >= 1)
				normBreathPosition -= 1;
		}
	}
}