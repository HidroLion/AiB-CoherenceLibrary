using System;
using System.Collections;
using System.Collections.Generic;
using BreathLib.Core;
using UnityEngine;
using UnityEngine.Events;


namespace BreathLib.Unity
{
	public abstract class CorrelatorComponent<CorrType> : MonoBehaviour, ICorrelator where CorrType : Correlator
	{
		[Header("Component Settings")]
		[SerializeField, Tooltip("If true, the correlator will begin when this component is enable, and end when disabled. If false, you will need to call Start and Stop manually and all correlator values will not be accessible until Start is called.")]
		private bool ActivateOnEnabled = true;

		/// <summary>
        /// Called on update if the correlator is running and the correlation has changed.
        /// </summary>
		public UnityEvent<float> OnCorrelationChanged;

		
		protected abstract ClassTypeReference CorrelatorType { get; }
		
		[Header("Correlator Settings")]
		[SerializeReference] private CorrType Configuration;
		public CorrType Correlator => Configuration;

		public float Correlation => Correlator.Correlation;
		public bool IsRunning => Correlator.IsRunning;

		public void Begin(bool sync = false) => Correlator.Begin(sync);
		public void Stop() => Correlator.Stop();

		protected virtual void OnEnable()
		{
			if (ActivateOnEnabled) Begin();
		}

		protected virtual void OnDisable()
		{
			if (ActivateOnEnabled) Stop();
		}

		private float lastCorrelation = 0;
		protected virtual void Update()
		{
			if (!IsRunning) return;

			if (lastCorrelation != Correlation)
			{
				lastCorrelation = Correlation;
				OnCorrelationChanged?.Invoke(Correlation);
			}
		}


#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			if (CorrelatorType == null || CorrelatorType.Type == null) Configuration = null;
			else if (Configuration == null)
			{
				// If the DetectorType is a type of unity object, we need to create an instance of it using the asset database
				if (CorrelatorType.Type.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					CorrelatorType.Type = null;
					Debug.LogError("Unity objects are not supported as a scriptable detector type. If you are trying to use a unity object which is a component, attach the component to a game object and use that as a detector instead.");
				}
				else Configuration = (CorrType) Activator.CreateInstance(CorrelatorType);
			}
			else if (Configuration.GetType() != CorrelatorType)
			{
				Debug.LogWarning("Detector type and interface type do not match. Detector type will be updated to match the type. If you are trying to change the type, please set the type to null/none first to confirm that you want to change the type REMOVING ALL SERIALIZED DATA.");
				CorrelatorType.Type = Configuration.GetType();
			}
		}

		protected void SetConfiguration<T>() where T : CorrType
		{
			if (Application.isPlaying)
			{
				Debug.LogError("Cannot change the configuration of a correlator while the game is running.");
				return;
			}

			CorrelatorType.Type = typeof(T);
			Configuration = (CorrType) Activator.CreateInstance(typeof(T));
		}
#endif

		public static implicit operator CorrType(CorrelatorComponent<CorrType> correlatorComponent) => correlatorComponent.Correlator;
	}
}