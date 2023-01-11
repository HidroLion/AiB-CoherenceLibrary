using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualRangeSlider : MonoBehaviour
{
	public Text startText;
	public Text endText;
	public Image[] fillImages;
	public Transform nob;
	public Vector3 startPosition;
	public Vector3 endPosition;
	public Vector2 range = new Vector2(0, 1);

	[SerializeField]
	private float _value = 0.0f;
	[SerializeField]
	private float _enableValue = 1.0f;

	public void SetValue(float value)
	{
		_value = value;
		_Update();
	}

	public void SetEnable(float value)
	{
		_enableValue = value;
		_Update();
	}

	public void _Update()
	{
		float pos = Mathf.Clamp01((_value - range.x) / (range.y - range.x));
		nob.localPosition = Vector3.Lerp(startPosition, endPosition, pos);
		startText.color = new Color(startText.color.r, startText.color.g, startText.color.b, Mathf.Lerp(0.1f, 1f, _enableValue * (1 - pos)));
		endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, Mathf.Lerp(0.1f, 1f, _enableValue * pos));

		for (int i = 0; i < fillImages.Length; i++)
		{
			fillImages[i].color = new Color(fillImages[i].color.r, fillImages[i].color.g, fillImages[i].color.b, Mathf.Lerp(0.1f, 1f, _enableValue));
		}
	}

#if UNITY_EDITOR
	
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			SetValue(_value);
		}
	}
#endif
}
