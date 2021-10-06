using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 8,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			if(GUILayout.Button(new GUIContent("PLAY\nMain", "Start Scene Player_Act_Main"), ToolbarStyles.commandButtonStyle))
			{
				SceneHelper.StartScene("Scene_Act_Player_Main");
			}

			if(GUILayout.Button(new GUIContent("PLAY\nGraphic", "Start Scene Player_Act_Graphic"), ToolbarStyles.commandButtonStyle))
			{
				SceneHelper.StartScene("Scene_Act_Player_Graphic");
			}

			GUILayout.Space(10f);

			if(GUILayout.Button(new GUIContent("OPEN\nMain", "Open Scene Player_Act_Main To Single"), ToolbarStyles.commandButtonStyle))
			{
				
				EditorSceneManager.OpenScene("Assets/Scenes/Programmer/Scene_Act_Player_Main.unity");
				
			}

			if(GUILayout.Button(new GUIContent("OPEN\nGraphic", "Open Scene Player_Act_Graphic To Single"), ToolbarStyles.commandButtonStyle))
			{
				EditorSceneManager.OpenScene("Assets/Scenes/Programmer/Scene_Act_Player_Graphic.unity");
			}
		}
	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void StartScene(string sceneName)
		{
			if(EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			sceneToOpen = sceneName;
			EditorApplication.update += OnUpdate;
		}

		static void OnUpdate()
		{
			if (sceneToOpen == null ||
			    EditorApplication.isPlaying || EditorApplication.isPaused ||
			    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
			{
				return;
			}

			EditorApplication.update -= OnUpdate;

			if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				// need to get scene via search because the path to the scene
				// file contains the package version so it'll change over time
				string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
				if (guids.Length == 0)
				{
					Debug.LogWarning("Couldn't find scene file");
				}
				else
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
					EditorSceneManager.OpenScene(scenePath);
					EditorApplication.isPlaying = true;
				}
			}
			sceneToOpen = null;
		}
	}
}