using UnityEngine;
using BreathLib.Core;
using System;

namespace BreathLib.Unity
{
	/// <summary>
	/// A component which allows you to use and set up a correlator in the inspector.
	/// </summary>
	public class BasicCorrelatorComponent : CorrelatorComponent<Correlator>
	{
		[SerializeField, ClassExtends(typeof(Correlator))] protected ClassTypeReference correlatorType;
		protected override ClassTypeReference CorrelatorType => correlatorType;
	}
}