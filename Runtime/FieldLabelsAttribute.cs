using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

namespace Neonalig.Attributes
{
    public class FieldLabelsAttribute : PropertyAttribute
    {
        /// <summary>
        /// The label for the X field
        /// </summary>
        /// <remarks>Valid for <see cref="bool2"/>, <see cref="bool3"/>, <see cref="bool4"/>, <see cref="Vector2"/>, <see cref="Vector2Int"/>, <see cref="Vector3"/>, <see cref="Vector3Int"/>, <see cref="Vector4"/>, and <see cref="Quaternion"/></remarks>
        public string X { get; set; }

        /// <summary>
        /// The label for the Y field
        /// </summary>
        /// <remarks>Valid for <see cref="bool2"/>, <see cref="bool3"/>, <see cref="bool4"/>, <see cref="Vector2"/>, <see cref="Vector2Int"/>, <see cref="Vector3"/>, <see cref="Vector3Int"/>, <see cref="Vector4"/>, and <see cref="Quaternion"/></remarks>
        public string Y { get; set; }

        /// <summary>
        /// The label for the Z field
        /// </summary>
        /// <remarks>Valid for <see cref="bool3"/>, <see cref="bool4"/>, <see cref="Vector3"/>, <see cref="Vector3Int"/>, <see cref="Vector4"/>, and <see cref="Quaternion"/></remarks>
        public string Z { get; set; }

        /// <summary>
        /// The label for the W field
        /// </summary>
        /// <remarks>Valid for <see cref="bool4"/>, <see cref="Vector4"/>, and <see cref="Quaternion"/></remarks>
        public string W { get; set; }
    }
}
