using System;

using Debug = UnityEngine.Debug;
using SerializeField = UnityEngine.SerializeField;
using BreathLib.SerializableInterface;
using Tooltip = UnityEngine.TooltipAttribute;
using Min = UnityEngine.MinAttribute;

namespace BreathLib.Core
{
	/// <summary>
	/// Configuration for a subscribed detector instance. This is used to help manage and merge the detector information.
	/// Notably, this contains the weight of the detector, which is used to determine the weight of the data when merging.
	/// TODO: Probably more here based on the detector own frequency or even per value weighting.
    /// TODO: Use serialization to be able to able the interface directly within the inspector when viewing in unity.
	/// </summary>
    [Serializable]
	public struct DetectorConfig
	{
		/// <summary>
		/// Detector associated with this configuration.
		/// </summary>
		[SerializeField] private SerializableInterface<IBreathDetector> detectorObject;

		private IBreathDetector detectorInterface;

		public IBreathDetector Detector => detectorInterface ?? detectorObject?.Value;

		/// <summary>
		/// Weight of the detector for merging breath sample into the stream.
		/// </summary>
        [Min(0.01f)]
		public float weight;

		[Tooltip("If true, the individual weights will be used for merging breath sample into the stream (scaled by the normal weight). If false, the weight will be used for all values. Mostly useful for selecting specific values to merge. For example, a accelerometer detector is great with in/out, but may be less with volume, so you can set the volume weight to 0 and use a microphone detector to pick up the slack.")]
		public bool useIndividualWeights;

		[Min(0.0f), ConditionalField("useIndividualWeights")]
		[Tooltip("Weight of the yes/no value when merging breath sample into the stream. If the value is 0, the yes/no value will not be merged (treated as null).")]
		public float yesNoWeight;

		[Min(0.0f), ConditionalField("useIndividualWeights")]
		[Tooltip("Weight of the in/out value when merging breath sample into the stream. If the value is 0, the in/out value will not be merged (treated as null).")]
		public float inOutWeight;

		[Min(0.0f), ConditionalField("useIndividualWeights")]
		[Tooltip("Weight of the nasal/mouth value when merging breath sample into the stream. If the value is 0, the nasal/mouth value will not be merged (treated as null).")]
		public float nasalMouthWeight;

		[Min(0.0f), ConditionalField("useIndividualWeights")]
		[Tooltip("Weight of the pitch value when merging breath sample into the stream. If the value is 0, the pitch value will not be merged (treated as null).")]
		public float pitchWeight;

		[Min(0.0f), ConditionalField("useIndividualWeights")]
		[Tooltip("Weight of the volume value when merging breath sample into the stream. If the value is 0, the volume value will not be merged (treated as null).")]
		public float volumeWeight;

		[NonSerialized]
		private float[] m_individualWeights;
		public float[] GetIndividualWeights(bool dirty = false)
		{
			if (m_individualWeights != null && dirty == false)
				return m_individualWeights;

			return m_individualWeights = new float[] {
				useIndividualWeights ? (yesNoWeight < 0.01f ? 0f : yesNoWeight * weight) : weight,
				useIndividualWeights ? (inOutWeight < 0.01f ? 0f : inOutWeight * weight) : weight,
				useIndividualWeights ? (nasalMouthWeight < 0.01f ? 0f : nasalMouthWeight * weight) : weight,
				useIndividualWeights ? (pitchWeight < 0.01f ? 0f : pitchWeight * weight) : weight,
				useIndividualWeights ? (volumeWeight < 0.01f ? 0f : volumeWeight * weight) : weight
			};
		}

		public DetectorConfig(DetectorConfig config)
		{
			this.detectorObject = config.detectorObject;
			this.detectorInterface = config.detectorInterface;
			this.weight = config.weight;

			this.useIndividualWeights = config.useIndividualWeights;
			this.yesNoWeight = config.yesNoWeight;
			this.inOutWeight = config.inOutWeight;
			this.nasalMouthWeight = config.nasalMouthWeight;
			this.pitchWeight = config.pitchWeight;
			this.volumeWeight = config.volumeWeight;

			m_individualWeights = null;
		}

		public DetectorConfig(IBreathDetector detector, float weight = 1)
		{
			Debug.Assert(detector != null, "new DetectorConfig(): Detector cannot be null.");
			Debug.Assert(weight > 0, "new DetectorConfig(): Weight must be greater than 0");

			this.detectorObject = null;
			this.detectorInterface = detector;
			this.weight = weight;

			useIndividualWeights = false;
			yesNoWeight = 1;
			inOutWeight = 1;
			nasalMouthWeight = 1;
			pitchWeight = 1;
			volumeWeight = 1;

			m_individualWeights = null;
		}
	}
}