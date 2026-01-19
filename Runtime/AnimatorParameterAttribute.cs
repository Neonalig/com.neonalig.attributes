using System.Diagnostics;
using UnityEngine;

namespace Neonalig.Attributes
{
    /// <summary>
    /// Attribute for selecting an animator parameter in the inspector.
    /// </summary>
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        /// <summary>
        /// The name of the animator serialized field that this attribute is associated with.
        /// </summary>
        public string AnimatorMemberName { get; set; }

        /// <summary>
        /// The type of animator parameter to filter by.
        /// </summary>
        /// <remarks>If <see langword="null"/>, all parameter types are accepted.</remarks>
        public AnimatorControllerParameterType? ParameterType { get; set; }

        /// <summary>
        /// Draws a field for selecting an animator parameter.
        /// </summary>
        /// <param name="animatorMemberName">The name of the animator serialized field that this attribute is associated with.</param>
        public AnimatorParameterAttribute(string animatorMemberName)
        {
            AnimatorMemberName = animatorMemberName;
        }

        /// <summary>
        /// Draws a field for selecting an animator parameter of a specific type.
        /// </summary>
        /// <param name="animatorMemberName">The name of the animator serialized field that this attribute is associated with.</param>
        /// <param name="parameterType">The type of animator parameter to filter by.</param>
        public AnimatorParameterAttribute(string animatorMemberName, AnimatorControllerParameterType parameterType)
        {
            AnimatorMemberName = animatorMemberName;
            ParameterType = parameterType;
        }
    }
}
