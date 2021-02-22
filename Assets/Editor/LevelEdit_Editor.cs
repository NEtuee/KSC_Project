using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelEdit_Controll))]
public class LevelEdit_Editor : Editor
{
	LevelEdit_Controll controll;

	private string currentPath;
	private string createPath;

	private string[] patternList;

	void OnEnable()
    {
        controll = (LevelEdit_Controll)target;
		controll.EditorSetup();
    }

    public override void OnInspectorGUI()
    {
		BeginV();

		var path = controll.GetPointManager().movePaths;
		string deleteTarget = "";

		for(int i = 0; i < path.Count; ++i)
		{
			BeginH();
			GUILayout.Label(path[i].name);

			if(GUILayout.Button("Select",GUILayout.Width(100f)))
			{
				currentPath = path[i].name;
			}
			else if(GUILayout.Button("Delete",GUILayout.Width(100f)))
			{
				deleteTarget = path[i].name;
				currentPath = currentPath == path[i].name ? "" : currentPath;
			}

			EndH();
		}

		if(deleteTarget != "")
			controll.GetPointManager().DisposePath(deleteTarget);

		EndV();

		Space(10f);

		BeginH();

		createPath = GUILayout.TextField(createPath);
		if(GUILayout.Button("CreatePath",GUILayout.Width(100f)))
		{
			controll.GetPointManager().CreatePath(createPath);
			currentPath = createPath;
			createPath = "";
		}

		EndH();

		Space(20f);
		GUILayout.Label("CurrentPath : " + currentPath);

		DrawPointList();
		Space(10f);
		if(currentPath != "")
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
		var list = controll.GetPointList(currentPath);
		if(list == null)
			return;

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
			Selection.activeGameObject = controll.GetPoint(currentPath,pos).gameObject;
		}
		if(GUILayout.Button("delete",GUILayout.Width(70)))
		{
			controll.DeletePoint(currentPath,pos);
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

		if(GUILayout.Button("AddPoint"))
		{
			controll.AddPoint(currentPath,CreatePoint());
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