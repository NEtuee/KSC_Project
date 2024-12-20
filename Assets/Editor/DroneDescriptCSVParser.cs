using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
public class DroneDescriptCSVParser : EditorWindow
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public Object targetDroneDescript = null;


    [MenuItem("CustomWindow/DroneDescriptCSV")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DroneDescriptCSVParser), false, "DroneDescriptCSVParser");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("열기", GUILayout.Width(90)))
        {
            string dir = EditorUtility.OpenFilePanel("ddd", "", "csv");
            if(File.Exists(dir) == true)
            {
                Read(dir);
            }
        }

        targetDroneDescript = EditorGUILayout.ObjectField(targetDroneDescript, typeof(ScriptableObject), false);
    }

    private void Read(string dir)
    {
        string source;
        StreamReader sr = new StreamReader(dir);
        source = sr.ReadToEnd();
        sr.Close();

        List<Descript> descripts = new List<Descript>();
        DroneDescript descriptScriptable = targetDroneDescript as DroneDescript;

        string[] lines = Regex.Split(source, LINE_SPLIT_RE);
        //Debug.Log(lines.Length);
        if (lines.Length <= 1) return;

        string[] header = Regex.Split(lines[0], SPLIT_RE);
        for(int i = 1; i < lines.Length; i++)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            Descript entry = new Descript();
            entry.key = values[0];
            values[1] = string.Join("", values[1].Split('"'));
            entry.descript = values[1];

            bool duplicate = false;
            foreach (var descript in descriptScriptable.descripts)
            {
                if (descript.key == entry.key)
                {
                    duplicate = true;
                    break;
                }
            }
            
            if(duplicate == false)
                descripts.Add(entry);
        }

        //Debug.Log(descripts.Count);

        if(targetDroneDescript != null)
        {
            //DroneDescript descriptScriptable = targetDroneDescript as DroneDescript;
            //descriptScriptable.descripts = new Descript[descripts.Count];

            // for(int i = 0; i<descriptScriptable.descripts.Count;i++)
            // {
            //     descriptScriptable.descripts[i] = descripts[i];
            // }
            
            for(int i = 0; i<descripts.Count;i++)
            {
                bool isNew = true;
                foreach(var dest in descriptScriptable.descripts)
                {
                    if(dest.key == descripts[i].key)
                    {
                        dest.descript = descripts[i].descript;
                        isNew = false;
                        break;
                    }
                }

                if(isNew)
                    descriptScriptable.descripts.Add(descripts[i]);
            }
        }

        EditorUtility.SetDirty(descriptScriptable);
    }
}
