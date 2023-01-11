using System;
using System.Reflection;
using BreathLib.SerializableInterface;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BreathLib.SerializableInterface
{
    internal class UnityReferenceDrawer : ReferenceDrawer, IReferenceDrawer
    {
        private GUIContent label;

        public void Initialize(SerializedProperty property, Type genericType, GUIContent label, FieldInfo fieldInfo, bool excludesUnityObjects)
        {
            Initialize(property, genericType, fieldInfo, excludesUnityObjects);
            this.label = label;
        }

        public float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public void OnGUI(Rect position)
        {
            Object unityReference = UnityReferenceProperty.objectReferenceValue;
            Type referenceType = unityReference == null ? typeof(Object) : unityReference.GetType();
            GUIContent objectContent = EditorGUIUtility.ObjectContent(unityReference, referenceType);
            CustomObjectDrawer.OnGUI(position, label, objectContent, Property);
            HandleDragAndDrop(position);
        }

        protected override void PingObject(SerializedProperty property)
        {
            EditorGUIUtility.PingObject((Object)GetPropertyValue(property));
        }

        protected override void OnPropertiesClicked(SerializedProperty property)
        {
            PropertyEditorUtility.Show(property.UnityReferenceProperty().objectReferenceValue);
        }
    }
}
