using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MessageReceiver),true)]
public class MessageReceiverEditor : Editor
{
    protected MessageReceiver messageControl;

    const string uniqueNumberField_Text = "Unique Number : ";
    const string sendedQueueField_Text = "Sended Messages";
    const string receivedQueueField_Text = "Received Messages";

    public virtual void OnEnable()
    {
        messageControl = (MessageReceiver)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    
        if(!messageControl.allowDebugMode)
            return;

        EditorGUILayout.Space();

        if(!EditorApplication.isPlaying)
        {
            GUILayout.Label("DebugArea");
            return;
        }

        GUILayout.Label(uniqueNumberField_Text + messageControl.uniqueNumber);

        if(EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            GUILayout.Label("isPlaying");
            return;
        }

        DrawUILine(Color.gray);

        GUILayout.Label(sendedQueueField_Text);
        GUILayout.BeginVertical("box");
        foreach(var msg in messageControl.sendedQueue)
        {
            DrawMessageInfo(msg);
        }
        GUILayout.EndVertical();

        DrawUILine(Color.gray);

        GUILayout.Label(receivedQueueField_Text);
        GUILayout.BeginVertical("box");
        foreach(var msg in messageControl.receivedQueue)
        {
            DrawMessageInfo(msg);
        }
        GUILayout.EndVertical();
    }

    public void DrawMessageInfo(DebugMessage msg)
    {
        string title = msg.title.ToString("X4");
        string target = msg.target.ToString();
        string dataExists = msg.data ? "exists" : "null";
        string sender = msg.senderNumber + ":" + msg.senderName;

        string label = title + " _ " + target + " _ " + dataExists + " _ " + sender;
    
        GUILayout.Label(label);
    }

    public static void DrawUILine(Color color, int thickness = 1, int padding = 8)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
        r.height = thickness;
        r.y+=padding/2;
        r.x-=2;
        r.width +=6;
        EditorGUI.DrawRect(r, color);
    }
}
