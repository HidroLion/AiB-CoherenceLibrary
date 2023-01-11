using System;
using System.Linq;
using UnityEngine;
using BreathLib.Core;
using System.Collections.Generic;

namespace BreathLib.Unity
{
	public class DetectorManager : MonoBehaviour, IBreathStreamContainer
	{
		public BreathStream BreathStream => DetectorController.breathStream;

		public DetectorController DetectorController = new();

		/// <summary>
		/// This is the counter for counting the number of frames that have passed since the last update.
		/// </summary>
		private int FixedFrameCount = 0;
		/// <summary>
		/// This is the number of frames that have to pass before the buffer is updated. This is fixed after initialization.
		/// </summary>
		private int FixedUpdateFrameCount = 0;
		
		/// <summary>
		/// Called once this GameObject is loaded into the scene.
		/// Initializes the BreathAggregation system.
		/// </summary>
		protected void Awake()
		{
			FixedUpdateFrameCount = Math.Max((int)Math.Round(BreathStream.SamplingRate / Time.fixedDeltaTime), 1);

			if (Math.Abs(BreathStream.SamplingRate - FixedUpdateFrameCount * Time.fixedDeltaTime) > 0.001f)
			{
				// TODO: add this to a custom inspector (as a warning label while editing)
				Debug.LogWarning("SampleDeltaTime is not a multiple of FixedDeltaTime. Sampling rate is being rounded to " + (FixedUpdateFrameCount * Time.fixedDeltaTime) + ". Sampling rate of the breath stream will be off by " + ((BreathStream.SamplingRate - FixedUpdateFrameCount * Time.fixedDeltaTime) / 1000) + " ms.");
			}
		}

		private void Start()
		{
			DetectorController.Initialize();
		}

		/// <summary>
		/// Preforms an update on the Aggregator, but only at the desired rate.
		/// </summary>
		private void FixedUpdate()
		{
			FixedFrameCount++;
			if (FixedFrameCount % FixedUpdateFrameCount != 0) return;
			else FixedFrameCount = 0;

			DetectorController.GatherSamples();
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			DetectorConfig config;
			for (int i = 0; i < DetectorController.detectors.Count; i++)
			{
				config = DetectorController.detectors[i];
				if (config.weight <= 0)
				{
					config.weight = 1;
					DetectorController.detectors[i] = config;
				}
			}
		}
#endif
	}
}
