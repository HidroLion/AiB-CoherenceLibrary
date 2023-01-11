using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;
using Tooltip = UnityEngine.TooltipAttribute;

namespace BreathLib.Core
{
	/// <summary>
	/// Data class for a breath pattern, including a list of keyframes for a breath.
	/// Class contains helper methods for calculating in-between keyframes.
	/// </summary>
	[Serializable]
	public class Pattern : IPatternContainer
	{
		Pattern IPatternContainer.Pattern => this;

		[Tooltip("Name of the pattern. Used for debugging and presentation purposes.")]
		public string name;
		[Tooltip("Length of the pattern in seconds.")]
		public float length;
		[Tooltip("Array of keyframes that define the pattern.")]
		public Keyframe[] keyframes;

		[NonSerialized] private BreathSample[] _targets;
		public BreathSample[] Targets {
			get {
				if (_targets == null)
				{
					Initialize();
					_targets = keyframes.Select(kf => kf.target).ToArray();
				}
				return _targets;
			}
		}

		public bool overrideConstants = true;

		/// <summary>
		/// Creates a Pattern based on the given data.
		/// </summary>
		/// <param name="name">Name of the Pattern.</param>
		/// <param name="totalTime">Time in seconds that the breath pattern will take.</param>
		/// <param name="keyframes">Array that defines how the breath should look.</param>
		public Pattern(string name, float length, Keyframe[] keyframes)
		{
			this.name = name;
			this.length = length;
			this.keyframes = keyframes;
		}

		private bool _initialized = false;
		/// <summary>
        /// Sets up the keyframes for use, including assuming constants and sorting.
        /// </summary>
		public void Initialize()
		{
			if (_initialized)
				return;

			OrderKeyframes(this.keyframes);

			if (!overrideConstants)
			{
				this.keyframes = BuildConstants(this.keyframes);
			}

			ApplyHoldValues(this.keyframes);

			_initialized = true;
		}

		/// <summary>
		/// Determines if the pattern has been set and contains valid data.
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			if (length == 0 || keyframes == null || keyframes.Length == 0)
				return false;

			if (string.IsNullOrEmpty(name))
				Debug.LogWarning("Pattern.IsValid: Pattern has valid data but no name. Make sure patterns have a name for debugging and presentation purposes.");
			
			return true;
		}

		public (int, int) GetKeyframeIndexesInRange(float norm_a, float norm_b)
		{
			Initialize();

			int a_index = -1, b_index = -1;

			if (norm_a == norm_b)
			{
				// for (int i = 0; i < keyframes.Length; i++)
				// {
				// 	if (keyframes[i].time > norm_a)
				// 		break;
				// 	if (keyframes[i].time == norm_a)
				// 	{
				// 		if (a_index == -1)
				// 			a_index = b_index = i;
				// 		else b_index++;
				// 	}
				// }
			}

			else if (norm_a < norm_b)
			{
				for (int i = 0; i < keyframes.Length; i++)
				{
					if (keyframes[i].time > norm_a)
					{
						if (keyframes[i].time < norm_b)
						{
							if (a_index == -1)
								a_index = b_index = i;
							else b_index++;
						}
						else break;
					}
				}
			}

			else if (norm_a > norm_b)
			{
				for (int i = 0; i < keyframes.Length; i++)
				{
					if (keyframes[i].time < norm_b)
					{
						b_index = i;
					}
					else if (keyframes[i].time > norm_a)
					{
						if (a_index == -1)
						{
							a_index = i;

							if (b_index != -1) break;
						}

						b_index = i;
					}
				}
			}

			return (a_index, b_index);
		}


		/// <summary> Cached value to reduce garbage collection. </summary>
		private static BreathSample _workingResult;

		/// <summary>
        /// Returns the keyframe index that is currently active at the given normalized time (the closest keyframe before the given time).
        /// Function runs in O(log n) time.
        /// </summary>
        /// <param name="normalizedTime">Normalized position in the pattern.</param>
        /// <returns>The index of the keyframe that is currently active at the given time.</returns>
		public int GetKeyframeIndexAtNormalizedTime(float normalizedTime)
		{
			Initialize();

			// Loop the time. Time 1 == 0, Time 1.34 == 0.34, Time 4.5 == 0.5, Time -1.66 == 0.66, etc.
			normalizedTime = normalizedTime - (float)Math.Truncate(normalizedTime);
			if (normalizedTime < 0)
				normalizedTime += 1;

			int index = -1;

			// Binary search for the index of the keyframe that is currently active at the given time.
			int low = 0, high = keyframes.Length - 1;
			while (low <= high)
			{
				int mid = (low + high) / 2;
				float midTime = keyframes[mid].time;

				if (midTime < normalizedTime)
				{
					low = mid + 1;
				}
				else if (midTime > normalizedTime)
				{
					high = mid - 1;
				}
				else
				{
					index = mid;
					break;
				}
			}

			if (index == -1)
			{
				index = high;
			}

			return index;
		}

