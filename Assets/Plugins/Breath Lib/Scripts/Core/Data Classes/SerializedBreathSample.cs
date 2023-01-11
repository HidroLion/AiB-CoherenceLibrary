using SerializeField = UnityEngine.SerializeField;
using Range = UnityEngine.RangeAttribute;
using System;

namespace BreathLib.Core
{
	[Serializable]
	public class SerializedBreathSample : IPatternContainer
	{
		[System.NonSerialized] private Pattern _pattern;
		public Pattern Pattern => _pattern ?? (_pattern =
			new Pattern("Serialized Breath Sample", 4, new Keyframe[] {
				new Keyframe(
					new BreathSample (
						_in: UseInOut ? In : null,
						no: UseYesNo ? No : null,
						nasal: UseMouthNose ? Nasal : null,
						volume: UseVolume ? Volume : null,
						pitch: UsePitch ? Pitch : null),
					0,
					Interpolators.Method.Constant)
			})
		);

		[SerializeField]
		private bool UseInOut;

		[SerializeField, ConditionalField(nameof(UseInOut)), Range(0, 1)]
		private float In;

		[SerializeField]
		private bool UseYesNo;

		[SerializeField, ConditionalField(nameof(UseYesNo)), Range(0, 1)]
		private float No;

		[SerializeField]
		private bool UseMouthNose;

		[SerializeField, ConditionalField(nameof(UseMouthNose)), Range(0, 1)]
		private float Nasal;

		[SerializeField]
		private bool UseVolume;

		[SerializeField, ConditionalField(nameof(UseVolume)), Range(0, 1)]
		private float Volume;

		[SerializeField]
		private bool UsePitch;

		[SerializeField, ConditionalField(nameof(UsePitch)), Range(0, 1)]
		private float Pitch;

		public SerializedBreathSample() { }

		public SerializedBreathSample(float? no, float? _in, float? nasal, float? volume, float? pitch)
		{
			UseInOut = _in.HasValue;
			In = _in ?? 0;
			UseYesNo = no.HasValue;
			No = no ?? 0;
			UseMouthNose = nasal.HasValue;
			Nasal = nasal ?? 0;
			UseVolume = volume.HasValue;
			Volume = volume ?? 0;
			UsePitch = pitch.HasValue;
			Pitch = pitch ?? 0;
		}
	}
}
