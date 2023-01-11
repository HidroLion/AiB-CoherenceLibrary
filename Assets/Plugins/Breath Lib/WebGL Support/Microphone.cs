//#define SHOW_CODE
#if UNITY_WEBGL && !UNITY_EDITOR || SHOW_CODE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	public static class Microphone
	{

		[DllImport("__Internal")]
		internal static extern string GetDeviceNameAtIndex(int deviceIndex);

		[DllImport("__Internal")]
		internal static extern int GetDeviceCount();

		[DllImport("__Internal")]
		internal static extern int WebGL_Start(int deviceIndex, int[] variables, float[] clipData);

		[DllImport("__Internal")]
		internal static extern void WebGL_End(int deviceIndex);

		[DllImport("__Internal")]
		internal static extern void InitPermissions();

		[DllImport("__Internal")]
		internal static extern void InitDevices();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		private static void Initialize()
		{
			InitPermissions();
			InitDevices();
		}

		public static string[] devices
		{
			get
			{
				string[] result = new string[GetDeviceCount()];
				for (int i = 0; i < result.Length; i++)
				{
					result[i] = GetDeviceNameAtIndex(i);
					Debug.Log("Unity got device of name " + result[i]);
				}
				return result;
			}
		}

		public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency)
		{
			
			//a_device = GetDeviceIndex(deviceName);
			//WebGL_Start(a_device, frequency, 1);

			//return AudioClip.Create(deviceName, frequency * lengthSec, 1, frequency, false);
			// ADDITION
			if(deviceName == null){
				var _devices = devices; // Prevent double call
				if (_devices.Length != 0){
					deviceName = _devices[0];
					Debug.Log("CUSTOM DO KIM: null case in Start() reached! deviceName is now: " + deviceName);
				}
			}
			// ADDITION ^

			Debug.Log("Microphone.Start()...");

			if (!managers.TryGetValue(deviceName, out WebGL_MicManager manager))
			{
				Debug.Log("Microphone.Start(2)...");
				GameObject go = new GameObject(deviceName + ": WebGLManager");
				Object.DontDestroyOnLoad(go);

				Debug.Log("Microphone.Start(3)...");
				manager = go.AddComponent<WebGL_MicManager>();

				Debug.Log("Microphone.Start(4)...");
				managers.Add(deviceName, manager);
			}
			Debug.Log("Microphone.Start(5)...");
			return manager.Initialize(deviceName, loop, lengthSec, frequency);
		}

		public static void End(string deviceName)
		{
			WebGL_End(GetDeviceIndex(deviceName));
		}

		[NonSerialized]
		private static Dictionary<string, WebGL_MicManager> managers = new Dictionary<string, WebGL_MicManager>();

		public static int GetPosition(string deviceName)
		{
			// Modification by Nevin Foster
			if (deviceName == null){
				var _devices = devices; // Prevent double call
				if (_devices.Length != 0){
					deviceName = _devices[0];
					Debug.Log("DOWON KIM: STATIC GetPosition() null statement reached");
				} else{
					Debug.Log("There are no input audio devices availble for Microphone.Start()");
					//return -1;
				}
			}
			Debug.Log("CUSTOM DOWON KIM: GETPOSITION REACHED");
			if (managers.TryGetValue(deviceName, out WebGL_MicManager manager)){
				return manager.GetPosition();
			} 
			else {
				return -1;
			}
		}

		internal static int GetDeviceIndex(string deviceName)
		{
			string[] devs = devices;
			for (int i = 0; i < devs.Length; i++)
				if (devs[i] == deviceName)
					return i;
				 /*else if (deviceName == null){
					// Should return 0! MODIFICATION BY DO KIM
					return i;
				}*/
			return -1;
		}

		private class WebGL_MicManager : MonoBehaviour
		{
			public string deviceName { get; private set; }
			public int deviceIndex { get; private set; }

			internal int GetPosition()
			{
				// Put updated by WebGL
				Debug.Log("DO KIM: INTERNAL GETPOSITION OF MANAGER REACHED");
				return Put;
			}

			private AudioClip streamedClip;

			/// <summary> A pair that contains the shared data for Web media and Unity, includes put position [0], buffersize [1], frequency [2], and Channels [3]. Shared values 
			/// for each are used in case Web media cannot support some of the value, thus they will be overriden and will be caught by Unity. s_ for private shared memory. </summary>
			/// s_data is shared data where Unity and webGL share data. in array of values so WebGL puts into buffer. 
			private readonly int[] s_data = new int[] { -1, -1, -1, -1 };
			
			/// <summary> This is the index at which data should be put into the buffer. </summary>
			private int Put { get => s_data[0]; set => s_data[0] = value; }

			/// <summary> The buffer size for reading audio. The Web media will only write to the shared clip data in these intervals.
			/// Must be one of the following: 256, 512, 1024, 2048, 4096, 8192, 16384 </summary>
			private int BufferSize { get => s_data[1]; set => s_data[1] = value; }

			/// <summary> The shared buffer size. This will always be equal to the number of samples that can fit into the audio clip. </summary>
			private int Frequency { get => s_data[2]; set => s_data[2] = value; }

			/// <summary> The shared buffer size. This will always be equal to the number of samples that can fit into the audio clip. </summary>
			private int Channels { get => s_data[3]; set => s_data[3] = value; }

			/// <summary> The size of the shared clip data. This will always be equal to the number of samples that can fit into the audio clip. </summary>
			private int ClipSize { get => s_data[4]; set => s_data[4] = value; }

			/// <summary> The Clip Buffer, containing the audio data from the microphone. Used as a communication channel for Web media and Unity. s_ for private shared memory. This is
			/// a const pointer throughout the lifetime of each recording, however, it can be changed if new recordings are started before this object is discarded.</summary>
			private float[] s_clipData;

			private Coroutine UpdateMethod = null;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="deviceName"></param>
			/// <param name="loop">Normally, this only determines if the microphone should continue to overwrite data after clip has ended. Now, this also determines IF data ever needs to be overwriten.
			/// If loop is false, then the audioclip will have to overwrite ALL DATA every time it is updated to ensure that the clip is saved.</param>
			/// <param name="lengthSec"></param>
			/// <param name="frequency"></param>
			/// <returns></returns>
			public AudioClip Initialize(string deviceName, bool loop, int lengthSec, int frequency)
			{
				Debug.Log("Initializing manager...");

				this.deviceName = deviceName;
				deviceIndex = GetDeviceIndex(deviceName);

				Put = 0;
				BufferSize = 1024; // Note that Unity does not support user controll over the audio buffer size.
				Channels = 1; // Note that Unity only supports one channel on each audio device :/
				Frequency = frequency;
				ClipSize =  lengthSec * (Frequency * Channels);
				s_clipData = new float[BufferSize];

				Debug.Log("Calling WebGL_Start()...");
				WebGL_Start(deviceIndex, s_data, s_clipData);

				Debug.Log("WebGL_Start returned...");
				// Catch WebGL_Erorrs::
				if (Put < 0)
				{
					switch (Put)
					{
						case -1:
							// GetUserMedia not supported
							throw new NotImplementedException();
						case -2:
							// GetUserMedia error callback
							throw new NotImplementedException();
						default:
							throw new NotImplementedException("Default should not be reachable anyways..?");
					}
				}

				if (UpdateMethod != null) StopCoroutine(UpdateMethod);
				UpdateMethod = StartCoroutine((loop) ? LoopOnceFull() : KillOnceFull());

				Debug.Log("Manager Init about to return...");
				Debug.Log("CUSTOM DO KIM: manager Initialize() deviceName is: " + deviceName);
				return streamedClip = AudioClip.Create($"Microphone Clip: {deviceName}", ClipSize, Channels, Frequency, false);
			}

			private IEnumerator KillOnceFull()
			{
				int lastPut;

				while (true)
				{
					lastPut = Put;
					yield return null;

					if (lastPut != Put)
					{
						if (lastPut > Put) break;

						streamedClip.SetData(s_clipData, 0);
					}
				}

				End(deviceName);
			}

			private IEnumerator LoopOnceFull()
			{
				int lastPut;

				while (true)
				{
					Debug.Log("Update!!: " + Put);
					lastPut = Put;
					yield return null;

					if (lastPut != Put)
						streamedClip.SetData(s_clipData, 0);
				}
			}
		}
	}
}

#endif