# Neonalig Attributes

Custom Unity `PropertyAttribute`s with editor drawers for enhanced Inspector workflows.

## Features

* **AnimatorParameterAttribute**
  Select Animator parameters from a dropdown, with optional type filtering
* **FieldLabelsAttribute**
  Override field labels for composite values (e.g. `Vector2`, `Vector3`)
* **FolderAttribute**
  Folder picker for string fields

## Usage

### AnimatorParameterAttribute

```csharp
using Neonalig.Attributes;
using UnityEngine;

public class MyAnimator : MonoBehaviour
{
    public Animator animator;

    [AnimatorParameter(nameof(animator))]
    public string parameterName;

    [AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Trigger)]
    public string triggerName;
}
```

### FieldLabelsAttribute

```csharp
using Neonalig.Attributes;
using UnityEngine;

public class MyComponent : MonoBehaviour
{
    [FieldLabels("Start", "End")]
    public Vector2 range;
}
```

## Installation

### Option 1 - Package Manager (Recommended)

1. Open **Window ▸ Package Manager**
2. Click **➕**
3. Select **Install package from Git URL…**
4. Paste:

```
https://github.com/Neonalig/com.neonalig.attributes.git#v1.0.0
```

Supported suffixes:

* `#v1.0.0` – tag
* `#main` – branch
* `#<commit-hash>` – exact commit

> **Tip:** Using a tag or commit hash is recommended for reproducible builds.

---

### Option 2 - `manifest.json`

Add to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.neonalig.attributes": "https://github.com/Neonalig/com.neonalig.attributes.git#v1.0.0"
  }
}
```

---

### Option 3 - Scoped Dependency

If you are consuming this from a local package or a scoped registry, use the package name directly:

```json
{
  "dependencies": {
    "com.neonalig.attributes": "1.0.0"
  }
}
```

### Requirements

* Unity **2021.3 LTS** or newer