		/// <summary>
		/// Gets an in-between keyframe for the given normalized time [0,1].
		/// </summary>
		/// <param name="normalizedTime">Normalized position in the pattern.</param>
		/// <returns>A BreathSample object that contains the target state at the given time in the pattern.</returns>
		public BreathSample GetTargetAtNormalizedTime(float normalizedTime)
		{
			Initialize();

			// Loop the time. Time 1 == 0, Time 1.34 == 0.34, Time 4.5 == 0.5, Time -1.66 == 0.66, etc.
			normalizedTime = normalizedTime - (float)Math.Truncate(normalizedTime);

			if (normalizedTime < 0)
				normalizedTime += 1;

			// Find the keyframe that is closest to the normalized time.
			for (int tempIndex = 0; tempIndex < _workingResult.Length; tempIndex++)
			{
				// (time: float, temporal value: float, transition type: Method)
				(float, float, Interpolators.Method)?
					begin = null,   // The 'a' of the interpolation.
					end = null,	 // The 'b' of the interpolation.
					first = null;   // Reference to the first valid keyframe. Used for circular looping.
				for (int frameIndex = 0; frameIndex < keyframes.Length; frameIndex++)
				{
					if (keyframes[frameIndex].target[tempIndex] == null)
						continue;

					begin = end;
					end = (keyframes[frameIndex].time, keyframes[frameIndex].target[tempIndex].Value, keyframes[frameIndex].transition);

					if (first == null)
						first = end;

					if (keyframes[frameIndex].time > normalizedTime)
					{
						// If the normalized position is less than the first keyframe that appears, we need to also grab the
						// 	last keyframe (as long as the last doesn't match the first).
						if (begin == null)
						{
							for (int reverseIndex = keyframes.Length - 1; reverseIndex > frameIndex; reverseIndex--)
							{
								if (keyframes[reverseIndex].target[tempIndex] == null)
									continue;
								begin = (keyframes[reverseIndex].time - 1, keyframes[reverseIndex].target[tempIndex].Value, keyframes[reverseIndex].transition);
								break;
							}
						}

						break;
					}
				}

				//UnityEngine.Debug.LogFormat("i: {2} :: Begin: [{0}], end: [{1}], first: [{3}]", begin, end, tempIndex, first);

				// No keyframes with this value.
				if (end == null)
					continue;

				// If there was only one keyframe.
				else if (begin == null)
				{
					UnityEngine.Debug.Assert(first == end); // First is only different from end if there is more than one keyframe.
					_workingResult[tempIndex] = first.Value.Item2;
				}
				else
				{
					float pointInTransition;
					// Check for circular keyframe (ie, start time is after the end time due to looping)
					if (end.Value.Item1 < normalizedTime)
					{
						begin = end;
						end = first;

						pointInTransition = Interpolators.ILinear(
							// Cycle the end time.
							start: begin.Value.Item1,
							end: end.Value.Item1 + 1.0f,
							value: normalizedTime
						);
					}
					else
					{
						pointInTransition = Interpolators.ILinear(
							start: begin.Value.Item1,
							end: end.Value.Item1,
							value: normalizedTime
						);
					}

					if (begin.Value.Item3 == Interpolators.Method.Hold)
						_workingResult[tempIndex] = begin.Value.Item2;
					else
						_workingResult[tempIndex] = Interpolators.Interp(
							start: begin.Value.Item2,
							end: end.Value.Item2,
							time: pointInTransition,
							type: begin.Value.Item3
						);
				}
			}
			return _workingResult;
		}

		/// <summary>
		/// Helper for converting time in seconds to a normalized time [0,1] in the pattern.
		/// </summary>
		/// <param name="timeInSeconds">Time that will be converted</param>
		/// <returns>Normalized value [0,1] that equals the number of seconds into a pattern.</returns>
		public BreathSample GetTargetAtActualTime(float timeInSeconds)
		{
			return GetTargetAtNormalizedTime(timeInSeconds / length);
		}

		/// <summary>
        /// Predicate for if the pattern has a hold at the given normalized time.
        /// </summary>
        /// <param name="normalizedTime">The normalized time to check for a hold.</param>
        /// <param name="holdIndex">The index that the hold starts.</param>
        /// <returns>True if there is a hold at the given normalized time.</returns>
		public bool HasHoldAtNormalizedTime(float normalizedTime, out int holdIndex)
		{
			holdIndex = GetKeyframeIndexAtNormalizedTime(normalizedTime);

			if (keyframes[holdIndex].transition != Interpolators.Method.Hold)
			{
				holdIndex = -1;
				return false;
			}

			return true;
		}

