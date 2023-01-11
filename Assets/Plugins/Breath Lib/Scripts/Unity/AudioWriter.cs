using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioWriter : MonoBehaviour
{

	private void Start()
	{
		// Grab the GetFloatBufferCallback from the audio effect

		float[] buffer = new float[1024];
		AudioListener.GetSpectrumData(buffer, 0, FFTWindow.Rectangular);
	}
}
