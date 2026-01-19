using System;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Neonalig.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(FieldLabelsAttribute))]
    public sealed class FieldLabelsAttributeDrawer : PropertyDrawer
    {
        private GUIContent XLabel => EditorGUIUtility.TrTextContent(((FieldLabelsAttribute)attribute).X);
        private GUIContent YLabel => EditorGUIUtility.TrTextContent(((FieldLabelsAttribute)attribute).Y);
        private GUIContent ZLabel => EditorGUIUtility.TrTextContent(((FieldLabelsAttribute)attribute).Z);
        private GUIContent WLabel => EditorGUIUtility.TrTextContent(((FieldLabelsAttribute)attribute).W);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool changed;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    property.vector2Value = DrawVector2(position, label, property.vector2Value, XLabel, YLabel, out changed);
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = DrawVector2Int(position, label, property.vector2IntValue, XLabel, YLabel, out changed);
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = DrawVector3(position, label, property.vector3Value, XLabel, YLabel, ZLabel, out changed);
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = DrawVector3Int(position, label, property.vector3IntValue, XLabel, YLabel, ZLabel, out changed);
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = DrawVector4(position, label, property.vector4Value, XLabel, YLabel, ZLabel, WLabel, out changed);
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = DrawQuaternion(position, label, property.quaternionValue, XLabel, YLabel, ZLabel, WLabel, out changed);
                    break;
                default:
                    switch (property.type)
                    {
                        case nameof(bool2):
                            label = EditorGUI.BeginProperty(position, label, property);
                            property.boxedValue = DrawBool2(position, label, (bool2)property.boxedValue, XLabel, YLabel, out changed);
                            EditorGUI.EndProperty();
                            break;
                        case nameof(bool3):
                            label = EditorGUI.BeginProperty(position, label, property);
                            property.boxedValue = DrawBool3(position, label, (bool3)property.boxedValue, XLabel, YLabel, ZLabel, out changed);
                            EditorGUI.EndProperty();
                            break;
                        case nameof(bool4):
                            label = EditorGUI.BeginProperty(position, label, property);
                            property.boxedValue = DrawBool4(position, label, (bool4)property.boxedValue, XLabel, YLabel, ZLabel, WLabel, out changed);
                            EditorGUI.EndProperty();
                            break;
                        default:
                            position = EditorGUI.PrefixLabel(position, label);
                            EditorGUI.HelpBox(position, "FieldLabelsAttribute only supports Vector2, Vector2Int, Vector3, Vector3Int, Vector4, Quaternion, bool2, bool3, and bool4", MessageType.Error);
                            return;
                    }
                    break;
            }

            if (changed)
                property.serializedObject.ApplyModifiedProperties();
        }

        public static Vector2 DrawVector2(Rect position, GUIContent label, Vector2 value, GUIContent xLabel, GUIContent yLabel, out bool changed)
        {
            float[] values = { value.x, value.y };
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(position, label, new[] { xLabel, yLabel }, values);
            changed = EditorGUI.EndChangeCheck();
            return changed ? new Vector2(values[0], values[1]) : value;
        }

        public static Vector2Int DrawVector2Int(Rect position, GUIContent label, Vector2Int value, GUIContent xLabel, GUIContent yLabel, out bool changed)
        {
            int[] values = { value.x, value.y };
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiIntField(position, new[] { xLabel, yLabel }, values);
            changed = EditorGUI.EndChangeCheck();
            return changed ? new Vector2Int(values[0], values[1]) : value;
        }

        public static Vector3 DrawVector3(Rect position, GUIContent label, Vector3 value, GUIContent xLabel, GUIContent yLabel, GUIContent zLabel, out bool changed)
        {
            float[] values = { value.x, value.y, value.z };
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(position, label, new[] { xLabel, yLabel, zLabel }, values);
            changed = EditorGUI.EndChangeCheck();
            return changed ? new Vector3(values[0], values[1], values[2]) : value;
        }

        public static Vector3Int DrawVector3Int(Rect position, GUIContent label, Vector3Int value, GUIContent xLabel, GUIContent yLabel, GUIContent zLabel, out bool changed)
        {
            int[] values = { value.x, value.y, value.z };
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiIntField(position, new[] { xLabel, yLabel, zLabel }, values);
            changed = EditorGUI.EndChangeCheck();
            return changed ? new Vector3Int(values[0], values[1], values[2]) : value;
        }

        public static Vector4 DrawVector4(Rect position, GUIContent label, Vector4 value, GUIContent xLabel, GUIContent yLabel, GUIContent zLabel, GUIContent wLabel, out bool changed)
        {
            float[] values = { value.x, value.y, value.z, value.w };
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(position, label, new[] { xLabel, yLabel, zLabel, wLabel }, values);
            changed = EditorGUI.EndChangeCheck();
            return changed ? new Vector4(values[0], values[1], values[2], values[3]) : value;
        }

        public static Quaternion DrawQuaternion(Rect position, GUIContent label, Quaternion value, GUIContent xLabel, GUIContent yLabel, GUIContent zLabel, GUIContent wLabel, out bool changed)
        {
            float[] values = { value.x, value.y, value.z, value.w };
            EditorGUI.BeginChangeCheck();
            EditorGUI.MultiFloatField(position, label, new[] { xLabel, yLabel, zLabel, wLabel }, values);
            changed = EditorGUI.EndChangeCheck();
            return changed ? new Quaternion(values[0], values[1], values[2], values[3]) : value;
        }

        public static bool2 DrawBool2(Rect position, GUIContent label, bool2 value, GUIContent xLabel, GUIContent yLabel, out bool changed)
        {
            bool[] values = { value.x, value.y };
            MultiToggle(position, label, new[] { xLabel, yLabel }, values, out changed);
            return changed ? new bool2(values[0], values[1]) : value;
        }

        public static bool3 DrawBool3(Rect position, GUIContent label, bool3 value, GUIContent xLabel, GUIContent yLabel, GUIContent zLabel, out bool changed)
        {
            bool[] values = { value.x, value.y, value.z };
            MultiToggle(position, label, new[] { xLabel, yLabel, zLabel }, values, out changed);
            return changed ? new bool3(values[0], values[1], values[2]) : value;
        }

        public static bool4 DrawBool4(Rect position, GUIContent label, bool4 value, GUIContent xLabel, GUIContent yLabel, GUIContent zLabel, GUIContent wLabel, out bool changed)
        {
            bool[] values = { value.x, value.y, value.z, value.w };
            MultiToggle(position, label, new[] { xLabel, yLabel, zLabel, wLabel }, values, out changed);
            return changed ? new bool4(values[0], values[1], values[2], values[3]) : value;
        }

        private const float _toggleWidth = 16f;
        private static void MultiToggle(Rect position, GUIContent label, GUIContent[] subLabels, bool[] values, out bool changed)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= _toggleWidth;

            position = EditorGUI.PrefixLabel(position, label);
            int ln = subLabels.Length;
            if (ln != values.Length)
                throw new ArgumentException("subLabels and values must have the same length", nameof(subLabels));

            float width = position.width / ln;

            EditorGUIUtility.labelWidth = width - _toggleWidth; // Set labelWidth temporarily to available width - toggle width, to avoid the labels being cut off
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < ln; i++)
            {
                values[i] = EditorGUI.ToggleLeft(new Rect(position.x + width * i, position.y, width, position.height), subLabels[i], values[i]);
            }
            changed = EditorGUI.EndChangeCheck();

            EditorGUIUtility.labelWidth = labelWidth;
        }

        // private static void MultiToggle(Rect position, GUIContent label, GUIContent[] subLabels, SerializedProperty valuesIterator, out bool changed)
        // {
        //     EditorGUI.BeginChangeCheck();
        //     EditorGUI.MultiPropertyField(position, subLabels, valuesIterator, label);
        //     changed = EditorGUI.EndChangeCheck();
        // }
    }
}
