using UnityEngine;
using BreathLib.Core;
using System;
using BreathLib.SerializableInterface;

namespace BreathLib.Unity
{
	[CreateAssetMenu(fileName = "ScriptableBreathDetector", menuName = "BreathLib/ScriptableBreathDetector")]
	public class ScriptableBreathDetector : ScriptableObject, IBreathDetector
	{
		[SerializeField]
		private SerializableInterface<IBreathDetector> DetectorType;

		private IBreathDetector Configuration;

		public SupportedPlatforms SupportedPlatforms => Configuration.SupportedPlatforms;
		public void Initialize(BreathStream stream) => Configuration.Initialize(stream);
		public BreathSample GetBreathSample(BreathStream stream) => Configuration.GetBreathSample(stream);

#if UNITY_EDITOR
		private void Reset()
		{
			DetectorType = new SerializableInterface<IBreathDetector>(false);
		}
#endif

	}
}