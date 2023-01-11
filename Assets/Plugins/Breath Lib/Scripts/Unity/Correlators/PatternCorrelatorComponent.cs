using UnityEngine;
using BreathLib.Core;
using System;

namespace BreathLib.Unity
{
	public class PatternCorrelatorComponent : CorrelatorComponent<PatternCorrelator>, IBreathDetector
	{
		[SerializeField, ClassExtends(typeof(PatternCorrelator))] protected ClassTypeReference correlatorType;
		protected override ClassTypeReference CorrelatorType => correlatorType;

		public SupportedPlatforms SupportedPlatforms => Correlator.SupportedPlatforms;
		public BreathSample GetBreathSample(BreathStream stream) => Correlator.GetBreathSample(stream);
	}
}