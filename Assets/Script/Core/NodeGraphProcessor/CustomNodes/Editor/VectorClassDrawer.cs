using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VectorClasses;

[CustomPropertyDrawer(typeof(Vector3C))]
public class Vector3CDrawer : PropertyDrawer
{
    // public override VisualElement CreatePropertyGUI(SerializedProperty property)
    // {
    //     // Create property container element.
    //     var container = new VisualElement();

    //     // Create property fields.
    //     var x = new PropertyField(property.FindPropertyRelative("x"));
    //     var y = new PropertyField(property.FindPropertyRelative("y"));
    //     var z = new PropertyField(property.FindPropertyRelative("z"));

    //     x.RegisterValueChangeCallback(e=>{
    //         ValueSet(property);
    //     });

    //     y.RegisterValueChangeCallback(e=>{
    //         ValueSet(property);
    //     });

    //     z.RegisterValueChangeCallback(e=>{
    //         ValueSet(property);
    //     });
        

    //     x.style.flexDirection = new StyleEnum<FlexDirection>() { value = FlexDirection.Row };
    //     y.style.flexDirection = new StyleEnum<FlexDirection>() { value = FlexDirection.Row };
    //     z.style.flexDirection = new StyleEnum<FlexDirection>() { value = FlexDirection.Row };
    //     //container.style.flexDirection = new StyleEnum<FlexDirection>() { value = FlexDirection.Row };

    //     container.Add(x);
    //     container.Add(y);
    //     container.Add(z);

    //     return container;
    // }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUIUtility.labelWidth = 12f;
        float fieldWidth = position.width / 3f;
        position.width /= 3f;
        EditorGUI.PropertyField(position,property.FindPropertyRelative("x"));
        position.x += fieldWidth;
        EditorGUI.PropertyField(position,property.FindPropertyRelative("y"));
        position.x += fieldWidth;
        EditorGUI.PropertyField(position,property.FindPropertyRelative("z"));

    }

    public void ValueSet(SerializedProperty property)
    {
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }
}