		///<inheritdoc cref="HasHoldAtNormalizedTime(float, out int)"/>
		public bool HasHoldAtNormalizedTime(float normalizedTime)
		{
			return HasHoldAtNormalizedTime(normalizedTime, out int _);
		}



		/// <summary>
        /// Removes and combines keyframes, keeping the same shape of the curve within the error tolerance. This is useful for reducing the number of keyframes in a pattern, expecially if the pattern is being used in multiple places or the pattern is being saved to a file.
        /// TODO - This is not implemented yet.
        /// </summary>
        /// <param name="errorTolerance">The maximum error allowed between the original curve and the simplified curve.</param>
        /// <returns>The compression ratio of the simplified curve. Not representative of JSON compression ratio which will usually be greater.</returns>
		public float SimplifyKeyframes(float errorTolerance)
		{
			Debug.LogWarning("SimplifyKeyframes is not implemented yet.");
			return 1.0f;
		}

		private static List<Keyframe> _workingKeyframes;
		private static Keyframe[] BuildConstants(Keyframe[] orderedKeyframes)
		{
			if (_workingKeyframes == null)
				_workingKeyframes = new List<Keyframe>();
			else _workingKeyframes.Clear();

			_workingKeyframes.AddRange(orderedKeyframes);

			// For each keyframe, replace the Yes/No In/Out, and Mouth/Nose values with a new keyframe that has the same time with a transition type of Constant.
			Keyframe working;
			Interpolators.Method transition;
			for (int i = 0; i < _workingKeyframes.Count; i++)
			{
				transition = _workingKeyframes[i].transition;
				if (transition == Interpolators.Method.Constant)
					continue;

				if (_workingKeyframes[i].target.No.HasValue || _workingKeyframes[i].target.In.HasValue || _workingKeyframes[i].target.Nasal.HasValue)
				{
					if (!_workingKeyframes[i].target.Volume.HasValue && !_workingKeyframes[i].target.Pitch.HasValue)
					{
						working = _workingKeyframes[i];
						working.transition = Interpolators.Method.Constant;
						_workingKeyframes[i] = working;

						continue;
					}

					working.time = _workingKeyframes[i].time;
					working.transition = Interpolators.Method.Constant;
					working.target = new BreathSample(
						no: _workingKeyframes[i].target.No,
						_in: _workingKeyframes[i].target.In,
						nasal: _workingKeyframes[i].target.Nasal
					);

					if (i + 1 < _workingKeyframes.Count)
						_workingKeyframes.Insert(i + 1, working);
					else _workingKeyframes.Add(working);

					working.transition = transition;
					working.target = new BreathSample(
						volume: _workingKeyframes[i].target.Volume,
						pitch: _workingKeyframes[i].target.Pitch
					);

					_workingKeyframes[i] = working;
				}
			}

			return _workingKeyframes.ToArray();
		}

		/// <summary>
        /// Any keyframes with the "Hold" transition type will have all of their values set by using the previous valid keyframe values.
        /// </summary>
        /// <param name="orderedKeyframes">List of keyframes that have been ordered by time.</param>
		private static void ApplyHoldValues(Keyframe[] orderedKeyframes)
		{
			for (int i = 0; i < orderedKeyframes.Length; i++)
			{
				if (orderedKeyframes[i].transition == Interpolators.Method.Hold)
				{
					_workingResult = orderedKeyframes[i].target;

					for (int dataIndex = 0; dataIndex < orderedKeyframes[i].target.Length; dataIndex++)
					{
						if (orderedKeyframes[i].target[dataIndex].HasValue)
							continue;

						int j = i - 1;
						while (true)
						{
							if (j < 0)
								j = orderedKeyframes.Length - 1;

							if (j == i)
								break;

							if (orderedKeyframes[j].target[dataIndex].HasValue)
							{
								_workingResult[dataIndex] = orderedKeyframes[j].target[dataIndex];
								break;
							}

							j--;
						}
					}

					orderedKeyframes[i].target = _workingResult;

					int nextIndex = (i + 1) % orderedKeyframes.Length;
					for (int dataIndex = 0; dataIndex < orderedKeyframes[i].target.Length; dataIndex++)
					{
						if (!orderedKeyframes[nextIndex].target[dataIndex].HasValue)
							orderedKeyframes[nextIndex].target[dataIndex] = orderedKeyframes[i].target[dataIndex];
					}
				}
			}
		}

		private static void OrderKeyframes(Keyframe[] keyframes)
		{
			Array.Sort(keyframes, (a, b) => a.time.CompareTo(b.time));
		}

		public override string ToString()
		{
			return string.Format("BreathType: {0}, length: {1}, keyframes: [{2}]", name, length, string.Join(", ", keyframes));
		}
	}
}