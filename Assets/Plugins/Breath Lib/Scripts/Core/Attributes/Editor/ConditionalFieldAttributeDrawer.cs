using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace BreathLib.Core
{
	[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
	public class ConditionalFieldAttributeDrawer : PropertyDrawer
	{
		private bool _toShow = true;
		private bool _initialized;
		private PropertyDrawer _customPropertyDrawer;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!(attribute is ConditionalFieldAttribute conditional)) return EditorGUI.GetPropertyHeight(property);
			
			CachePropertyDrawer(property);
			_toShow = ConditionalUtility.IsPropertyConditionMatch(property, conditional.Data);
			if (!_toShow) return -2;

			if (_customPropertyDrawer != null) return _customPropertyDrawer.GetPropertyHeight(property, label);
			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_toShow) return;

			if (!CustomDrawerUsed()) EditorGUI.PropertyField(position, property, label, true);

			
			bool CustomDrawerUsed()
			{
				if (_customPropertyDrawer == null) return false;
				
				try
				{
					_customPropertyDrawer.OnGUI(position, property, label);
					return true;
				}
				catch (Exception e)
				{
					Debug.LogWarning(
						"Unable to use CustomDrawer of type " + _customPropertyDrawer.GetType() + ": " + e,
						property.serializedObject.targetObject);

					return false;
				}
			}
		}
		
		/// <summary>
		/// Try to find and cache any PropertyDrawer or PropertyAttribute on the field
		/// </summary>
		private void CachePropertyDrawer(SerializedProperty property)
		{
			if (_initialized) return;
			_initialized = true;
			if (fieldInfo == null) return;

			var customDrawer = CustomDrawerUtility.GetPropertyDrawerForProperty(property, fieldInfo, attribute);
			if (customDrawer == null) customDrawer = TryCreateAttributeDrawer();

			_customPropertyDrawer = customDrawer;
			
			
			// Try to get drawer for any other Attribute on the field
			PropertyDrawer TryCreateAttributeDrawer()
			{
				var secondAttribute = TryGetSecondAttribute();
				if (secondAttribute == null) return null;
				
				var attributeType = secondAttribute.GetType();
				var customDrawerType = CustomDrawerUtility.GetPropertyDrawerTypeForFieldType(attributeType);
				if (customDrawerType == null) return null;

				return CustomDrawerUtility.InstantiatePropertyDrawer(customDrawerType, fieldInfo, secondAttribute);
				
				
				//Get second attribute if any
				Attribute TryGetSecondAttribute()
				{
					return (PropertyAttribute)fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
						.FirstOrDefault(a => !(a is ConditionalFieldAttribute));
				}
			}
		}
	}
}