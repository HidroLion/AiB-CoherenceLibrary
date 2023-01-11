using UnityEngine;
using BreathLib.Util;

public class NormFloatTester : MonoBehaviour
{
	[SerializeField]
	public NormMappedFloat NormalizedValue;

	private void Reset()
	{
		NormalizedValue = NormMappedFloat.Default();
	}

	private void FixedUpdate()
	{
		float value = 0;
		if (Input.GetKey(KeyCode.Alpha1))
		{
			value += 1;
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			value += 2;
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			value += 3;
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			value += 4;
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			value += 5;
		}
		if (Input.GetKey(KeyCode.Alpha6))
		{
			value += 6;
		}
		if (Input.GetKey(KeyCode.Alpha7))
		{
			value += 7;
		}
		if (Input.GetKey(KeyCode.Alpha8))
		{
			value += 8;
		}
		if (Input.GetKey(KeyCode.Alpha9))
		{
			value += 9;
		}
		if (Input.GetKey(KeyCode.Minus))
		{
			value *= -1;
		}

		NormalizedValue.RawValue = value;
	}
}