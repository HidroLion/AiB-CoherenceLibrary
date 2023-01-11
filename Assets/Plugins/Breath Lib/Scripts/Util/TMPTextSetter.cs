using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A component which allows you to set the text of a Text component using a variety of types.
/// Useful for using UnityEvents to set the text of a Text component.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TMPTextSetter : MonoBehaviour
{
	[Tooltip("The format string to use when setting the text. {0} will be replaced with the value.")]
	public string format = "Value: {0}";

	private TMP_Text _text;

	private void Awake()
	{
		_text = GetComponent<TMP_Text>();
	}

	public void SetValue(float value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(int value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(string value)
	{
		_text.text = string.Format(format, value);
	}

	public void SetValue(object value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(bool value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Vector2 value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Vector3 value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Vector4 value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Color value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Rect value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Quaternion value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Bounds value)
	{
		_text.text = string.Format(format, value.ToString());
	}

	public void SetValue(Ray value)
	{
		_text.text = string.Format(format, value.ToString());
	}
}
