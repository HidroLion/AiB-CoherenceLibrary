using System.Collections;
using System.Collections.Generic;
using BreathLib;
using BreathLib.Unity;
using UnityEngine;
using BreathLib.Core;

public class KeyboardBreathDetect : IBreathDetector
{
	# region Configuration
	[Header("In/Out and Yes/No")]
	[Tooltip("The key that will be used to detect the In breath. Note that this is also used for Yes/No breaths, using if a key is held down to maintain the Yes/No breath.")]
	public KeyCode InKey = KeyCode.LeftArrow;
	[Tooltip("The key that will be used to detect the Out breath. Note that this is also used for Yes/No breaths, using if a key is held down to maintain the Yes/No breath.")]
	public KeyCode OutKey = KeyCode.RightArrow;
	[Tooltip("The speed in seconds at which the In/Out values changes from 0 to 1 or 1 to 0, assuming the key is held down.")]
	public float InOut_ChangeSpeed = 0.17f;
	[Tooltip("The speed in seconds at which the Yes/No values changes from 0 to 1 or 1 to 0, assuming the key is held down.")]
	public float YesNo_ChangeSpeed = 0.17f;


	[Header("Mouth/Nose")]
	[Tooltip("If this script should use keys for the mouth and nose. If this is false, the mouth and nose values will be null and thus not used for calculations.")]
	public bool UseMouthNose = true;

	[ConditionalField(nameof(UseMouthNose))]
	[Tooltip("The key that will be used to detect the Mouth breath. Note that this is also used for Yes/No breaths, using if a key is held down to maintain the Yes/No breath.")]
	public KeyCode MouthKey = KeyCode.UpArrow;

	[ConditionalField(nameof(UseMouthNose))]
	[Tooltip("The key that will be used to detect the Nose breath. Note that this is also used for Yes/No breaths, using if a key is held down to maintain the Yes/No breath.")]
	public KeyCode NoseKey = KeyCode.DownArrow;

	[ConditionalField(nameof(UseMouthNose))]
	[Tooltip("The speed in seconds at which the Mouth/Nose values changes from 0 to 1 or 1 to 0, assuming the key is held down.")]
	public float MouthNose_ChangeSpeed = 0.17f;

	[Header("Volume")]
	[Tooltip("If this script should use keys for the volume. If this is false, the volume will be null and thus not used for calculations.")]
	public bool UseVolume = true;

	[ConditionalField(nameof(UseVolume))]
	[Tooltip("The key that will be used to detect the Volume Up breath.")]
	public KeyCode VolumeUpKey = KeyCode.W;

	[ConditionalField(nameof(UseVolume))]
	[Tooltip("The key that will be used to detect the Volume Down breath.")]
	public KeyCode VolumeDownKey = KeyCode.S;

	[ConditionalField(nameof(UseVolume))]
	[Tooltip("The speed in seconds at which the Volume value changes from 0 to 1 or 1 to 0, assuming the key is held down.")]
	public float Volume_ChangeSpeed = 0.5f;

	[Header("Pitch")]
	[Tooltip("If this script should use keys for the pitch. If this is false, the pitch will be null and thus not used for calculations.")]
	public bool UsePitch = true;

	[ConditionalField(nameof(UsePitch))]
	[Tooltip("The key that will be used to detect the Pitch Up breath.")]
	public KeyCode PitchUpKey = KeyCode.D;

	[ConditionalField(nameof(UsePitch))]
	[Tooltip("The key that will be used to detect the Pitch Down breath.")]
	public KeyCode PitchDownKey = KeyCode.A;

	[ConditionalField(nameof(UsePitch))]
	[Tooltip("The speed in seconds at which the Pitch value changes from 0 to 1 or 1 to 0, assuming the key is held down.")]
	public float Pitch_ChangeSpeed = 0.5f;

	# endregion

	# region IBreathDetector

	public SupportedPlatforms SupportedPlatforms => SupportedPlatforms.Windows | SupportedPlatforms.MacOS | SupportedPlatforms.Linux;

	private BreathSample detected = new BreathSample();

	public void Initialize(BreathStream stream)
	{
		// The script supports active changes to flags, but the initializations are done here for clarity.
		detected = new BreathSample(
			yes: 0.5f,
			_in: 0.5f,
			mouth: UseMouthNose ? 0.5f : (float?)null,
			volume: UseVolume ? 0.5f : (float?)null,
			pitch: UsePitch ? 0.5f : (float?)null
		);
	}

