using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class BirdyBossPatternEditor : EditorWindow
{
    public BirdyBoss_PatternOne targetPattern;
    public BirdyBoss_PatternOneEditor targetPatternEditor;

    private Vector2 _mainSeqScroll;
    private Vector2 _mainSeqItemScroll;

    private Vector2 _timelineTitleScroll;
    private Vector2 _timelineScroll;

    private static string[] _targetMenuTitles = { "Main", "Loop" };
    private string[] _sequenceEventTitles;
    private string[] _sequenceTitles;

    private int _currentTargetMenu = 0;
    private int _currentSequencer = 0;
    private int _currentEvent = 0;
    private int _currentEventCreate = 0;

    public void Initialize()
    {
        CreateSequenceEventMenu();
        CreateSequenceTitleMenu(0);
    }

    void OnGUI()
    {
        if(EditorApplication.isPlaying)
        {
            GUILayout.Label("Now Playing");
            return;
        }
        else if(targetPattern == null || targetPatternEditor == null)
        {
            var item = GameObject.FindObjectOfType(typeof(BirdyBoss_PatternOne)) as BirdyBoss_PatternOne;
            if(item == null)
            {
                //Close();
                return;
            }

            BirdyBoss_PatternOneEditor[] editors = (BirdyBoss_PatternOneEditor[])Resources.FindObjectsOfTypeAll(typeof(BirdyBoss_PatternOneEditor));
            if(editors.Length == 0)
            {
                //Close();
                return;
            }

            targetPattern = item;
            targetPatternEditor = editors[0];
        }

        if(_sequenceEventTitles == null)
        {
            CreateSequenceEventMenu();
        }

        Event currentEvent = Event.current;
        if(currentEvent.isKey && currentEvent.keyCode == KeyCode.Return)
        {
            GUI.FocusControl("");
            Repaint();
        }


        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical("box", GUILayout.Width(200f));
            {
                GUILayout.BeginHorizontal();
                {
                    var prev = _currentTargetMenu;
                    _currentTargetMenu = EditorGUILayout.IntPopup(_currentTargetMenu, _targetMenuTitles, null);
                    if (prev != _currentTargetMenu)
                    {
                        _mainSeqScroll = Vector3.zero;
                        _currentSequencer = 0;

                        CreateSequenceTitleMenu(_currentTargetMenu);
                    }

                    if (GUILayout.Button("Create", GUILayout.Width(60f)))
                    {
                        CreateNewSequencer(_currentTargetMenu);
                    }
                    if (GUILayout.Button("Delete", GUILayout.Width(60f)))
                    {
                        DeleteSequencer(_currentTargetMenu, _currentSequencer);
                    }
                }
                GUILayout.EndHorizontal();

                _currentSequencer = EditorGUILayout.IntPopup(_currentSequencer, _sequenceTitles, null);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("name");
                if(_sequenceTitles.Length == 0 || _sequenceTitles.Length <= _currentSequencer)
                {
                    GUILayout.Label("Missing");
                }
                else
                {
                    string sequenecName = _sequenceTitles[_currentSequencer];
                    sequenecName = GUILayout.TextField(sequenecName);

                    if (sequenecName != _sequenceTitles[_currentSequencer])
                    {
                        _sequenceTitles[_currentSequencer] = sequenecName;
                        GetTargetSequence(_currentTargetMenu, _currentSequencer).title = sequenecName;
                    }
                }
                
            }
            GUILayout.EndVertical();
            
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical(GUILayout.Width(200f));
            {
                GUILayout.BeginVertical("box");
                {
                    DrawEventCreateMenu(_currentTargetMenu, _currentSequencer, _currentEvent);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
                {
                    DrawEventModifyMenu(_currentTargetMenu, _currentSequencer);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();

                if(GUILayout.Button("��"))
                {
                    var item = GetTargetSequence(_currentTargetMenu, _currentSequencer);
                    if(item != null && item.loopSequences.Count > _currentEvent && _currentEvent > 0)
                    {
                        var one = item.loopSequences[_currentEvent];
                        var two = item.loopSequences[_currentEvent - 1];

                        item.loopSequences[_currentEvent] = two;
                        item.loopSequences[_currentEvent - 1] = one;

                        --_currentEvent;
                        Repaint();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                    
                }
                if(GUILayout.Button("��"))
                {
                    var item = GetTargetSequence(_currentTargetMenu, _currentSequencer);
                    if (item != null && item.loopSequences.Count - 1 > _currentEvent)
                    {
                        var one = item.loopSequences[_currentEvent];
                        var two = item.loopSequences[_currentEvent + 1];

                        item.loopSequences[_currentEvent] = two;
                        item.loopSequences[_currentEvent + 1] = one;

                        ++_currentEvent;
                        Repaint();
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }

                GUILayout.EndHorizontal();
                DrawCurrentSequencers(_currentTargetMenu, _currentSequencer);
            }
            GUILayout.EndVertical();
            
        }
        GUILayout.EndHorizontal();

        DrawTimeLine(_currentTargetMenu);
    }

    void DrawTimeLine(int menu)
    {
        var currMenu = GetMenuSequence(menu);

        GUILayout.BeginHorizontal("box");

        _timelineTitleScroll = GUILayout.BeginScrollView(_timelineTitleScroll,"box", GUILayout.Width(150f), GUILayout.Height(150f));
        {
            for(int i = 0; i < currMenu.Count; ++i)
            {
                GUILayout.Label(currMenu[i].title);
                GUILayout.Space(2f);
            }
        }
        GUILayout.EndScrollView();

        _timelineScroll = GUILayout.BeginScrollView(_timelineScroll, "box", GUILayout.ExpandWidth(true), GUILayout.Height(150f));
        {
            for (int i = 0; i < currMenu.Count; ++i)
            {
                float total = 0f;
                GUILayout.BeginHorizontal();
                {
                    for (int j = 0; j < currMenu[i].loopSequences.Count; ++j)
                    {
                        var item = currMenu[i].loopSequences[j];
                        var time = GetEventTime(item);

                        total += time;
                        if (time > 0f)
                        {
                            if(GUILayout.Button(item.identifier,GUILayout.Width(time * 20f)))
                            {

                            }
                        }

                    }

                }
                GUILayout.EndHorizontal();

                if (total == 0f)
                {
                    GUILayout.Space(15f);
                }
            }
            
        }
        GUILayout.EndScrollView();

        GUILayout.EndHorizontal();

    }

    void DrawEventModifyMenu(int menu, int target)
    {
        var seq = GetTargetSequence(menu, target);
        if(seq == null || seq.loopSequences.Count <= _currentEvent || seq.loopSequences[_currentEvent] == null)
        {
            GUILayout.Label("Missing");
            return;
        }

        DrawEventTarget(seq.loopSequences[_currentEvent]);
    }

    void DrawEventTarget(BirdyBoss_PatternOne.SequenceItem targetEvent)
    {
        EditorGUIUtility.labelWidth = 40f;
        float descHeight = 100f;
        targetEvent.identifier = EditorGUILayout.TextField("Name", targetEvent.identifier);

        var item = targetEvent.type;
        item = (BirdyBoss_PatternOne.EventEnum)EditorGUILayout.IntPopup((int)item, _sequenceEventTitles, null);
        if(item != targetEvent.type)
        {
            targetEvent.type = item;
        }

        string desc = "";
        if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpawnDrone)
        {
            desc = "��� ��ȯ.\nn�ʵ��� m���� ��ȯ��\n��) 10, 5�̸� \n10 / 5 = 2�ʵ��� 5���� ��ȯ\n��ġ -1�� ��� ����\n��ġ�� DroneSpawnPoint�� ����";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
            targetEvent.code = EditorGUILayout.IntField("����", targetEvent.code);
            targetEvent.point = EditorGUILayout.IntField("��ġ", targetEvent.point);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpawnFlySpider)
        {
            desc = "���� �Ź� ��ȯ.\nn�ʵ��� m���� ��ȯ��\n��) 10, 5�̸� \n10 / 5 = 2�ʵ��� 5���� ��ȯ\n";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
            targetEvent.code = EditorGUILayout.IntField("����", targetEvent.code);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpawnSpider)
        {
            desc = "���⼭ ����";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpawnSpiderRandomGrid)
        {
            desc = "���� �Ź� ���� ��ġ ��ȯ.\nn�ʵ��� m���� ��ȯ��\n��) 10, 5�̸� \n10 / 5 = 2�ʵ��� 5���� ��ȯ\n������ �ö��";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
            targetEvent.code = EditorGUILayout.IntField("����", targetEvent.code);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpawnMedusa)
        {
            desc = "Ȧ�� �޵λ� ���� ��ġ ��ȯ.\nn�ʵ��� m���� ��ȯ��\n��) 10, 5�̸� \n10 / 5 = 2�ʵ��� 5���� ��ȯ\n";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
            targetEvent.code = EditorGUILayout.IntField("����", targetEvent.code);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.WaitSeconds)
        {
            desc = "n�� ���";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.AnnihilationFence)
        {
            desc = "���� �Ź�, �����Ź�, �޵λ�\n���� ���";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.InvokeEvent)
        {
            desc = "n�� �ڿ� ����Ƽ �̺�Ʈ ����";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);

            var prop = targetPatternEditor.serializedObject.FindProperty(_currentTargetMenu == 0 ? "sequences" : "loopSequences")
                            .GetArrayElementAtIndex(_currentSequencer)
                            .FindPropertyRelative("loopSequences")
                            .GetArrayElementAtIndex(_currentEvent)
                            .FindPropertyRelative("eventSet");

            EditorGUILayout.PropertyField(prop);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.CutsceneFence)
        {
            desc = "�ƾ� ���� ���";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.DroneAnnihilationFence)
        {
            desc = "���� ��� ���� ���";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.ActiveHPUI)
        {
            desc = "���⼭ �Ⱦ�";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.DeactiveHPUI)
        {
            desc = "���⼭ �Ⱦ�";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.HeadStemp)
        {
            desc = "���� �������� �ĵ�Ÿ��.\nn�� �ڿ� ����\n��ġ 0�� ��� �÷��̾� ������,\n1�� ��� �߾ӿ��� ������\n�ڼ��� ���� ������ BirdyBody��";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
            targetEvent.code = EditorGUILayout.IntField("��ġ", targetEvent.code);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.BirdyApear)
        {
            desc = "���⼭ �Ⱦ�";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpawnFlySpiderBall)
        {
            desc = "���� �Ź� �� ��ȯ.\nn�� �ڿ� ���� ���� ��ġ ��ȯ\n�ڼ��� ������\nFlySpiderBall����������";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.StartFog)
        {
            desc = "���� ���� \nn�� ���� ���� ����";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.EndFog)
        {
            desc = "���� �� \nn�� ���� ���� ����";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.GenieHitGround)
        {
            desc = "n�� �ڿ� ���� ���ͼ� �� ħ\n�ڼ��� ������ Genies��";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("��", targetEvent.value);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.Giro)
        {
            desc = "���̷�\n�ڼ��� ������ ������";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.FallPillar)
        {
            desc = "��� ������\n�ڼ��� ������ ������";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.value = EditorGUILayout.FloatField("����?", targetEvent.value);
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.HorizonPillar)
        {
            desc = "��� ��ġ��\n�ڼ��� ������ ������";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.SpiderPillar)
        {
            desc = "�Ÿ� ���\n�ڼ��� ������ ������";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.GroundCutStart)
        {
            desc = "���� ����\n�ڼ��� ������ PatternOne �� �ؿ�";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else if (targetEvent.type == BirdyBoss_PatternOne.EventEnum.GroundCutEnd)
        {
            desc = "���� ���� ��";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;
        }
        else
        {
            desc = "����??";
            GUI.enabled = false;
            EditorGUILayout.TextArea(desc, GUILayout.Height(descHeight));
            GUI.enabled = true;

            targetEvent.code = EditorGUILayout.IntField("Code",targetEvent.code);
            targetEvent.point = EditorGUILayout.IntField("Point", targetEvent.point);
            targetEvent.value = EditorGUILayout.FloatField("Value", targetEvent.value);

            var prop = targetPatternEditor.serializedObject.FindProperty(_currentTargetMenu == 0 ? "sequences" : "loopSequences")
                            .GetArrayElementAtIndex(_currentSequencer)
                            .FindPropertyRelative("loopSequences")
                            .GetArrayElementAtIndex(_currentEvent)
                            .FindPropertyRelative("eventSet");

            EditorGUILayout.PropertyField(prop);
        }


        
    }

    float GetEventTime(BirdyBoss_PatternOne.SequenceItem item)
    {
        if (item.type == BirdyBoss_PatternOne.EventEnum.SpawnDrone)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.SpawnFlySpider)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.SpawnSpider)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.SpawnSpiderRandomGrid)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.SpawnMedusa)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.WaitSeconds)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.AnnihilationFence)
        {
            return -1f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.InvokeEvent)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.CutsceneFence)
        {
            return -1f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.DroneAnnihilationFence)
        {
            return -1f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.ActiveHPUI)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.DeactiveHPUI)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.HeadStemp)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.BirdyApear)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.SpawnFlySpiderBall)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.StartFog)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.EndFog)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.GenieHitGround)
        {
            return item.value;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.Giro)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.FallPillar)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.HorizonPillar)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.SpiderPillar)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.GroundCutStart)
        {
            return 0f;
        }
        else if (item.type == BirdyBoss_PatternOne.EventEnum.GroundCutEnd)
        {
            return 0f;
        }

        return 0f;
    }

    void DrawEventCreateMenu(int menu, int target, int current)
    {
        GUILayout.BeginHorizontal();
        {
            var seq = GetTargetSequence(menu, target);
            GUI.enabled = seq != null;
            if (GUILayout.Button("Add Event"))
            {
                CreateSequenceItem(menu, target);
            }

            if (GUILayout.Button("Delete Event"))
            {
                DeleteSequenceItem(menu, target, current);
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
        //_currentEvent
    }

    void DrawCurrentSequencers(int menu, int target)
    {
        var targetSeq = GetTargetSequence(menu, target);
        if (targetSeq == null)
            return;

        GUILayout.BeginScrollView(_mainSeqScroll);
        float total = 0f;

        for(int i = 0; i < targetSeq.loopSequences.Count; ++i)
        {
            GUILayout.BeginHorizontal();

            var item = targetSeq.loopSequences[i];

            var time = GetEventTime(item);


            if (time > 0f)
            {
                var color = GUI.color;
                GUI.color = Color.green;
                GUILayout.Label(time + " sec", GUILayout.Width(50f));
                total += time;
                GUI.color = color;
            }
            else if(time == 0f)
            {
                GUILayout.Label(time + " sec", GUILayout.Width(50f));
            }
            else if (time < 0f)
            {
                var color = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("Fence", GUILayout.Width(50f));
                GUI.color = color;
            }

            if (_currentEvent == i)
            {
                GUI.enabled = false;
            }

            if(GUILayout.Button(item.identifier))
            {
                GUI.FocusControl("");
                _currentEvent = i;
            }

            GUI.enabled = true;


            GUILayout.EndHorizontal();
        }

        GUILayout.Label("total : " + total + " sec");
        GUILayout.EndScrollView();
    }

    void CreateSequenceItem(int menu, int target)
    {
        var seq = new BirdyBoss_PatternOne.SequenceItem();
        seq.identifier = "New Event";//_sequenceEventTitles[_currentEventCreate];
        seq.type = 0;
        var t = GetTargetSequence(menu, target).loopSequences;
        _currentEvent = t.Count;
        t.Add(seq);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void DeleteSequenceItem(int menu, int target, int deleteTarget)
    {
        var seq = GetTargetSequence(menu, target);
        if(seq.loopSequences.Count <= deleteTarget)
        {
            Debug.Log("Target out of range");
            return;
        }

        seq.loopSequences.RemoveAt(deleteTarget);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void CreateNewSequencer(int menu)
    {
        var targetSeq = GetMenuSequence(menu);

        var item = new BirdyBoss_PatternOne.LoopSequence();
        item.title = "New Sequencer " + targetSeq.Count;
        item.active = true;

        _currentSequencer = targetSeq.Count;
        targetSeq.Add(item);

        CreateSequenceTitleMenu(menu);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void DeleteSequencer(int menu, int target)
    {
        var targetSeq = GetMenuSequence(menu);
        if(targetSeq.Count <= target || target < 0)
        {
            Debug.Log("target not found");
            return;
        }

        targetSeq.RemoveAt(target);

        CreateSequenceTitleMenu(menu);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    void CreateSequenceTitleMenu(int menu)
    {
        var targetList = GetMenuSequence(menu);

        if(_sequenceTitles == null)
        {
            _sequenceTitles = new string[targetList.Count];
        }
        else
        {
            Array.Resize<string>(ref _sequenceTitles, targetList.Count);
        }

        for(int i = 0; i < targetList.Count; ++i)
        {
            _sequenceTitles[i] = targetList[i].title;
        }
    }

    void CreateSequenceEventMenu()
    {
        int end = (int)BirdyBoss_PatternOne.EventEnum.PatternEND;

        if(_sequenceEventTitles == null)
        {
            _sequenceEventTitles = new string[end];
        }
        else if(_sequenceEventTitles.Length != end)
        {
            Array.Resize<string>(ref _sequenceEventTitles, end);
        }

        for(int i = 0; i < end; ++i)
        {
            _sequenceEventTitles[i] = ((BirdyBoss_PatternOne.EventEnum)i).ToString();
        }
    }

    List<BirdyBoss_PatternOne.LoopSequence> GetMenuSequence(int menu)
    {
        List<BirdyBoss_PatternOne.LoopSequence> targetSeq;

        if (menu == 0)
        {
            targetSeq = targetPattern.sequences;
        }
        else
        {
            targetSeq = targetPattern.loopSequences;
        }

        return targetSeq;
    }

    BirdyBoss_PatternOne.LoopSequence GetTargetSequence(int menu, int target)
    {
        if (GetMenuSequence(menu).Count == 0 || GetMenuSequence(menu).Count <= target)
            return null;
        return GetMenuSequence(menu)[target];
    }
}

[CustomEditor(typeof(BirdyBoss_PatternOne))]
public class BirdyBoss_PatternOneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Show Pattern Editor"))
        {
            var window = EditorWindow.GetWindow(typeof(BirdyBossPatternEditor), false, "Birdy Pattern Editor") as BirdyBossPatternEditor;
            window.targetPattern = (BirdyBoss_PatternOne)target;
            window.targetPatternEditor = this;
            window.Initialize();
            
        }
    }
}