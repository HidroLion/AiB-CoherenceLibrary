using BreathLib.SerializableInterface;
using Debug = UnityEngine.Debug;
using SerializeField = UnityEngine.SerializeField;
using Tooltip = UnityEngine.TooltipAttribute;
using Range = UnityEngine.RangeAttribute;
using Min = UnityEngine.MinAttribute;
using Header = UnityEngine.HeaderAttribute;
using System.Linq;

namespace BreathLib.Core
{
	/// <summary>
    /// Correlator that is intended to be used with a pattern. The Correlator is expected to estimate the position of the pattern,
    /// which then can be used for output as a detected breath for stream usage.
    /// </summary>
	public abstract class PatternCorrelator : Correlator, IBreathDetector
	{
		public virtual SupportedPlatforms SupportedPlatforms => SupportedPlatforms.All;
		public virtual BreathSample GetBreathSample(BreathStream stream)
		{
			SyncValues();
			if (OverrideStreamBehaviours != null && OverrideStreamBehaviours.Some(x => x.Value.BreathStream == stream))
				return DefaultDetectorUseTolerance ? PatternSample : LastSample;
			else return DefaultDetectorUseTolerance ? LastSample : PatternSample;
		}

		[SerializeField, Tooltip("The pattern that is being correlated to a detected breath stream")]
		private SerializableInterface<IPatternContainer> _targetPattern = new(false);
		[System.NonSerialized] private Pattern _targetPatternValue;

		/// <summary> The target pattern that is being correlated to a detected breath stream </summary>
		public Pattern TargetPattern
		{
			get => _targetPatternValue ?? (_targetPatternValue = _targetPattern.Value.Pattern);
			set => _targetPatternValue = value;
		}

		[Tooltip("Starting time of the progression on play (normalized position while playing).")]
		[SerializeField, Range(0, 1)]
		protected float normBreathPosition = 0.0f;

		[Tooltip("The range within the pattern that the correlator will look into the past for correlation. If before and after are both 0, the correlator will only look at the current sample.")]
		[Min(0)]
		public float absBeforeCorrelation = 0f;

		[Tooltip("The range within the pattern that the correlator will look into the future for correlation. If before and after are both 0, the correlator will only look at the current sample rather than a range.")]
		[Min(0)]
		public float absAfterCorrelation = 0f;

		[Tooltip("If the pattern has a hold transitions, then the correlator will try to hold that breath until the detected stream releases (does not match) that hold.")]
		public bool SupportDynamicLengthPatterns;

		[Tooltip("The threshold in likeness which the hold will be maintained, assuming the detected stream matches the next section less than the hold section. The hold is maintained iff likeness(detected, hold) >= HoldThreshold AND likeness(detected, next) < NextSectionThreshold. For example, if the threshold is 0.0, then the hold will be maintained until the NextSectionThreshold is met. If the threshold is 1.0, then the hold will be maintained only as long as the detected stream exactly matches the hold section.")]
		[Range(0, 1)]
		[ConditionalField(nameof(SupportDynamicLengthPatterns))]
		public float HoldLikenessThreshold = 0.65f;

		[Tooltip("The threshold in likeness which the detected stream must match the next section before moving on. The hold is maintained iff likeness(detected, hold) >= HoldThreshold AND likeness(detected, next) < NextSectionThreshold. For example, if the threshold is 0.0, then the pattern will not hold at all. If the threshold is 1.0, then the detected stream will only move on to the next section if it exactly matches the next section or the Hold Threshold is no longer met.")]
		[Range(0, 1)]
		[ConditionalField(nameof(SupportDynamicLengthPatterns))]
		public float NextLikenessThreshold = 0.65f;


		[Header("Detector Settings")]
		[Tooltip("The seconds that the correlator offset the latest expected sample when outputting a detected breath. Used for allowing the user to see the upcoming breath before it begins to happen.")]
		public float absPreviewOffset = 1f;

		[Tooltip("When used as a breath detector, if this correlator should use the tolerance sample (gotten from before/after bounds) for the detected breath. Otherwise, the detected breath will be the latest sample that is being correlated to the target pattern.")]
		public bool DefaultDetectorUseTolerance = false;

		[Tooltip("All breath streams added here will use the opposite of the default detector use tolerance. This is useful for when you want to use the pattern correlator as a detector, but want to use the tolerance sample for one stream and the latest sample for another.")]
		public SerializableInterface<IBreathStreamContainer>[] OverrideStreamBehaviours = new SerializableInterface<IBreathStreamContainer>[0];


		public float NormBreathPosition { get { SyncValues(); return normBreathPosition; } }
		public float AbsBreathPosition { get { return NormBreathPosition * TargetPattern.length; } }

		protected float _holdTime = float.NaN;
		protected int _holdIndex = -1;
		public float HoldTime { get { SyncValues(); return _holdTime;  } protected set => _holdTime = value; }
		public bool Holding => !float.IsNaN(_holdTime);

		/// <inheritdoc />
		public override void Begin(bool syncWithHistory = false)
		{
			base.Begin(syncWithHistory);
			Debug.Assert(TargetPattern != null, "TargetPattern is null!");
			Debug.Assert(normBreathPosition >= 0 && normBreathPosition <= 1, "normStartPosition must be between 0 and 1.");
			Debug.Assert(absBeforeCorrelation >= 0 && absBeforeCorrelation <= 1, "normBeforeCorrelation must be between 0 and 1.");
			Debug.Assert(absAfterCorrelation >= 0 && absAfterCorrelation <= 1, "normAfterCorrelation must be between 0 and 1.");
			
			if (absBeforeCorrelation + absAfterCorrelation > TargetPattern.length)
				Debug.LogWarning("The sum of normBeforeCorrelation and normAfterCorrelation is greater than 1. A sum value or 1 (or more) will be representative of the entire pattern. Consider reducing the sum of the two values to less than the pattern length.");
		}