	public BreathSample GetBreathSample(BreathStream stream)
	{
		if (!detected.Yes.HasValue || !detected.In.HasValue)
		{
			Debug.LogWarning("Detector is running before it has been initialized. This will not impact this detector, but is bad practice.");
			detected.Yes = 0.5f;
			detected.In = 0.5f;
		}

		bool yes = false;

		bool in_ = Input.GetKey(KeyCode.LeftArrow);
		bool out_ = Input.GetKey(KeyCode.RightArrow);

		if (in_ && out_)
		{
			if (detected.In > 0.5f)
				detected.In = Mathf.Clamp(detected.In.Value - stream.SamplingRate / InOut_ChangeSpeed, 0.5f, 1f);
			else if (detected.In < 0.5f)
				detected.In = Mathf.Clamp(detected.In.Value + stream.SamplingRate / InOut_ChangeSpeed, 0f, 0.5f);
			else
				detected.In = 0.5f;
		}
		else if (in_ && !out_)
		{
			detected.In = Mathf.Clamp01(detected.In.Value + stream.SamplingRate / InOut_ChangeSpeed);
		}
		else if (!in_ && out_)
		{
			detected.In = Mathf.Clamp01(detected.In.Value - stream.SamplingRate / InOut_ChangeSpeed);
		}

		yes = yes || in_ || out_;

		if (UseMouthNose)
		{
			if (!detected.Mouth.HasValue) detected.Mouth = 0.5f;

			bool mouth = Input.GetKey(KeyCode.UpArrow);
			bool nose = Input.GetKey(KeyCode.DownArrow);

			if (mouth && nose)
			{
				if (detected.Mouth > 0.5f)
					detected.Mouth = Mathf.Clamp(detected.Mouth.Value - stream.SamplingRate / MouthNose_ChangeSpeed, 0.5f, 1f);
				else if (detected.Mouth < 0.5f)
					detected.Mouth = Mathf.Clamp(detected.Mouth.Value + stream.SamplingRate / MouthNose_ChangeSpeed, 0f, 0.5f);
				else
					detected.Mouth = 0.5f;
			}
			else if (mouth && !nose)
			{
				detected.Mouth = Mathf.Clamp01(detected.Mouth.Value + stream.SamplingRate / MouthNose_ChangeSpeed);
			}
			else if (!mouth && nose)
			{
				detected.Mouth = Mathf.Clamp01(detected.Mouth.Value - stream.SamplingRate / MouthNose_ChangeSpeed);
			}

			yes = yes || mouth || nose;
		}
		else if (detected.Mouth.HasValue) detected.Mouth = null;

		if (UseVolume)
		{
			if (!detected.Volume.HasValue) detected.Volume = 0.5f;

			bool up = Input.GetKey(KeyCode.W);
			bool down = Input.GetKey(KeyCode.S);

			if (up && down)
			{
				if (detected.Volume > 0.5f)
					detected.Volume = Mathf.Clamp(detected.Volume.Value - stream.SamplingRate / Volume_ChangeSpeed, 0.5f, 1f);
				else if (detected.Volume < 0.5f)
					detected.Volume = Mathf.Clamp(detected.Volume.Value + stream.SamplingRate / Volume_ChangeSpeed, 0f, 0.5f);
				else
					detected.Volume = 0.5f;
			}
			else if (up && !down)
			{
				detected.Volume = Mathf.Clamp01(detected.Volume.Value + stream.SamplingRate / Volume_ChangeSpeed);
			}
			else if (!up && down)
			{
				detected.Volume = Mathf.Clamp01(detected.Volume.Value - stream.SamplingRate / Volume_ChangeSpeed);
			}
		}
		else if (detected.Volume.HasValue) detected.Volume = null;

		if (UsePitch)
		{
			if (!detected.Pitch.HasValue) detected.Pitch = 0.5f;

			bool up = Input.GetKey(KeyCode.D);
			bool down = Input.GetKey(KeyCode.A);

			if (up && down)
			{
				if (detected.Pitch > 0.5f)
					detected.Pitch = Mathf.Clamp(detected.Pitch.Value - stream.SamplingRate / Pitch_ChangeSpeed, 0.5f, 1f);
				else if (detected.Pitch < 0.5f)
					detected.Pitch = Mathf.Clamp(detected.Pitch.Value + stream.SamplingRate / Pitch_ChangeSpeed, 0f, 0.5f);
				else
					detected.Pitch = 0.5f;
			}
			else if (up && !down)
			{
				detected.Pitch = Mathf.Clamp01(detected.Pitch.Value + stream.SamplingRate / Pitch_ChangeSpeed);
			}
			else if (!up && down)
			{
				detected.Pitch = Mathf.Clamp01(detected.Pitch.Value - stream.SamplingRate / Pitch_ChangeSpeed);
			}
		}
		else if (detected.Pitch.HasValue) detected.Pitch = null;

		if (yes)
		{
			detected.Yes = Mathf.Clamp01(detected.Yes.Value + stream.SamplingRate / YesNo_ChangeSpeed);
		}
		else 
		{
			detected.Yes = Mathf.Clamp01(detected.Yes.Value - stream.SamplingRate / YesNo_ChangeSpeed);
		}

		return detected;
	}

	# endregion
}
