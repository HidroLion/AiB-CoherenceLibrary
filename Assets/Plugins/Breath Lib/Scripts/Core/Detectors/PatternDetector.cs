using BreathLib.SerializableInterface;
using UnityEngine;

namespace BreathLib.Core
{
	public class PatternDetector : IBreathDetector
	{
		public SupportedPlatforms SupportedPlatforms => SupportedPlatforms.All;

		[SerializeField, Tooltip("The pattern to be interpreted as incoming breaths")]
		private SerializableInterface<IPatternContainer> _pattern;

		[Tooltip("While serialized, the normalized start point of the pattern. While playing, the normalized time which the pattern is currently at.")]
		public float normPatternTime;

		/// <summary> The pattern to be interpreted as incoming breaths </summary>
		public Pattern Pattern => _pattern.Value.Pattern;

		/// <summary>
		/// Sample to be reused to avoid garbage collection
		/// </summary>
		private BreathSample _workingSample = new BreathSample();
		public BreathSample GetBreathSample(BreathStream stream)
		{
			_workingSample = Pattern.GetTargetAtNormalizedTime(normPatternTime);

			normPatternTime += stream.SamplingRate / Pattern.length;
			while (normPatternTime > 1)
			{
				normPatternTime -= 1;
			}

			return _workingSample;
		}
	}
}