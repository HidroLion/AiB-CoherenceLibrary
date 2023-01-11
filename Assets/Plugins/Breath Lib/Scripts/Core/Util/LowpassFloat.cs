// Unity Specific for serialization
using Tooltip = UnityEngine.TooltipAttribute;
using Range = UnityEngine.RangeAttribute;

namespace BreathLib.Util
{
	/// <summary>
	/// Float like struct that applies a low pass filter to the value when updated.
	/// This is used for smoothing out the changes in a value, thus being less sensitive to noise or outliers
	/// </summary>
	[System.Serializable]
	public struct LowpassFloat
	{
		/// <summary>
        /// The current value of the low pass filter.
        /// </summary>
		[Tooltip("The initial/current value of the low pass filter.")]
		public float value;

		/// <summary>
		/// When updating the value, the alpha value is used to determine how much of the new value is used.
		/// </summary>
		/// <example>
		/// init: value = 1.0f, alpha = 0.5f
		/// 1. value = 0.0f => 0.5f
		/// 2. value = 2.0f => 1.5f
		/// 3. value = 5.0f => 3.5f
		/// </example>
		[Range(0, 1)]
		[Tooltip("When updating the value, the alpha value is used to determine how much of the new value is used.")]
		public float lowpassAlpha;

		public LowpassFloat(float value, float alpha)
		{
			this.value = value;
			this.lowpassAlpha = alpha;
		}

		/// <summary>
		/// Internal method to apply the low pass filter.
		/// </summary>
		/// <param name="newValue">The new value to apply.</param>
		/// <param name="alpha">The alpha blend value to use.</param>
		/// <returns>The new value.</returns>
		public float Set(float newValue)
		{
			return value = (value * (1.0f - lowpassAlpha)) + (newValue * lowpassAlpha);
		}

		public override string ToString()
		{
			return $"<LowpassFloat v={value:F2} a={lowpassAlpha:F2}>";
		}

		public static implicit operator float(LowpassFloat lowpassFloat)
		{
			return lowpassFloat.value;
		}

		public static explicit operator LowpassFloat(float value)
		{
			return new LowpassFloat(value, 1.0f);
		}

		public static LowpassFloat operator +(LowpassFloat lowpassFloat, float value)
		{
			lowpassFloat.Set(lowpassFloat.value + value);
			return lowpassFloat;
		}

		public static LowpassFloat operator -(LowpassFloat lowpassFloat, float value)
		{
			lowpassFloat.Set(lowpassFloat.value - value);
			return lowpassFloat;
		}

		public static LowpassFloat operator *(LowpassFloat lowpassFloat, float value)
		{
			lowpassFloat.Set(lowpassFloat.value * value);
			return lowpassFloat;
		}

		public static LowpassFloat operator /(LowpassFloat lowpassFloat, float value)
		{
			lowpassFloat.Set(lowpassFloat.value / value);
			return lowpassFloat;
		}

		public static LowpassFloat operator ++(LowpassFloat lowpassFloat)
		{
			lowpassFloat.Set(lowpassFloat.value + 1.0f);
			return lowpassFloat;
		}

		public static LowpassFloat operator --(LowpassFloat lowpassFloat)
		{
			lowpassFloat.Set(lowpassFloat.value - 1.0f);
			return lowpassFloat;
		}
	}
}