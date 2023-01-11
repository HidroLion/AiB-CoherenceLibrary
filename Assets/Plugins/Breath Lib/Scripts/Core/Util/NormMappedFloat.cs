using System;
using System.Collections.Generic;
using System.Linq;

namespace BreathLib.Util
{
	[Serializable]
	public struct NormMappedFloat
	{
		public enum RangeUpdateMode
		{
			NumberAssignments,
			// TODO: implement this
			// SecondsPassed,
		}

		[Serializable]
		public struct Config
		{
			public static Config Default()
			{
				return new Config(0.01f);
			}

			public Config(float updateAlpha = 0.01f, float fixedMin = float.MinValue, float fixedMax = float.MaxValue, int historyLength = 5, RangeUpdateMode updateMode = RangeUpdateMode.NumberAssignments, bool debugValues = false)
			{
				this.updateAlpha = updateAlpha;
				this.fixedMin = fixedMin;
				this.fixedMax = fixedMax;
				this.historyLength = historyLength;
				this.updateMode = updateMode;
				this.debugValues = debugValues;
			}

			[UnityEngine.Tooltip("The alpha value used to update the range. (max -= (max - new) * (1 - alpha)")]
			public float updateAlpha;
			[UnityEngine.Tooltip("The minimum value of the range. If the raw value is lower than this, it will be set to this.")]
			public float fixedMin;
			[UnityEngine.Tooltip("The maximum value of the range. If the raw value is higher than this, it will be set to this.")]
			public float fixedMax;
			[UnityEngine.Tooltip("The number of previous values to keep in history (x2, one for min and one for max)")]
			public int historyLength;
			[UnityEngine.Tooltip("The mode used to update the range. NumberAssignments will update the range every time a new value is assigned. SecondsPassed will update the range for every time a second passes.")]
			public RangeUpdateMode updateMode;
			[UnityEngine.Tooltip("Whether to print the current value and the range to the console on an update.")]
			public bool debugValues;
		}

		public static NormMappedFloat Default()
		{
			return new NormMappedFloat(new Config());
		}

		public NormMappedFloat(Config config)
		{
			this.config = config;
			if (config.historyLength <= 0)
			{
				this.config = Config.Default();
			}
			_maxHistory = new List<float>();
			_minHistory = new List<float>();
			for (int i = 0; i < config.historyLength; i++)
			{
				_maxHistory.Add(config.fixedMin);
				_minHistory.Add(config.fixedMax);
			}
			_value = config.fixedMin;
			NormValue = 0;
		}

		[UnityEngine.SerializeField]
		private Config config;

		private List<float> _maxHistory;
		private List<float> _minHistory;

		public float DynamicMax {
			get {
				if (_maxHistory == null || _minHistory == null) InitDefaults();
				return Math.Max(_maxHistory[_maxHistory.Count - 1], _minHistory[_minHistory.Count - 1]);
			}
		}
		public float DynamicMin {
			get {
				if (_maxHistory == null || _minHistory == null) InitDefaults();
				return Math.Min(_maxHistory[0], _minHistory[0]);
			}
		}

		public float NormValue { get; private set; }

		[NonSerialized]
		private float _value;
		public float RawValue
		{
			get => _value;
			set
			{
				if (_maxHistory == null || _minHistory == null) InitDefaults();
				_value = Math.Clamp(value, config.fixedMin, config.fixedMax);

				if (_value < _maxHistory[_maxHistory.Count - 1])
				{
					_maxHistory[_maxHistory.Count - 1] -= (_maxHistory[_maxHistory.Count - 1] - _value) * config.updateAlpha;
				}
				if (_value > _maxHistory[0])
				{
					_maxHistory[0] = _value;
				}
				_maxHistory.Sort();

				if (_value > _minHistory[0])
				{
					_minHistory[0] += (_value - _minHistory[0]) * config.updateAlpha;
				}
				if (_value < _minHistory[_minHistory.Count - 1])
				{
					_minHistory[_minHistory.Count - 1] = _value;
				}
				_minHistory.Sort();

				NormValue = (_value - DynamicMin) / (DynamicMax - DynamicMin);

#if UNITY_EDITOR
				if (config.debugValues)
				{
					UnityEngine.Debug.Log($"Raw: {_value}, Norm: {NormValue}, Range: ({DynamicMin}, {DynamicMax}),\nMaxHist: [{String.Join(",", _maxHistory.Select(x => string.Format("{0:0.0}", x)))}], MinHist: [{String.Join(",", _minHistory.Select(x => string.Format("{0:0.0}", x)))}]");
				}
#endif
			}
		}

		private void InitDefaults()
		{
			if (config.historyLength == 0) config = new Config(historyLength: 5);
			System.Diagnostics.Debug.Assert(config.historyLength > 0 && config.updateAlpha > 0.001 && config.updateAlpha <= 0.999 && _maxHistory == null && _minHistory == null);
			_maxHistory = new List<float>();
			_minHistory = new List<float>();
			for (int i = 0; i < config.historyLength; i++)
			{
				_maxHistory.Add(config.fixedMin);
				_minHistory.Add(config.fixedMax);
			}
		}
	}
}