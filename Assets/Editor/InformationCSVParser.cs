using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class InformationCSVParser : EditorWindow
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public Object infoData = null;


    [MenuItem("CustomWindow/InformationCSV")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(InformationCSVParser), false, "InformationCSVParser");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("¿­±â", GUILayout.Width(90)))
        {
            string dir = EditorUtility.OpenFilePanel("ddd", "", "csv");
            if (File.Exists(dir) == true)
            {
                Read(dir);
            }
        }

        infoData = EditorGUILayout.ObjectField(infoData, typeof(ScriptableObject), false);
    }

    private void Read(string dir)
    {
        string source;
        StreamReader sr = new StreamReader(dir);
        source = sr.ReadToEnd();
        sr.Close();

        List<InfomationText> info = new List<InfomationText>();
        InformationScriptable informationScriptable = infoData as InformationScriptable;

        string[] lines = Regex.Split(source, LINE_SPLIT_RE);
        //Debug.Log(lines.Length);
        if (lines.Length <= 1) return;

        string[] header = Regex.Split(lines[0], SPLIT_RE);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            InfomationText entry = new InfomationText();
            entry.key = values[0];
            values[1] = string.Join("", values[1].Split('"'));
            entry.keyboardMouse = values[1];
            entry.gamepad = values[2];

            bool duplicate = false;
            foreach (var curInfo in informationScriptable.data)
            {
                if (curInfo.key == entry.key)
                {
                    duplicate = true;
                    break;
                }
            }

            if (duplicate == false)
                info.Add(entry);
        }

        //Debug.Log(descripts.Count);

        if (infoData != null)
        {
            //DroneDescript descriptScriptable = targetDroneDescript as DroneDescript;
            //descriptScriptable.descripts = new Descript[descripts.Count];

            // for(int i = 0; i<descriptScriptable.descripts.Count;i++)
            // {
            //     descriptScriptable.descripts[i] = descripts[i];
            // }

            for (int i = 0; i < info.Count; i++)
            {
                bool isNew = true;
                foreach (var dest in informationScriptable.data)
                {
                    if (dest.key == info[i].key)
                    {
                        dest.keyboardMouse = info[i].keyboardMouse;
                        dest.gamepad = info[i].gamepad;
                        Debug.Log(dest.gamepad + " " + dest.keyboardMouse);
                        isNew = false;
                        break;
                    }
                }

                if (isNew)
                    informationScriptable.data.Add(info[i]);
            }
        }

        EditorUtility.SetDirty(informationScriptable);
    }
}
