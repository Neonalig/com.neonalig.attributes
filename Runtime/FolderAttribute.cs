using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Neonalig.Attributes
{
    /// <summary>
    /// Draws a folder selection field in the Unity editor.
    /// </summary>
    public sealed class FolderPathAttribute : PropertyAttribute
    {
        /// <summary>
        /// Enumeration for the root folder from which the folder path is relative.
        /// </summary>
        public enum FolderRoot
        {
            /// <summary>
            /// The root folder is the file system, meaning the path is absolute and includes the drive letter or root directory.
            /// </summary>
            /// <remarks>
            /// For example,<br/>
            /// <b>Input:</b> <c>&quot;C:/MyGame/Assets/MyFolder/MySubFolder/&quot;</c><br/>
            /// <b>Output:</b> <c>&quot;C:/MyGame/Assets/MyFolder/MySubFolder/&quot;</c>
            /// </remarks>
            FileSystem,
            /// <summary>
            /// The root folder is the Unity project Assets folder.
            /// </summary>
            /// <remarks>
            /// For example,<br/>
            /// <b>Input:</b> <c>&quot;C:/MyGame/Assets/MyFolder/MySubFolder/&quot;</c><br/>
            /// <b>Output:</b> <c>&quot;Assets/MyFolder/MySubFolder/&quot;</c>
            /// </remarks>
            Assets,
            /// <summary>
            /// The root folder is the Unity Resources folder.
            /// </summary>
            /// <remarks>
            /// For example,<br/>
            /// <b>Input:</b> <c>&quot;C:/MyGame/Assets/Resources/MyFolder/MySubFolder/&quot;</c><br/>
            /// <b>Output:</b> <c>&quot;MyFolder/MySubFolder/&quot;</c>
            /// </remarks>
            Resources,
            /// <summary>
            /// The root folder is the Unity StreamingAssets folder.
            /// </summary>
            /// <remarks>
            /// For example,<br/>
            /// <b>Input:</b> <c>&quot;C:/MyGame/Assets/StreamingAssets/MyFolder/MySubFolder/&quot;</c><br/>
            /// <b>Output:</b> <c>&quot;MyFolder/MySubFolder/&quot;</c>
            /// </remarks>
            StreamingAssets,
        }

        /// <summary>
        /// The root folder from which the folder path is relative.
        /// </summary>
        /// <remarks>This is used to enforce a specific form of folder path in the editor.<br/>
        /// For example, if you intend to consume the string via a <see cref="Resources.LoadAll(string)"/> call, this should be set to <see cref="FolderRoot.Resources"/> to ensure the path is relative to the Resources folder.</remarks>
        public FolderRoot Root { get; set; }

        /// <summary>
        /// Enumeration for the requirement of slashes in the folder path.
        /// </summary>
        public enum SlashRequirement
        {
            /// <summary>
            /// The slash is required in the folder path.
            /// </summary>
            Include,
            /// <summary>
            /// The slash is not required in the folder path.
            /// </summary>
            Omit,
            /// <summary>
            /// The slash is optional in the folder path.
            /// </summary>
            Optional
        }

        /// <summary>
        /// Whether the leading slash is required in the folder path.
        /// </summary>
        /// <remarks>
        /// This has no effect if the <see cref="Root"/> is set to <see cref="FolderRoot.Resources"/> or <see cref="FolderRoot.StreamingAssets"/>, as these folders do not require a leading slash.<br/>
        /// Similarly, if the <see cref="Root"/> is set to <see cref="FolderRoot.FileSystem"/>, this is also not required, as the path is absolute and includes the drive letter or root directory.<br/>
        /// However, if the <see cref="Root"/> is set to <see cref="FolderRoot.Assets"/>, the leading slash may be required depending on the consumer of the path, as it may expect a relative path starting from the Assets folder.
        /// </remarks>
        public SlashRequirement LeadingSlashRequirement { get; set; } = SlashRequirement.Omit;

        /// <summary>
        /// Whether the trailing slash is required in the folder path.
        /// </summary>
        /// <remarks>
        /// This has no effect if the <see cref="Root"/> is set to <see cref="FolderRoot.Resources"/> or <see cref="FolderRoot.StreamingAssets"/>, as these folders do not require a trailing slash.<br/>
        /// If the <see cref="Root"/> is set to <see cref="FolderRoot.FileSystem"/> or <see cref="FolderRoot.Assets"/>, the trailing slash may be required to ensure the path is treated as a folder rather than a file, depending on the consumer of the path.
        /// </remarks>
        public SlashRequirement TrailingSlashRequirement { get; set; } = SlashRequirement.Include;

        /// <summary>
        /// Enumeration for the type of slashes used in the folder path.
        /// </summary>
        public enum SlashType
        {
            System,
            Forward,
            Backward,

            Windows = Forward,
            Unix = Backward
        }

        /// <summary>
        /// The type of slashes used in the folder path.
        /// </summary>
        /// <remarks>
        /// This has no effect if the <see cref="Root"/> is set to <see cref="FolderRoot.Resources"/> or <see cref="FolderRoot.StreamingAssets"/>, as these folders enforce the use of forward slashes.
        /// </remarks>
        public SlashType Slashes { get; set; } = SlashType.Forward;

        /// <summary>
        /// Draws a folder selection field in the Unity editor.
        /// </summary>
        /// <remarks>This only provides an editor utility, and has no effect at runtime.</remarks>
        /// <param name="root">The root folder from which the folder path is relative.</param>
        public FolderPathAttribute(FolderRoot root = FolderRoot.Assets)
        {
            Root = root;
        }

        /// <summary>
        /// Normalizes the provided path string according to the attribute settings:
        /// <list type="definition">
        /// <item>
        /// <term>Root Folder</term>
        /// <description>FileSystem: absolute path; Assets: relative to Assets folder; Resources: relative to Resources folder; StreamingAssets: relative to StreamingAssets folder.</description>
        /// </item>
        /// <item>
        /// <term>Slash Type</term>
        /// <description>System: uses the system's directory separator; Forward: uses forward slashes; Backward: uses backward slashes.</description>
        /// </item>
        /// <item>
        /// <term>Leading Slash Requirement</term>
        /// <description>Include: adds a leading slash if not present; Omit: removes any leading slashes; Optional: leaves the leading slash as is.</description>
        /// </item>
        /// <item>
        /// <term>Trailing Slash Requirement</term>
        /// <description>Include: adds a trailing slash if not present; Omit: removes any trailing slashes; Optional: leaves the trailing slash as is.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="path">An absolute or relative folder path (any slash style).</param>
        /// <returns>The cleaned-up, root-relative path, with the correct slash style and optional leading/trailing slashes.</returns>
        [return: NotNullIfNotNull("path")]
        public string? NormalizePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            // 1) unify on forward-slashes for parsing
            string normalized = path.Replace('\\', '/').Trim();

            // 2) strip off the absolute prefix or enforce root-relative
            string relative = Root switch
            {
                FolderRoot.FileSystem => normalized,
                FolderRoot.Assets => NormalizeAssets(normalized),
                FolderRoot.Resources => NormalizeSubfolder(normalized, "Resources"),
                FolderRoot.StreamingAssets => NormalizeSubfolder(normalized, "StreamingAssets"),
                _ => normalized
            };

            // 3) apply your slash style
            relative = (Root is FolderRoot.Resources or FolderRoot.StreamingAssets)
                ? relative // always forward
                : Slashes switch
                {
                    SlashType.Forward => relative.Replace("\\", "/"),
                    SlashType.Backward => relative.Replace("/", "\\"),
                    SlashType.System => relative.Replace("/", Path.DirectorySeparatorChar.ToString()),
                    _ => relative
                };

            // 4) leading slash (only matters for Assets root)
            if (Root == FolderRoot.Assets)
            {
                char lead = Slashes switch
                {
                    SlashType.Backward => '\\',
                    SlashType.System => Path.DirectorySeparatorChar,
                    _ => '/'
                };

                relative = LeadingSlashRequirement switch
                {
                    SlashRequirement.Include when !relative.StartsWith(lead)
                        => lead + relative,
                    SlashRequirement.Omit
                        => relative.TrimStart('/', '\\'),
                    _ => relative
                };
            }

            // 5) trailing slash (FileSystem or Assets only)
            if (Root is FolderRoot.FileSystem or FolderRoot.Assets)
            {
                char trail = Slashes switch
                {
                    SlashType.Backward => '\\',
                    SlashType.System => Path.DirectorySeparatorChar,
                    _ => '/'
                };

                relative = TrailingSlashRequirement switch
                {
                    SlashRequirement.Include when !relative.EndsWith(trail)
                        => relative + trail,
                    SlashRequirement.Omit
                        => relative.TrimEnd('/', '\\'),
                    _ => relative
                };
            }

            return relative;

            //�� helper to strip off "<...>/Resources/" or leading "Resources/"
            static string NormalizeSubfolder(string input, string folderName)
            {
                string marker = $"/{folderName}/";
                int idx = input.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                return idx >= 0
                    ? input[(idx + marker.Length)..]
                    : input.StartsWith($"{folderName}/", StringComparison.OrdinalIgnoreCase)
                        ? input[($"{folderName}/".Length)..]
                        : input;
            }

            //�� helper to strip off "<...>/Assets/" or prefix with "Assets/"
            static string NormalizeAssets(string input)
            {
                const string marker = "/Assets/";
                int idx = input.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                return idx >= 0
                    ? input[(idx + 1)..] // drop the leading slash too
                    : input.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase)
                        ? input
                        : "Assets/" + input.TrimStart('/');
            }
        }

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        private const float _browseButtonWidth = 20f;
        private static GUIContent BrowseButtonContent => EditorGUIUtility.TrIconContent("Folder Icon", "Open a folder selection dialog to choose a folder.");
        private static GUIStyle BrowseButtonStyle => EditorStyles.iconButton;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawFolderPathField(position, label, property, (FolderPathAttribute)attribute);
        }

        public static string DrawFolderPathField(Rect position, string currentPath, FolderPathAttribute attr)
        {
            position.width -= _browseButtonWidth;
            string newPath = EditorGUI.TextField(position, currentPath);
            newPath = attr.NormalizePath(newPath);
            position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
            position.width = _browseButtonWidth;
            if (GUI.Button(position, BrowseButtonContent, BrowseButtonStyle))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", newPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    newPath = attr.NormalizePath(selectedPath);
                }
            }
            return newPath;
        }

        public static string DrawFolderPathField(Rect position, GUIContent label, string currentPath, FolderPathAttribute attr)
        {
            position = EditorGUI.PrefixLabel(position, label);
            return DrawFolderPathField(position, currentPath, attr);
        }

        public static void DrawFolderPathField(Rect position, GUIContent label, SerializedProperty property, FolderPathAttribute attr)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            if (property.propertyType is not SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "FolderPath attribute can only be used on string properties.", MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            position.xMax -= _browseButtonWidth + EditorGUIUtility.standardVerticalSpacing;

            string currentPath = property.stringValue;
            EditorGUI.BeginChangeCheck();
            string newPath = EditorGUI.TextField(position, currentPath);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = attr.NormalizePath(newPath);

            bool b = GUI.Button(new Rect(position.xMax + EditorGUIUtility.standardVerticalSpacing, position.y, _browseButtonWidth, position.height), BrowseButtonContent, BrowseButtonStyle);
            EditorGUI.EndProperty();

            if (b)
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", currentPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    property.stringValue = attr.NormalizePath(selectedPath);
                }
            }
        }
    }
#endif
}
