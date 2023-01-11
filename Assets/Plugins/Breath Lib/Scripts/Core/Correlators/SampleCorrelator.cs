using System;
using BreathLib.Core;

namespace BreathLib.Unity
{
	[Serializable]
	/// <summary>
    /// Correlator where the correlation is how well the detected breath matches the target breath in realtime.
    /// The target breath moved at a constant (independent) speed, regardless of correlation to the detected breath.
    /// 
    /// This mechanism is useful for following a breath pattern which is visible to the user.
    /// For example, matching the tempo of a song or the waves of an ocean.
    /// (The song plays with or without the user, but the user is still trying to match the song.)
    /// </summary>
	public class SampleCorrelator : PatternCorrelatorComponent
	{
#if UNITY_EDITOR
		protected void Reset()
		{
			SetConfiguration<FollowCorrelator>();
			Correlator.OverrideIPattern(new SerializedBreathSample(0f, null, null, null, null));
		}
#endif
	}
}