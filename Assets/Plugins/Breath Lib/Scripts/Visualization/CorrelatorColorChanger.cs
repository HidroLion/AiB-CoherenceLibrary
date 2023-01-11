using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BreathLib.Unity;
using BreathLib.Core;
using BreathLib.SerializableInterface;

public class CorrelatorColorChanger : MonoBehaviour
{
	[SerializeField, Tooltip("If true, the correlator will begin when this component is enable, and end when disabled. If false, you will need to call Start and Stop manually and all correlator values will not be accessible until Start is called.")]
	private bool ActivateCorrelatorOnEnabled = true;
	public Color MatchingColor;
	public Color NotMatchingColor;
	public SpriteRenderer SpriteRenderer;

	[SerializeField] private SerializableInterface<ICorrelator> _correlator = new();
	public ICorrelator Correlator => _correlator.Value;

	void Update()
    {
		SpriteRenderer.color = Color.Lerp(NotMatchingColor, MatchingColor, Correlator.Correlation);
    }

	private void OnEnable()
	{
		if (ActivateCorrelatorOnEnabled) Correlator.Begin();
	}

	private void OnDisable()
	{
		if (ActivateCorrelatorOnEnabled) Correlator.Stop();
	}

}
