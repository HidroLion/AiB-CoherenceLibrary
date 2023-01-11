using System;
using System.IO;
using UnityEngine;
#if NATIVE_BREATH_LIB
	using BreathLib.Native;
#endif

namespace BreathLib.Util
{
	public class AudioRecorder : MonoBehaviour
	{
		/// <summary> The number of bytes in the header of the WAV file. </summary>
		public const int HEADER_SIZE = 44;
		/// <summary> The number of bits taken per sample of audio. </summary>
		public const int BITS_PER_SAMPLE = 16;
		/// <summary> The maximum value of a 16-bit sample. </summary>
		public const int RESCALE_FACTOR = 32767;

		public enum RecorderTarget
		{
			[Tooltip("Record from the attached audio source or audio listener component.")]
			AttachedComponent = 0,

			[Tooltip("Record from an assigned audio source, reading from the clip.")]
			AudioSourceClip = 1,

#if NATIVE_BREATH_LIB
		[Tooltip("Record from a scripting sampler that is assigned to an audio mixer.")]
		AudioMixerScriptableSampler = 2,
#endif
		}

		[Tooltip("The target to record audio from.")]
		public RecorderTarget Target = RecorderTarget.AudioSourceClip;

		[Tooltip("The name of the file to save the recording to. If the file already exists, it will be overwritten.")]
		public string filename;


		/// <summary> The number of samples that have been written to the file. </summary>
		private int numSamples = 0;
		/// <summary> The file stream to write to (wav format) </summary>
		private FileStream fileStream;
		/// <summary> The number of channels to record. Used in file header. </summary>
		private int channels = -1;
		/// <summary> The frequency of the audio to record. Used in file header. </summary>
		private int frequency = -1;

		/// <summary> Called when the game object is enabled for the first time. Used to initialize the file stream. </summary>
		private void Start()
		{
			if (!filename.ToLower().EndsWith(".wav"))
				filename += ".wav";

			var filepath = Path.Combine(Application.persistentDataPath, filename);

			Debug.Log("Creating audio recording to the following file: " + filepath + " ...");

			// Make sure directory exists if user is saving to sub dir.
			Directory.CreateDirectory(Path.GetDirectoryName(filepath));
			fileStream = new FileStream(filepath, FileMode.Create);

			// Offset by header size
			fileStream.Seek(HEADER_SIZE, SeekOrigin.Begin);

#if NATIVE_BREATH_LIB
		if (Target == RecorderTarget.AudioMixerScriptableSampler)
		{
			_data = new float[16384];
			if (!NativesScriptingSampler.SetHistoryBuffer(scriptingSamplerIndex, _data))
				Debug.LogError("Failed to set history buffer on scripting sampler!");
		}
#endif

			if (Target != RecorderTarget.AudioSourceClip)
			{
				frequency = AudioSettings.outputSampleRate;
				channels = AudioSettings.speakerMode == AudioSpeakerMode.Mono ? 1 : 2;
			}
		}

		/// <summary>
		/// Clip:: Cached data from and audio clip. If the clip is streamed, this prevents garbage collection.
		/// Mixer:: The history buffer from the native plugin.
		/// </summary>
		private float[] _data;

		/// <summary> The index of the last sample that was written to the file. </summary>
		private int lastSampleIndex = 0;

		#region Assigned AudioSource Only

		[Tooltip("The audio source to record from. This script can also be attached to an AudioListener to record from the output of the audio mixer.")]
		[HideInInspector, SerializeField]
		private AudioSource audioSource;

		[Tooltip("If true, the data from the corresponding audio clip will be reread every frame. This is useful for streamed audio clips like the microphone or web audio.")]
		[HideInInspector, SerializeField]
		private bool isClipStreamed = false;

		/// <summary> Saved reference to the current clip, used to know if the clip has changed. </summary>
		private AudioClip _clip;

