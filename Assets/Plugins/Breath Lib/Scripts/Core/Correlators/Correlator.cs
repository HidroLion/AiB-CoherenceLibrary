using System;
using BreathLib.Util;
using BreathLib.SerializableInterface;

using Debug = UnityEngine.Debug;
using SerializeField = UnityEngine.SerializeField;
using Tooltip = UnityEngine.TooltipAttribute;

namespace BreathLib.Core
{
	/// <summary>
	/// Extendable class that can be used for interpreting aggregate data. This contains a lot of Util functions for comparing breath sample.
	/// Intended for making comparisons between different breath patterns.
	/// </summary>

	/// TODO: Add abstract features. What needs to be shared between all correlators?
	public abstract class Correlator : ICorrelator
	{
		# region Fields and Parameters

		[SerializeField, Tooltip("The breath stream that is being correlated to a target pattern")]
		private SerializableInterface<IBreathStreamContainer> _detectedBreath = new(false);
		private BreathStream _detectedBreathValue;

		/// <summary> The target pattern that is being correlated to a detected breath stream </summary>
		public BreathStream DetectedBreath
		{
			get => _detectedBreathValue ?? (_detectedBreathValue = _detectedBreath.Value.BreathStream);
			set => _detectedBreathValue = value;
		}

		[SerializeField, Tooltip("The initial/current correlation between detected stream and target pattern, with lowpass alpha for rate of change")]
		/// <summary> The initial/current correlation between detected stream and target pattern, with lowpass alpha for rate of change </summary>
		public LowpassFloat correlation = new(0.0f, 0.05f);

		# endregion

		/// <summary>
		/// Correlation value between the current and target pattern, given the current 
		/// </summary>
		public virtual float Correlation { get { SyncValues(); return correlation; } }

		public virtual bool IsRunning { get; protected set; }

		/// <summary>
        /// Starts the correlator. The correlator should have all fields and parameters set before this is called.
        /// The correlator will not process any data until this is called.
        /// </summary>
        /// <param name="syncWithHistory"></param>
		public virtual void Begin(bool syncWithHistory = false)
		{
			UnityEngine.Debug.Log("Starting correlator");
			Debug.Assert(DetectedBreath != null, "DetectedBreath (breath stream) is null! Either set the detected breath stream manually or set the serialized detected breath in the unity editor.");
			Debug.Assert(correlation.value >= 0 && correlation.value <= 1, "likenessThreshold must be between 0 and 1.");
			Debug.Assert(correlation.lowpassAlpha >= 0 && correlation.lowpassAlpha <= 1, "likenessThreshold must be between 0 and 1.");

			if (DetectedBreath.TotalSamples == 0)
			{
				lastUpdateSample = 0;
			}
			else if (syncWithHistory)
			{
				if (DetectedBreath.TotalSamples > (ulong)DetectedBreath.Capacity)
					lastUpdateSample = DetectedBreath.TotalSamples - (ulong)DetectedBreath.Capacity;
				else
					lastUpdateSample = 0;
			} else lastUpdateSample = DetectedBreath.TotalSamples - 1;

			if (lastUpdateSample < 0)
				lastUpdateSample = 0;

			IsRunning = true;

			SyncValues();
		}

		/// <summary>
        /// Stops the correlator. Note that the correlator would only ever process data if it is running.
        /// Stop() is only used to ensure that the correlator does not process any data after this is called,
        /// causing an error to be thrown if data is tried to be processed.
        /// </summary>
		public virtual void Stop()
		{
			lastUpdateSample = ulong.MaxValue;

			IsRunning = false;
		}

		// TODO: make the update chunk pull from the breath stream to avoid duplication of data. This will limit the size of the chunk to the size of the breath stream.

		/// <summary>
		/// Values stored for the next chunk update. Used for any correlators that need to update in chunks (needing context from previous samples)
		/// </summary>
		private BreathSample[] updateChunk = null;
		/// <summary> Put index for update chunk. Maintains the number of samples in the chunk. </summary>
		private int updateChunkPutIndex = 0;

