using System.Runtime.InteropServices;
using UnityEngine;

namespace BreathLib.Native
{
	public static class NativesScriptingSampler
	{
		[DllImport("NATIVEBREATHLIBPLUGIN", EntryPoint = "GetScriptingSamplerWriteIndex")]
		public static extern int GetWriteIndex(int index = 0);

		[DllImport("NATIVEBREATHLIBPLUGIN", EntryPoint = "GetScriptingSamplerLength")]
		public static extern int GetBufferLength(int index = 0);

		[DllImport("NATIVEBREATHLIBPLUGIN", EntryPoint = "SetScriptingSamplerHistoryBuffer")]
		private static extern bool SetScriptingSamplerHistoryBuffer(int index, float[] buffer, int bufferLength);

		[DllImport("NATIVEBREATHLIBPLUGIN", EntryPoint = "CleanUpScriptingSampler")]
		public static extern void CleanUp();

		public static bool SetHistoryBuffer(int index, float[] buffer)
		{
			return SetScriptingSamplerHistoryBuffer(index, buffer, buffer.Length);
		}

		public static bool SetHistoryBuffer(float[] buffer)
		{
			return SetHistoryBuffer(0, buffer);
		}

		[RuntimeInitializeOnLoadMethod]
		static void RunOnStart()
		{
			Application.quitting += OnApplicationQuit;
		}

		private static void OnApplicationQuit()
		{
			CleanUp();
		}
	}
}