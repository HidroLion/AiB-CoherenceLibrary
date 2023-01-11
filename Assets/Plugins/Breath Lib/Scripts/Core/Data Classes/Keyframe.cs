using System;

namespace BreathLib.Core
{
	[Serializable]
	public struct Keyframe
	{
		/// <summary>
		/// Target BreathSample at this specific time in the breath pattern.
		/// The specific time of this keyframe is defined by the total transition time up until this keyframe.
		/// </summary>
		public BreathSample target;

		/// <summary>
		/// The normalized time that this keyframe will take up in the breath pattern.
		/// </summary>
		public float time;

		/// <summary>
		/// The method of interpolation to use when transitioning between this keyframe and the next.
		/// <example>
		/// this target = { In: 1 }
		/// next target = { In: 0 }
		/// Linear: at 0.5 = { In: 0.5 }
		/// Linear: at 0.25 = { In: 0.25 }
		/// root: at 0.5 = { In: 0.71 }
		/// root: at 0.25 = { In: 0.05 }
		/// </example>
		/// </summary>
		public Interpolators.Method transition;

		public Keyframe(BreathSample target, float time, Interpolators.Method transition)
		{
			this.target = target;
			this.time = time;
			this.transition = transition;
		}

		public override string ToString()
		{
			return string.Format("state: [{0}], time: {1}, transition: {2}", target, time, transition);
		}
	}
}