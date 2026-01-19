# Neonalig Attributes

Custom Unity PropertyAttributes with editor drawers for enhanced inspector functionality.

## Features

- **AnimatorParameterAttribute**: Select animator parameters from a dropdown in the inspector
- **FieldLabelsAttribute**: Customize field labels in the inspector
- **FolderAttribute**: Select folder paths in the inspector

## Usage

### Animator Parameter Attribute

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

### Field Labels Attribute

```csharp
using Neonalig.Attributes;

public class MyComponent : MonoBehaviour
{
    [FieldLabels("Start", "End")]
    public Vector2 range;
}
```

## Installation

```json
{
  "dependencies": {
    "com.neonalig.attributes": "1.0.0"
  }
}
```
