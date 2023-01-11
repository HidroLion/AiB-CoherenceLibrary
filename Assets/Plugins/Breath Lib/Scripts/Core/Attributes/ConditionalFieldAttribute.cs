using System;
using System.Linq;
using UnityEngine;

namespace BreathLib.Core
{
	/// <summary>
	/// Conditionally Show/Hide field in inspector, based on some other field or property value
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		public bool IsSet => Data != null && Data.IsSet;
		public readonly ConditionalData Data;

		/// <param name="fieldToCheck">String name of field to check value</param>
		/// <param name="inverse">Inverse check result</param>
		/// <param name="compareValues">On which values field will be shown in inspector</param>
		public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
			=> Data = new ConditionalData(fieldToCheck, inverse, compareValues);

		
		public ConditionalFieldAttribute(string[] fieldToCheck, bool[] inverse = null, params object[] compare)
			=> Data = new ConditionalData(fieldToCheck, inverse, compare);

		public ConditionalFieldAttribute(params string[] fieldToCheck) => Data = new ConditionalData(fieldToCheck);
		public ConditionalFieldAttribute(bool useMethod, string method, bool inverse = false) 
			=> Data = new ConditionalData(useMethod, method, inverse);
	}
}