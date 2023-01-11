using UnityEngine.Audio;
using UnityEngine;
using BreathLib.Core;

namespace BreathLib.Unity
{

	public abstract class MicrophoneBreathDetector : IBreathDetector
	{
		public virtual SupportedPlatforms SupportedPlatforms => SupportedPlatforms.All;

		public virtual BreathSample GetBreathSample(BreathStream stream)
		{
			throw new System.NotImplementedException();
		}
	}
}