		/// <summary> Called every frame. Used to update the file with the data that has been played from the audio sources audio clip. </summary>
		private void UpdateAssignedSource()
		{
			if (audioSource == null) return;
			if (audioSource.clip == null) return;

			if (_clip != audioSource.clip)
			{
				ChangeClip();
			}

			if (lastSampleIndex != audioSource.timeSamples)
			{
				if (isClipStreamed) _clip.GetData(_data, 0);

				while (lastSampleIndex != audioSource.timeSamples)
				{
					if (NewData(_data[lastSampleIndex])) return;
					lastSampleIndex = (lastSampleIndex + 1) % _data.Length;
				}
			}
		}

		/// <summary> Called when the audio source's clip changes. </summary>
		private void ChangeClip()
		{
			_clip = audioSource.clip;
			_data = new float[_clip.samples];
			if (!isClipStreamed) _clip.GetData(_data, 0);

			lastSampleIndex = 0;

			if (channels == -1)
			{
				channels = audioSource.clip.channels;
			}
			else if (channels != audioSource.clip.channels)
			{
				Debug.LogWarning("AudioRecorder: Audio source changed channel count.");
				return;
			}

			if (frequency == -1)
			{
				frequency = audioSource.clip.frequency;
			}
			else if (frequency != audioSource.clip.frequency)
			{
				Debug.LogWarning("AudioRecorder: Audio source changed frequency.");
				return;
			}
		}

		#endregion

		#region Audio Mixer Scriptable Sampler Only
#if NATIVE_BREATH_LIB

	/// <summary> The index of the scripting sampler. </summary>
    [Tooltip("The index of the scripting sampler.")]
	[SerializeField, HideInInspector, Range(0, 127)]
	private int scriptingSamplerIndex = 0;

	/// <summary> Called every frame. Used to update the file with the data that has been played from the audio sources audio clip. </summary>
	private void UpdateScriptingSampler()
	{
		int write = NativesScriptingSampler.GetWriteIndex(scriptingSamplerIndex);

		while(lastSampleIndex != write)
		{
			if (NewData(_data[lastSampleIndex])) return;
			lastSampleIndex = (lastSampleIndex + 1) % _data.Length;
		}

		return;
	}

#endif
		#endregion

		#region Attached to Component Only

		/// <summary> Called by Unity when the audio source or an audio listener is attached to this object. </summary>
		/// <param name="data">Audio data</param>
		/// <param name="channels">Number of channels</param>
		private void OnAudioFilterRead(float[] data, int channels)
		{
			if (Target != RecorderTarget.AttachedComponent) return;

			if (fileStream == null)
				return;

			if (this.channels == -1) this.channels = channels;
			else if (this.channels != channels)
			{
				Debug.LogWarning("AudioRecorder: Audio source changed channel count.");
				return;
			}

			for (int i = 0; i < data.Length; i++)
			{
				if (NewData(data[i])) return;
			}
		}

		#endregion

		private void Update()
		{
			if (fileStream == null)
				return;

			if (Target == RecorderTarget.AudioSourceClip) UpdateAssignedSource();

#if NATIVE_BREATH_LIB
		else if (Target == RecorderTarget.AudioMixerScriptableSampler) UpdateScriptingSampler();
#endif
		}

		/// <summary> Saves the audio if it has not already been saved. </summary>
		private void OnApplicationQuit()
		{
			if (fileStream != null)
			{
				Save();
			}
		}

		/// <summary> Writes the given sample to the file. </summary>
		/// <param name="sample">The sample to write</param>
		private bool NewData(float sample)
		{
			fileStream.Write(ToBytes(sample), 0, 2);
			numSamples++;

			if (numSamples == int.MaxValue)
			{
				Save();
				return true;
			}

			return false;
		}

		/// <summary> Saves the recorded audio to a file and immediately stops the recording. </summary>
		public void Save()
		{
			byte[] data = new byte[HEADER_SIZE];
			GetWaveHeader(data);

			fileStream.Seek(0, SeekOrigin.Begin);
			fileStream.Write(data, 0, data.Length);

			fileStream.Close();
			fileStream = null;
		}

		/// <summary> Converts a float sample from audio to a ushort in bytes</summary>
		/// <param name="sample">Sample from audio to convert</param>
		/// <returns>Sample as a ushort in bytes</returns>
		private static byte[] ToBytes(float sample)
		{
			return BitConverter.GetBytes((short)(sample * RESCALE_FACTOR));
		}