		/// <summary> Sets the chunk size for the update chunk. Sync Chunks is not called unless this function is invoked with a positive chunk size </summary>
		protected void InitializeUpdateChunk(int chunkSize)
		{
			if (chunkSize <= 0)
			{
				updateChunk = null;
				updateChunkPutIndex = 0;
				return;
			}

			if (updateChunk == null || updateChunk.Length != chunkSize)
			{
				var oldChunk = updateChunk;
				updateChunk = new BreathSample[chunkSize];
				if (oldChunk != null && updateChunkPutIndex > 0)
				{
					if (updateChunkPutIndex < updateChunk.Length)
					{
						Array.Copy(oldChunk, 0, updateChunk, 0, updateChunkPutIndex);
					}
					else
					{
						while (updateChunkPutIndex >= updateChunk.Length)
						{
							Array.Copy(oldChunk, 0, updateChunk, 0, updateChunk.Length);
							updateChunkPutIndex -= updateChunk.Length;
							SyncChunkUpdate(updateChunk);
						}
						Array.Copy(oldChunk, 0, updateChunk, 0, updateChunkPutIndex);
					}
				}
				else updateChunkPutIndex = 0;
			}
		}

		/// <summary> The last sample (indexed by the stream) used to update the correlator. </summary>
		private ulong lastUpdateSample = ulong.MaxValue;
		
		/// <summary> Syncs the correlator with the latest samples passed into the detected stream. </summary>
		protected virtual void SyncValues()
		{
			Debug.Assert(DetectedBreath != null, "DetectedBreath (breath stream) is null!");

			if (lastUpdateSample == ulong.MaxValue)
			{
				Debug.LogError("Correlator has not been enabled. Call Start() before trying to access values.");
				return;
			}

			if (lastUpdateSample == DetectedBreath.TotalSamples)
				return;

			if (lastUpdateSample + (ulong)DetectedBreath.Capacity < DetectedBreath.TotalSamples)
			{
				ulong latestTime = DetectedBreath.TotalSamples - (ulong)DetectedBreath.Capacity;
				Debug.LogWarning("Correlator: last update is too old. Skipping '" + (latestTime - lastUpdateSample) + "' samples.");
				lastUpdateSample = latestTime;
			}

			while (DetectedBreath.TotalSamples > lastUpdateSample)
			{
				SyncCompareUpdate(DetectedBreath[lastUpdateSample++].Value);

				if (updateChunk != null && updateChunk.Length > 0)
				{
					updateChunk[updateChunkPutIndex++] = DetectedBreath[lastUpdateSample - 1].Value;
					if (updateChunkPutIndex >= updateChunk.Length)
					{
						SyncChunkUpdate(updateChunk);
						updateChunkPutIndex = 0;
					}
				}
			}

			SyncLastUpdate(DetectedBreath.Last);
		}

		/// <summary>
        /// Called one time whenever the correlator is updated with a new samples, using the last sample in the detected stream as the update sample.
        /// </summary>
        /// <param name="detectedSample">The last sample in the detected stream.</param>
		protected virtual void SyncLastUpdate(BreathSample detectedSample) { }

		/// <summary>
        /// Called for every new sample in the detected stream when the correlator is updated
        /// </summary>
        /// <param name="detectedSample">The next new sample detected in the stream</param>
		protected virtual void SyncCompareUpdate(BreathSample detectedSample) { }

		/// <summary>
        /// Called whenever enough new samples are detected to fill the update chunk
        /// This function is only called if InitializeUpdateChunk is called with a positive chunk size
        /// </summary>
        /// <param name="chunk">The chunk of samples that were detected (in order)</param>
		protected virtual void SyncChunkUpdate(BreathSample[] chunk) { }


#if UNITY_EDITOR
		// protected virtual void Reset()
		// {
		// 	if (_detectedBreath == null)
		// 		_detectedBreath = new SerializableInterface<IBreathStreamContainer>(false);
		// 	if (_targetPattern == null)
		// 		_targetPattern = new SerializableInterface<IPatternContainer>(false);
		// }
#endif
	}
}
