using System.Collections;
using System.Collections.Generic;
using UnityEditor;
[CustomEditor(typeof(BaseAppearButtonEditor))]
public class BaseAppearButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BaseAppearButtonEditor t = (BaseAppearButtonEditor)target;
    }
}