		/// <summary> Fills a wave header for the given data </summary>
		/// <param name="header">The header to fill</param>
		private void GetWaveHeader(byte[] header)
		{
			// RIFF chunk descriptor
			Array.Copy(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, header, 0, 4);

			// RIFF chunk size
			Array.Copy(BitConverter.GetBytes(numSamples * channels * 2 + HEADER_SIZE - 8), 0, header, 4, 4);

			// WAVE chunk descriptor
			Array.Copy(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, header, 8, 4);

			// "fmt " sub-chunk
			Array.Copy(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, header, 12, 4);

			// "fmt " sub-chunk size
			Array.Copy(BitConverter.GetBytes(16), 0, header, 16, 4);

			// AudioFormat (1 = PCM)
			Array.Copy(BitConverter.GetBytes((ushort)1), 0, header, 20, 2);

			// NumChannels
			Array.Copy(BitConverter.GetBytes(channels), 0, header, 22, 2);

			// SampleRate
			Array.Copy(BitConverter.GetBytes(frequency), 0, header, 24, 4);

			// ByteRate (SampleRate * NumChannels * BitsPerSample/8)
			Array.Copy(BitConverter.GetBytes(frequency * channels * 2), 0, header, 28, 4);

			// BlockAlign (NumChannels * BitsPerSample/8)
			Array.Copy(BitConverter.GetBytes((ushort)(channels * BITS_PER_SAMPLE / 8)), 0, header, 32, 2);

			// BitsPerSample
			Array.Copy(BitConverter.GetBytes((ushort)BITS_PER_SAMPLE), 0, header, 34, 2);

			// "data" sub-chunk
			Array.Copy(System.Text.Encoding.UTF8.GetBytes("data"), 0, header, 36, 4);

			// "data" sub-chunk size
			Array.Copy(BitConverter.GetBytes(numSamples * channels * BITS_PER_SAMPLE / 8), 0, header, 40, 4);
		}

#if UNITY_EDITOR
		private void Reset()
		{
			bool hasAudioSource = GetComponent<AudioSource>() != null;
			bool hasAudioListener = GetComponent<AudioListener>() != null;

			if (hasAudioSource || hasAudioListener)
			{
				Target = RecorderTarget.AttachedComponent;
			}
		}

		private void OnValidate()
		{
			bool hasAudioSource = GetComponent<AudioSource>() != null;
			bool hasAudioListener = GetComponent<AudioListener>() != null;

			if (!hasAudioSource && !hasAudioListener && Target == RecorderTarget.AttachedComponent)
			{
				Debug.LogError("AudioRecorder: No audio source or listener found on this game object but target is an attached component. Switching to AudioSourceClip.");
				Target = RecorderTarget.AudioSourceClip;
			}

			if (hasAudioSource && hasAudioListener && Target == RecorderTarget.AttachedComponent)
			{
				Debug.LogError("AudioRecorder: Both an audio source and listener found on this game object and target is an attached component. They will interfere with one another. Switching to AudioSourceClip.");
				Target = RecorderTarget.AudioSourceClip;
			}
		}
	}



	[UnityEditor.CustomEditor(typeof(AudioRecorder))]
	public class AudioRecorderEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			AudioRecorder recorder = (AudioRecorder)target;

			AudioRecorder.RecorderTarget recorderTarget = (AudioRecorder.RecorderTarget)serializedObject.FindProperty("Target").enumValueIndex;

			// Display the audio source field if no audio listener is found
			if (recorderTarget == AudioRecorder.RecorderTarget.AudioSourceClip)
			{
				UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));
				UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("isClipStreamed"));
			}
#if NATIVE_BREATH_LIB
		else if (recorderTarget == AudioRecorder.RecorderTarget.AudioMixerScriptableSampler)
		{
			UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("scriptingSamplerIndex"));
		}
#endif

			serializedObject.ApplyModifiedProperties();
		}

#endif
	}
}