		protected BreathSample patternSample;
		protected BreathSample? latestSample;
		/// <summary>
		/// Sample that will be passed to the breath detector when <see cref="GetBreathSample"/> is called.
		/// Marked as an abstract property such that derived classes make sure to set it.
		/// </summary>
		protected BreathSample PatternSample { get => patternSample; private set => patternSample = value; }

		protected BreathSample LastSample => latestSample ?? (latestSample = Holding ?
			TargetPattern.Targets[(_holdIndex + 1) % TargetPattern.Targets.Length] :
			TargetPattern.GetTargetAtNormalizedTime(normBreathPosition + absPreviewOffset / TargetPattern.length)
		).Value;

		/// <inheritdoc />
		protected override void SyncValues()
		{
			Debug.Assert(TargetPattern != null, "TargetPattern is null!");
			base.SyncValues();

			latestSample = null;
		}

		protected override void SyncCompareUpdate(BreathSample detectedSample)
		{
			base.SyncCompareUpdate(detectedSample);

			if (SupportDynamicLengthPatterns)
			{
				if (ApplyHold(detectedSample))
					return;
			}

			PatternSample = GetPatternSample(detectedSample);
		}

		/// <summary>
        /// Set the pattern sample for detection and correlation. By default, gets the closest sample between the
        /// last value of the breath stream and the pattern. given the before and after range.
        /// </summary>
		protected virtual BreathSample GetPatternSample(BreathSample detectedSample)
		{
			if (TargetPattern.Targets.Length == 1)
				return TargetPattern.Targets[0];

			if (absBeforeCorrelation + absAfterCorrelation == 0)
				return TargetPattern.GetTargetAtNormalizedTime(normBreathPosition);

			BreathSample[] samples;

			if (absBeforeCorrelation + absAfterCorrelation > TargetPattern.length)
			{
				samples = TargetPattern.Targets;
			}
			else
			{
				float minTime = normBreathPosition - absBeforeCorrelation / TargetPattern.length;
				float maxTime = normBreathPosition + absAfterCorrelation / TargetPattern.length;

				(int minIndex, int maxIndex) = TargetPattern.GetKeyframeIndexesInRange(minTime, maxTime);

				UnityEngine.Debug.Log($"minIndex: {minIndex}, maxIndex: {maxIndex}");

				if (minIndex != -1 && maxIndex != -1)
				{
					samples = new BreathSample[maxIndex - minIndex + 3];

					for (int i = 0; i < samples.Length - 2; i++)
					{
						samples[i] = TargetPattern.Targets[(i + minIndex) % TargetPattern.Targets.Length];
					}
				}
				else samples = new BreathSample[2];

				samples[samples.Length - 2] = TargetPattern.GetTargetAtNormalizedTime(minTime);
				samples[samples.Length - 1] = TargetPattern.GetTargetAtNormalizedTime(maxTime);
			}

			if (samples == null)
				UnityEngine.Debug.Log("Samples is null");
			else UnityEngine.Debug.Log("Samples: " + string.Join(", ", samples.Select(s => s.ToString())));

			return detectedSample.MaxLikeness(samples);
		}

		/// <summary>
        /// Applies the hold keyframe to the normalized position, if the hold keyframe is active. Returns true if the hold keyframe exists.
        /// </summary>
        /// <param name="detectedSample">Detected sample to compare to the hold and next keyframe.</param>
        /// <returns>True if the hold keyframe exists.</returns>
		public virtual bool ApplyHold(BreathSample detectedSample)
		{
			if (TargetPattern.HasHoldAtNormalizedTime(normBreathPosition, out _holdIndex))
			{
				int nextIndex = (_holdIndex + 1) % TargetPattern.Targets.Length;

				float holdLikeness = detectedSample.SampleLikeness(TargetPattern.Targets[_holdIndex]);
				if (holdLikeness < HoldLikenessThreshold)
				{
					PatternSample = TargetPattern.Targets[nextIndex];
					normBreathPosition = TargetPattern.keyframes[nextIndex].time;
					_holdTime = float.NaN;
					return true;
				}
				
				float nextLikeness = detectedSample.SampleLikeness(TargetPattern.Targets[nextIndex]);
				if (nextLikeness >= NextLikenessThreshold)
				{
					PatternSample = TargetPattern.Targets[nextIndex];
					normBreathPosition = TargetPattern.keyframes[nextIndex].time;
					_holdTime = float.NaN;
					return true;
				}

				if (float.IsNaN(_holdTime))
					_holdTime = 0;

				_holdTime += DetectedBreath.SamplingRate;
				PatternSample = TargetPattern.Targets[_holdIndex];
				return true;
			}

			System.Diagnostics.Debug.Assert(float.IsNaN(_holdTime), "Hold time is not NaN when there is no hold keyframe.");
			return false;
		}

#if UNITY_EDITOR
		public void OverrideIPattern(IPatternContainer pattern)
		{
			if (UnityEngine.Application.isPlaying)
				throw new System.InvalidOperationException("Cannot override IPatternContainer while in play mode.");

			_targetPattern.Value = pattern;
		}
#endif
	}
}