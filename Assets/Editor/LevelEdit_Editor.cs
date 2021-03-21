using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelEdit_Controll))]
public class LevelEdit_Editor : Editor
{
	LevelEdit_Controll controll;

	private string createPath;
	private string[] patternList;

	private float _dist = 0.1f;

	void OnEnable()
    {
        controll = (LevelEdit_Controll)target;
		controll.EditorSetup();
    }

    public override void OnInspectorGUI()
    {
		BeginV();

		var path = controll.GetPointManager().movePaths;
		var pointManager = controll.GetPointManager();
		string deleteTarget = "";

		for(int i = 0; i < path.Count; ++i)
		{
			BeginH();
			GUILayout.Label(path[i].name);

			path[i].isLoop = GUILayout.Toggle(path[i].isLoop,"IsLoop");

			if(GUILayout.Button("Select",GUILayout.Width(100f)))
			{
				pointManager.currentPath = path[i].name;
			}
			else if(GUILayout.Button("Delete",GUILayout.Width(100f)))
			{
				deleteTarget = path[i].name;
				pointManager.currentPath = pointManager.currentPath == path[i].name ? "" : pointManager.currentPath;
			}

			EndH();
		}

		if(deleteTarget != "")
			pointManager.DisposePath(deleteTarget);

		EndV();

		Space(10f);

		BeginH();
		GUILayout.Label("Distance : ");
		_dist = EditorGUILayout.FloatField(_dist);
		EndH();

		BeginH();

		createPath = GUILayout.TextField(createPath);
		if(GUILayout.Button("CreatePath",GUILayout.Width(100f)))
		{
			pointManager.CreatePath(createPath);
			pointManager.currentPath = createPath;
			createPath = "";
		}

		EndH();

		Space(20f);
		GUILayout.Label("CurrentPath : " + pointManager.currentPath);

		DrawPointList();
		Space(10f);
		if(pointManager.currentPath != "")
		{
			DrawControllMenu();
		}


		if(GUILayout.Button("Save"))
		{
			EditorUtility.SetDirty((LevelEdit_Controll)target);
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			
			
			Debug.Log("save");
		}
    }

	void OnSceneGUI()
	{
        AddPointToCursorPoint();
    }

	public void AddPointToCursorPoint()
	{
		Event e = Event.current;

		if(e.type == EventType.KeyDown && 
			e.keyCode == KeyCode.P &&
			controll.GetPointManager().currentPath != "")
		{
			//Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(e.mousePosition);
			Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay( new Vector3( e.mousePosition.x, Screen.height - e.mousePosition.y - 36, 0 ) );
			if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity))
			{
				var point = CreatePoint();
				point.transform.position = hit.point + hit.normal * _dist;
				point.transform.rotation = Quaternion.FromToRotation(point.transform.up,hit.normal);

				controll.AddPoint(controll.GetPointManager().currentPath,point);
			}
			else
			{
				Debug.Log("raycast failed");
			}
		}        
	}

	public void DrawPointList()
	{
		var list = controll.GetPointList(controll.GetPointManager().currentPath);
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
			Selection.activeGameObject = controll.GetPoint(controll.GetPointManager().currentPath,pos).gameObject;
		}
		if(GUILayout.Button("delete",GUILayout.Width(70)))
		{
			controll.DeletePoint(controll.GetPointManager().currentPath,pos);
		}


		EndH();

	}

	private LevelEdit_MovePoint CreatePoint()
	{
		GameObject obj = new GameObject(controll.GetPointManager().currentPath + " : Point");
		var point = obj.AddComponent<LevelEdit_MovePoint>();

		obj.transform.SetParent(controll.transform);

		//point.Initialize();

		return point;
	}

	private void DrawControllMenu()
	{
		BeginV();

		if(GUILayout.Button("AddPoint"))
		{
			controll.AddPoint(controll.GetPointManager().currentPath,CreatePoint());
		}


		EndV();
	}

	// [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    // void OnDrawGizmos() 
    // {
	// 	if(currentPath == "")
	// 		return;

	// 	var list = controll.GetPointManager().GetPath(currentPath);
	// 	if(list == null)
	// 		return;

    //     for(int i = 0; i < list.movePoints.Count; ++i)
    //     {
    //         if(i == 0)
    //             Handles.Label(list.movePoints[i].transform.position, list.name);
    //         else
    //             Handles.Label(list.movePoints[i].transform.position, "p" + i);
    
    //         Handles.color = Color.red;
    //         // Handles.Label(list.movePoints[i].GetBezierPoint1().position, "p" + i + " Bezier_1");
    //         // Handles.Label(list.movePoints[i].GetBezierPoint2().position, "p" + i + " Bezier_2");
    //         Handles.DrawLine(list.movePoints[i].GetPoint(),list.movePoints[i].GetPoint() + Vector3.up * 10f);
    //         // Handles.DrawLine(list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint(),list.movePoints[i].GetBezierPoint2().position);
    //     }

    //     Handles.color = Color.white;

    //     for(int i = 0; i < list.movePoints.Count; ++i)
    //     {
    //         Vector3 startPoint = list.movePoints[i].GetPoint();
    //         Vector3 endPoint = list.movePoints[(i == list.movePoints.Count - 1 ? 0 : i + 1)].GetPoint();

    //         Handles.DrawLine(startPoint,endPoint);

    //     }
    // }

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