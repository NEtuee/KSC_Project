using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class HexGridPicker : MonoBehaviour
{
    public HexCubeGrid targetGrid;
    public Transform indicator;
}

#if UNITY_EDITOR
[CustomEditor(typeof(HexGridPicker)),CanEditMultipleObjects]
public class HexGridPickerEditor : Editor
{
    public HexGridPicker picker;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        picker = (HexGridPicker)target;

        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    void OnDestroy() 
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView) 
    {
        var mousePosition = Mouse.current.position.ReadValue();
        mousePosition.y = Screen.height - mousePosition.y;
        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePosition);
        var pos = MathEx.PlaneLineIntersect(picker.targetGrid.transform.position,Vector3.up,ray.origin,ray.direction);

        var point = picker.targetGrid.GetCubePointFromWorld(pos);
        var world = picker.targetGrid.CubePointToWorld(point);
        picker.indicator.transform.position = world;

        if(Event.current.keyCode == KeyCode.LeftControl)
        {
            var axial = picker.targetGrid.WorldToAxial(world);

            if(picker.targetGrid.GetCubeFromList(axial) != null)
                return;

            var cube = picker.targetGrid.CreateCube();
            cube.Init(axial.x,axial.y,picker.targetGrid.mapSize,picker.targetGrid.cubeSize);
            cube.transform.position = world;

            picker.targetGrid.AddCubeToList(cube);
        }
    }
}

#endif