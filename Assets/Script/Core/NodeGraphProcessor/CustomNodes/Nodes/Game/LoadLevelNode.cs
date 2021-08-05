using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using VectorClasses;
using NodeGraphProcessor.Examples;

[System.Serializable, NodeMenuItem("Game/Load NextLevel")]
public class LoadNextLevelNode : LinearConditionalNode
{
    [Input(name = "LevelObject")]
	public GraphObjectBase obj;

	public override string		name => "Load NextLevel";

	protected override void Process()
	{
	    obj.SendMessageEx(MessageTitles.scene_loadNextLevel,
            UniqueNumberBase.GetSavedNumberStatic("SceneManager"),null);
	}
}

[System.Serializable, NodeMenuItem("Game/Load PrevLevel")]
public class LoadPrevLevelNode : LinearConditionalNode
{
    [Input(name = "LevelObject")]
	public GraphObjectBase obj;

	public override string		name => "Get Layer";

	protected override void Process()
	{
	    obj.SendMessageEx(MessageTitles.scene_loadPrevLevel,
                UniqueNumberBase.GetSavedNumberStatic("SceneManager"),null);
	}
}