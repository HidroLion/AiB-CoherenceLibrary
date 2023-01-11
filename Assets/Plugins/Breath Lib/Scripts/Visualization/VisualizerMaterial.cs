using System.Collections;
using System.Collections.Generic;
using BreathLib.Core;
using BreathLib.SerializableInterface;
using BreathLib.Util;
using UnityEngine;

public class VisualizerMaterial : MonoBehaviour
{
	[SerializeField, Tooltip("The breath stream that is being correlated to a target pattern")]
	private SerializableInterface<IBreathStreamContainer> _breathStream = new(false);
	private BreathStream BreathStream => _breathStream.Value.BreathStream;

	public Material visualizerMaterial;

	private BreathSample _lastSample;
	private float _airInLungs;
	private void FixedUpdate()
	{
		_lastSample = BreathStream.Last;

		if (_lastSample.In.HasValue)
		{
			float intake = _lastSample.In.Value;
			if (_lastSample.Yes.HasValue) intake *= _lastSample.Yes.Value;

			_airInLungs = (_airInLungs * 0.95f) + (intake * 0.05f);
		}
	}

	private void Update()
	{
		if (visualizerMaterial.HasFloat("_No"))
			visualizerMaterial.SetFloat("_No", _lastSample.No ?? -1);
		if (visualizerMaterial.HasFloat("_In"))
			visualizerMaterial.SetFloat("_In", _lastSample.In ?? -1);
		if (visualizerMaterial.HasFloat("_Nasal"))
			visualizerMaterial.SetFloat("_Nasal", _lastSample.Nasal ?? -1);
		if (visualizerMaterial.HasFloat("_Pitch"))
			visualizerMaterial.SetFloat("_Pitch", _lastSample.Pitch ?? -1);
		if (visualizerMaterial.HasFloat("_Volume"))
			visualizerMaterial.SetFloat("_Volume", _lastSample.Volume ?? -1);
		if (visualizerMaterial.HasFloat("_AirInLungs"))
			visualizerMaterial.SetFloat("_AirInLungs", _airInLungs);
	}
}
