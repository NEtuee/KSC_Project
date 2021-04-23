using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KeyCustomItem : MonoBehaviour
{
    public KeybindingActions action;
    public InputType inputType;

    public abstract void Initialize(InputType inputType);
}
