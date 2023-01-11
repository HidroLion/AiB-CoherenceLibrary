using System.Collections.Generic;

// Unity Specific for serialization and debugging
using Debug = UnityEngine.Debug;
using SerializeField = UnityEngine.SerializeField;
using Tooltip = UnityEngine.TooltipAttribute;
using Range = UnityEngine.RangeAttribute;
using Min = UnityEngine.MinAttribute;

namespace BreathLib.Core
{
	/// <summary>
	/// Data class for holding and manipulating Aggregate Breath Samples.
	/// Contains a samples for the most recent in-the-moment data.
	/// </summary>
    [System.Serializable]
	public class BreathStream : IBreathStreamContainer
	{
		BreathStream IBreathStreamContainer.BreathStream => this;

		[SerializeField]
		[Min(1)]
		[Tooltip("The number of samples that can fit in the stream. This is the size of the buffer.")]
		private int streamCapacity = 100;

		[SerializeField]
		[Tooltip("The time in seconds between gathering breath samples into the breath stream.")]
		[Min(0.003f)]
		private float samplingRate = 0.02f; // 20ms

		/// <summary>
		/// Creates a new AggregateData object with the given buffer size for the samples.
		/// </summary>
		/// <param name="size">Fixed size of the sames</param>
        /// <param name="samplingRate">The time in seconds between gathering breath samples into the breath stream.</param>
		public BreathStream(int capacity = 100, float samplingRate = 0.02f)
		{
			this.streamCapacity = capacity;
			this.samplingRate = samplingRate;
		}

		public bool Initialized => samples != null;

		/// <summary>
        /// Initializes the stream by creating a new buffer for the samples and resetting the counters.
        /// </summary>
		public void Initialize()
		{
			Debug.Assert(streamCapacity > 0, "BreathStream.Initialize: Stream capacity must be greater than 0.");
			Debug.Assert(samplingRate >= 0.003f, "BreathStream.Initialize: Sampling rate must be greater than or equal to 0.003f.");

			samples = new BreathSample[streamCapacity];
			lastPut = -1;
			TotalSamples = 0;
		}

		/// <summary>
		/// The time in seconds between gathering breath samples into the breath stream.
		/// </summary>
		public float SamplingRate => samplingRate;

		/// <summary>
		/// The size of the stream, which is the number of samples that can fix in the stream.
		/// </summary>
		public int Capacity => streamCapacity;
		/// <summary>
        /// Unsigned long version of Capacity. This is used for indexing into the stream.
        /// </summary>
		public ulong uCapacity => (ulong)streamCapacity;

		/// <summary>
		/// High Quality Timeline: A buffer for history breath sample. Readonly due to fixed size.
		/// This is a circular buffer that has a fixed size.
		/// The oldest data is removed when the buffer is full.
		/// </summary>
		private BreathSample[] samples;

		/// <summary>
		/// The index for the end of the timeline.
		/// </summary>
		private int lastPut;

		/// <summary>
		/// Total number of samples that have been recorded into the samples.
		/// This is not the same as the size of the samples once the buffer is full (and overwrites data).
		/// </summary>
		public ulong TotalSamples { get; private set; }

		/// <summary>
		/// Adds a new data sample to the samples, overwriting the oldest data if the buffer is full.
		/// </summary>
		/// <param name="data">New element to add to the samples</param>
		public void Enqueue(BreathSample data)
		{
			if (Initialized == false)
			{
				Debug.LogWarning("BreathStream is not initialized, initializing now. Please call Initialize() before using the stream.");
				Initialize();
			}

			lastPut = (lastPut + 1) % samples.Length;
			TotalSamples++;
			samples[lastPut] = data;
		}

		/// <summary>
		/// Helper for getting the last data enqueued into the samples.
		/// </summary>
		/// <returns>The last Breath sample data that was added to the samples</returns>
		public BreathSample Last => samples[lastPut];

		public BreathSample? this[ulong index]
		{
			get
			{
				if (index >= TotalSamples || index + (ulong)samples.Length < TotalSamples)
					return null;
				else
					return samples[(int)(index % (ulong)samples.Length)];
			}
		}

		// TODO: add offsets/counts for getting a subset of the samples
		/// <summary>
		/// Copies the samples into a new array, which is ordered from oldest to newest.
		/// Useful for getting a snapshot of the samples at a specific time, without having to worry about circular indexing.
		/// </summary>
		/// <param name="buffer">The array that will be filled with BreathSample from the latest 'buffer.Length' samples.</param>
		public void GetSamples(BreathSample[] buffer)
		{
			if (Initialized == false)
			{
				Debug.LogWarning("BreathStream is not initialized, initializing now. Please call Initialize() before using the stream.");
				return;
			}

			if (buffer.Length > samples.Length)
			{
				Debug.LogError("Buffer is larger than the stream size. Please use a buffer of size " + samples.Length + " or smaller.");
				return;
			}

			if ((ulong)buffer.Length > TotalSamples)
			{
				Debug.LogError("Buffer is larger than the number of recorded samples. Please use a buffer of size " + TotalSamples + " or smaller.");
				return;
			}

			int index = lastPut + 1 - buffer.Length;
			if (index < 0) index += samples.Length;

			for (int i = 0; i < buffer.Length; i++, index = (index + 1) % samples.Length)
			{
				buffer[i] = samples[index];
			}
		}

		// TODO add function here for creating a pattern from the samples (and the length of the stream in seconds)
		public Pattern ToPattern()
		{
			if (Initialized == false)
			{
				Debug.LogWarning("BreathStream is not initialized, initializing now. Please call Initialize() before using the stream.");
				return null;
			}

			if (TotalSamples == 0)
			{
				Debug.LogWarning("BreathStream is empty, cannot convert to pattern.");
				return null;
			}

			throw new System.NotImplementedException("BreathStream.ToPattern() is not implemented yet.");

			// Pattern pattern = new Pattern();
			// pattern.Initialize((int)TotalSamples);

			// int index = lastPut + 1 - (int)TotalSamples;
			// if (index < 0) index += samples.Length;

			// for (int i = 0; i < TotalSamples; i++, index = (index + 1) % samples.Length)
			// {
			// 	pattern[i] = samples[index];
			// }

			// return pattern;
		}
	}
}
