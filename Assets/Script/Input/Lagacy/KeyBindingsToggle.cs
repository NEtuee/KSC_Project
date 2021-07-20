using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeybindingsToggle", menuName = "KeybindingsToggle")]
public class KeyBindingsToggle : ScriptableObject
{
   [System.Serializable]
   public class SerializeDictKey : SerializeDictionary<KeybindingActions, KeybindingCheckToggle> { }

   public KeybindingCheckToggle[] keybindingChecks;

}
