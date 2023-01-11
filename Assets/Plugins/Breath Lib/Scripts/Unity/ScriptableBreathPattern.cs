using UnityEngine;
using BreathLib.Core;
using System;
using BreathLib.SerializableInterface;

namespace BreathLib.Unity
{
	[CreateAssetMenu(fileName = "ScriptableBreathPattern", menuName = "BreathLib/ScriptableBreathPattern")]
	public class ScriptableBreathPattern : ScriptableObject, IPatternContainer
	{
		[SerializeField]
		private TextAsset BreathPatternJSON;

		[SerializeField]
		private bool OverrideInOut;

		[SerializeField, ConditionalField(nameof(OverrideInOut)), Range(0, 1)]
		private float In;

		[SerializeField]
		private bool OverrideYesNo;

		[SerializeField, ConditionalField(nameof(OverrideYesNo)), Range(0, 1)]
		private float No;

		[SerializeField]
		private bool OverrideMouthNose;

		[SerializeField, ConditionalField(nameof(OverrideMouthNose)), Range(0, 1)]
		private float Nasal;

		[SerializeField]
		private bool OverrideVolume;

		[SerializeField, ConditionalField(nameof(OverrideVolume)), Range(0, 1)]
		private float Volume;

		[SerializeField]
		private bool OverridePitch;

		[SerializeField, ConditionalField(nameof(OverridePitch)), Range(0, 1)]
		private float Pitch;

		[SerializeField]
		private bool OverrideLength;

		[SerializeField, ConditionalField(nameof(OverrideLength)), Min(0.01f)]
		private float Length;


		public Pattern Pattern
		{
			get {
				if (_cachedPattern != null && _cachedHash == EditHash())
					return _cachedPattern;

				Pattern rawPattern = PatternLibrary.GetBreathPattern(BreathPatternJSON);
				_cachedHash = EditHash();

				if (OverrideInOut || OverrideYesNo || OverrideMouthNose || OverrideVolume || OverridePitch)
				{
					_cachedPattern = new Pattern(
						rawPattern.name + " (Scriptable: " + _cachedHash + ")",
						OverrideLength ? Length : rawPattern.keyframes.Length,
						new Core.Keyframe[rawPattern.keyframes.Length]
					);
					for (int i = 0; i < _cachedPattern.keyframes.Length; i++)
					{
						var keyframe = rawPattern.keyframes[i];
						_cachedPattern.keyframes[i] = new Core.Keyframe();
						_cachedPattern.keyframes[i].time = keyframe.time;
						_cachedPattern.keyframes[i].target = new BreathSample(
							_in: OverrideInOut ? In : keyframe.target.In,
							no: OverrideYesNo ? No : keyframe.target.No,
							nasal: OverrideMouthNose ? Nasal : keyframe.target.Nasal,
							volume: OverrideVolume ? Volume : keyframe.target.Volume,
							pitch: OverridePitch ? Pitch : keyframe.target.Pitch
						);
						_cachedPattern.keyframes[i].transition = keyframe.transition;
					}
				}
				else
				{
					_cachedPattern = rawPattern;
				}
				return _cachedPattern;
			}
		}

		[NonSerialized] private Pattern _cachedPattern;
		[NonSerialized] private int _cachedHash;
		private int EditHash()
		{
			int hash = 0;
			hash ^= OverrideInOut.GetHashCode();
			hash ^= OverrideYesNo.GetHashCode();
			hash ^= OverrideMouthNose.GetHashCode();
			hash ^= OverrideVolume.GetHashCode();
			hash ^= OverridePitch.GetHashCode();
			hash ^= OverrideLength.GetHashCode();
			hash ^= In.GetHashCode();
			hash ^= No.GetHashCode();
			hash ^= Nasal.GetHashCode();
			hash ^= Volume.GetHashCode();
			hash ^= Pitch.GetHashCode();
			hash ^= Length.GetHashCode();
			hash ^= BreathPatternJSON.GetHashCode();
			return hash;
		}
	}
}