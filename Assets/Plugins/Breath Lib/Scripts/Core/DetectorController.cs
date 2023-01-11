using System;
using System.Collections.Generic;
using System.Linq;

using Debug = UnityEngine.Debug;

namespace BreathLib.Core
{
	/// <summary>
	/// Post Processor for aggregating the Temporal Breath data into timelines, stored in an AggregateData object.
	/// Notable functions include:
	/// <list>
	/// 	<item>
	/// 		<term>Sample Frequency Synchronization</term>
	/// 		<description>Synchronizes the sample frequency of the detectors with a fixed sampling rate (much better for AI processing and usage).</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Data Correction</term>
	/// 		<description>Corrects the data for any errors, abnormalities, or outliers.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>Data Aggregation</term>
	/// 		<description>Aggregates the data into a timeline for the most recent in-the-moment data.</description>
	/// 	</item>
	/// </list>
	/// </summary>
	[Serializable]
	public class DetectorController
	{
		public BreathStream breathStream = new BreathStream();
		public List<DetectorConfig> detectors;
		protected float[][] cached_weights;

		/// <summary>
		/// Sets up the Breath CAS manager.
		/// </summary>
		/// <param name="config">The configuration for this CAS</param>
		/// <param name="detectors">List of Detectors that are running on this CAS</param>
		public DetectorController(BreathStream breathStream = null, List<DetectorConfig> detectors = null)
		{
			this.breathStream = breathStream ?? new BreathStream();
			this.detectors = detectors ?? new List<DetectorConfig>();
		}

		public bool Initialized { get; private set; }

		/// <summary>
        /// Adds detector to the controller, with a corresponding configuration.
        /// Make sure that the detector is initialized before adding it to the list of detectors, assuming that the detector is already initialized and running.
        /// </summary>
        /// <param name="config">The configuration for the detector</param>
		public void AddDetector(DetectorConfig config)
		{
			detectors.Add(config);
			SetWeights();
		}

		public void RemoveDetector(IBreathDetector detector)
		{
			detectors.RemoveAll(d => d.Detector == detector);
			SetWeights();
		}

		internal void Initialize()
		{
			if (Initialized) return;
			Initialized = true;
			
			breathStream.Initialize();

			foreach (var detector in detectors)
			{
				detector.Detector.Initialize(breathStream);
			}

			SetWeights();
		}

		protected void SetWeights(bool dirty = false)
		{
			cached_weights = detectors.Select(conf => conf.GetIndividualWeights(dirty)).ToArray();
		}

		/// <summary>
        /// Fills the stream by gathering samples from the detectors.
        /// If the stream is already filled, it will not do anything unless the refill parameter is set to true.
        /// If refill is set to true, this will gather samples equal to the capacity of the stream (i.e. it will refill the stream with new data).
        /// </summary>
        /// <param name="refill">If true, it will refill the stream with new data. If false, it will only fill the stream if it is not already filled.</param>
		internal void FillStream(bool refill = false)
		{
			if (!Initialized)
			{
				Debug.LogWarning("Detector Controller is not initialized. Make sure that it is initialized before calling this function. Initializing now...");
				Initialize();
			}

			int samplesToTake;
			if (refill)
			{
				samplesToTake = breathStream.Capacity;
			}
			else if (breathStream.TotalSamples < breathStream.uCapacity)
			{
				samplesToTake = (int)(breathStream.uCapacity - breathStream.TotalSamples);
			}
			else return; // Already filled.

			for (int i = 0; i < samplesToTake; i++)
			{
				GatherSamples();
			}
		}

		private BreathSample _lastSample;
		/// <summary>
		/// Updates the AggregateData object with the new data.
		/// TODO: Manage the synchronization of the sample frequency.
		/// </summary>
		internal void GatherSamples()
		{
			if (!Initialized)
			{
				Debug.LogWarning("Detector Controller is not initialized. Make sure that it is initialized before calling this function. Initializing now...");
				Initialize();
			}

			// If statements are used for slight optimization / validation.
			if (detectors == null || detectors.Count == 0)
			{
				UnityEngine.Debug.LogWarning("Detector Controller does not have any detectors. Make sure that it is configured properly by adding BreathDetector objects/scripts to the list of detectors.");
				_lastSample.Clear();
			}
			else if (detectors.Count == 1)
			{
				_lastSample = detectors[0].Detector.GetBreathSample(breathStream);

				if (detectors[0].useIndividualWeights)
					for (int i = 0; i < _lastSample.Length; i++)
						if (detectors[0].GetIndividualWeights()[i] < 0.01f)
							_lastSample[i] = null;
			}
			else
			{
				_lastSample = BreathSample.MergeSamples(
					detectors.Select(conf =>
						(conf.Detector.GetBreathSample(breathStream), conf.GetIndividualWeights())
					).ToArray()
				);
			}

			breathStream.Enqueue(_lastSample);
		}
	}
}