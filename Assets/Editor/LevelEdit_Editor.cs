using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelEdit_Controll))]
public class LevelEdit_Editor : Editor
{
	LevelEdit_Controll controll;

	private string[] patternList;

	void OnEnable()
    {
        controll = (LevelEdit_Controll)target;
		controll.EditorSetup();
    }

    public override void OnInspectorGUI()
    {
		BeginV();
		GUILayout.Label("Behavior");
		controll.behaviorControll = EditorGUILayout.ObjectField(controll.behaviorControll,typeof(LevelEdit_BehaviorControll),true) as LevelEdit_BehaviorControll;
		EndV();

		DrawPointList();
		Space(10f);
		DrawControllMenu();

		if(GUILayout.Button("Save"))
		{
			EditorUtility.SetDirty((LevelEdit_Controll)target);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			
			
			Debug.Log("save");
		}
    }

	public void DrawPointList()
	{
		var list = controll.GetPointList();

		for(int i = 0; i < list.Count; ++i)
		{
			DrawPointItem(list[i],i.ToString(),i);
		}
	}

	private void DrawPointItem(LevelEdit_MovePoint point, string label, int pos)
	{
		GUILayout.BeginHorizontal("box");

		GUILayout.Label(label);
		if(GUILayout.Button("select",GUILayout.Width(70)))
		{
			Selection.activeGameObject = controll.GetPoint(pos).gameObject;
		}
		if(GUILayout.Button("delete",GUILayout.Width(70)))
		{
			controll.DeletePoint(pos);
		}


		EndH();

	}

	private LevelEdit_MovePoint CreatePoint()
	{
		GameObject obj = new GameObject("Point");
		var point = obj.AddComponent<LevelEdit_MovePoint>();

		obj.transform.SetParent(controll.transform);

		point.Initialize();

		return point;
	}

	private void DrawControllMenu()
	{
		BeginV();

		if(GUILayout.Button("Add"))
		{
			controll.AddPoint(CreatePoint());
		}


		EndV();
	}

	public void Space(float space)
	{
		GUILayout.Space(space);
	}

	public void BeginH()
	{
		GUILayout.BeginHorizontal();
	}

	public void EndH()
	{
		GUILayout.EndHorizontal();
	}

	public void BeginV()
	{
		GUILayout.BeginVertical();
	}

	public void EndV()
	{
		GUILayout.EndVertical();
	}
}