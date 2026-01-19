using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using EditorAnimatorController = UnityEditor.Animations.AnimatorController;

namespace Neonalig.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public class AnimatorParameterAttributeDrawer : PropertyDrawer
    {
        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    OnGUI_String(position, property, label);
                    break;
                case SerializedPropertyType.Integer:
                    OnGUI_Integer(position, property, label);
                    break;
                default:
                    position = EditorGUI.PrefixLabel(position, label);
                    EditorGUI.HelpBox(position, "AnimatorParameterAttribute only supports string and int fields.", MessageType.Error);
                    break;
            }
        }

        private static EditorAnimatorController? GetAnimator(SerializedObject serializedObject, string animatorMemberName, [NotNullWhen(false)] out string? error)
        {
            SerializedProperty? animatorProperty = serializedObject.FindProperty(animatorMemberName);
            if (animatorProperty == null)
            {
                error = $"Field '{animatorMemberName}' not found.";
                return null;
            }
            Animator? animator = animatorProperty.objectReferenceValue as Animator;
            if (animator == null)
            {
                error = $"Field '{animatorMemberName}' is not an Animator (Actual type: {animatorProperty.type}).";
                return null;
            }
            if (animator.runtimeAnimatorController is EditorAnimatorController controller)
            {
                error = null;
                return controller;
            }
            error = $"Field '{animatorMemberName}' does not have a RuntimeAnimatorController assigned (Actual: {animator.runtimeAnimatorController?.GetType().Name ?? "<null>"}).";
            return null;
        }

        private static bool TryGetAnimatorParameterFromHash(EditorAnimatorController animator, int hash, [MaybeNullWhen(false)] out AnimatorControllerParameter parameter)
        {
            parameter = null;
            foreach (AnimatorControllerParameter currentParameter in animator.parameters)
            {
                if (currentParameter.nameHash == hash)
                {
                    parameter = currentParameter;
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetAnimatorParameterFromName(EditorAnimatorController animator, string name, [MaybeNullWhen(false)] out AnimatorControllerParameter parameter)
        {
            parameter = null;
            foreach (AnimatorControllerParameter currentParameter in animator.parameters)
            {
                if (currentParameter.name == name)
                {
                    parameter = currentParameter;
                    return true;
                }
            }

            return false;
        }

        private static GenericMenu CreateParameterMenu(EditorAnimatorController animator, SerializedProperty property, bool includeTypeName, Action<SerializedProperty, AnimatorControllerParameter> onParameterSelected)
        {
            GenericMenu menu = new GenericMenu();
            bool any = false;
            foreach (AnimatorControllerParameter currentParameter in animator.parameters)
            {
                any = true;
                var menuItemName = new GUIContent(
                    includeTypeName
                        ? $"{currentParameter.name} ({currentParameter.type})"
                        : currentParameter.name
                );
                menu.AddItem(menuItemName, property.propertyType == SerializedPropertyType.String
                    ? property.stringValue == currentParameter.name
                    : property.intValue == currentParameter.nameHash, () =>
                    {
                        onParameterSelected(property, currentParameter);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                );
            }
            if (!any)
            {
                menu.AddDisabledItem(new GUIContent("No parameters found"));
            }
            return menu;
        }

        private void OnGUI_String(Rect position, SerializedProperty property, GUIContent label)
        {
            AnimatorParameterAttribute? attr = (AnimatorParameterAttribute)attribute;
            var animator = GetAnimator(property.serializedObject, attr.AnimatorMemberName, out string? error);
            if (animator == null)
            {
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.HelpBox(position, error, MessageType.Error);
                return;
            }
            // Debug.Log(animator, animator);
            // throw new Exception();

            string currentParamName;
            if (TryGetAnimatorParameterFromName(animator, property.stringValue, out AnimatorControllerParameter? parameter))
            {
                currentParamName = attr.ParameterType is { } requiredType && parameter.type != requiredType
                    ? $"<TYPE-ERR:{parameter.type}|WAS:{requiredType}|EXP:{parameter.name}>"
                    : parameter.name;
            }
            else
            {
                currentParamName = $"<MISSING:{property.stringValue}>";
            }

            position = EditorGUI.PrefixLabel(position, label);
            if (!GUI.Button(position, currentParamName, EditorStyles.popup)) return;

            GenericMenu menu = CreateParameterMenu(animator, property, attr.ParameterType is null, (prop, param) =>
            {
                prop.stringValue = param.name;
            });
            menu.ShowAsContext();
        }

        private void OnGUI_Integer(Rect position, SerializedProperty property, GUIContent label)
        {
            AnimatorParameterAttribute? attr = (AnimatorParameterAttribute)attribute;
            var animator = GetAnimator(property.serializedObject, attr.AnimatorMemberName, out string? error);
            if (animator == null)
            {
                position = EditorGUI.PrefixLabel(position, label);
                EditorGUI.HelpBox(position, error, MessageType.Error);
                return;
            }

            int hash = property.intValue;
            string currentParamName;
            if (TryGetAnimatorParameterFromHash(animator, hash, out AnimatorControllerParameter? parameter))
            {
                currentParamName = attr.ParameterType is { } requiredType && parameter.type != requiredType
                    ? $"<TYPE-ERR:{parameter.name}|WAS:{parameter.type}|EXP:{requiredType}>"
                    : parameter.name;
            }
            else
            {
                currentParamName = $"<MISSING:{hash}>";
            }

            position = EditorGUI.PrefixLabel(position, label);
            if (!GUI.Button(position, currentParamName, EditorStyles.popup)) return;

            GenericMenu menu = CreateParameterMenu(animator, property, attr.ParameterType is null, (prop, param) =>
            {
                prop.intValue = param.nameHash;
            });
            menu.ShowAsContext();
        }
    }
}
