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

	public override string		name => "Load PrevLevel";

	protected override void Process()
	{
	    obj.SendMessageEx(MessageTitles.scene_loadPrevLevel,
                UniqueNumberBase.GetSavedNumberStatic("SceneManager"),null);
	}
}

[System.Serializable, NodeMenuItem("Game/Load SpecificLevel")]
public class LoadSpecificLevelNode : LinearConditionalNode
{
    [Input(name = "LevelObject")]
	public GraphObjectBase obj;

	[Input(name = "Level Key"),SerializeField]
	public string level;


	public override string		name => "Load SpecificLevel";

	protected override void Process()
	{
		var data = MessageDataPooling.GetMessageData<MD.StringData>();
		data.value = level;

	    obj.SendMessageEx(MessageTitles.scene_loadSpecificLevel,
                UniqueNumberBase.GetSavedNumberStatic("SceneManager"),data);
	}
}

[System.Serializable, NodeMenuItem("Game/Load Out Scene")]
public class LoadOutroNode : LinearConditionalNode
{
    public override string name => "Load Out Scene";

    protected override void Process()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("OutScene");
    }
}