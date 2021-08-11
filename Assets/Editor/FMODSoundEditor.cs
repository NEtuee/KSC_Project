using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System;
using MD;

public class FMODSoundEditor : EditorWindow
{
    public class ScrollViewMenu
    {
        public Vector2 scroll;

        public Vector2 BeginScrollView(Vector2 sc, float height)
        {
            return EditorGUILayout.BeginScrollView(sc,"box",GUILayout.Height(height - 10f));
        }

        public Vector2 BeginScrollView(Vector2 sc)
        {
            return EditorGUILayout.BeginScrollView(sc,"box",GUILayout.ExpandHeight(true));
        }

        public void BeginScrollView(float height)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll,"box",GUILayout.Height(height - 10f));
        }

        public void BeginScrollView()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll,"box",GUILayout.ExpandHeight(true));
        }

        public void EndScrollView()
        {
            EditorGUILayout.EndScrollView();
        }

        public float VerticalSlider(float value,float min,float max,float width,float height,float alignPer = 0.5f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(width * alignPer - 5f);
            value = GUILayout.VerticalSlider(value,max,min,GUILayout.Width(0f),GUILayout.ExpandHeight(true));
            GUILayout.Space(width * (1f - alignPer) - 5f);
            GUILayout.EndHorizontal();

            return value;
        }

        public virtual void ShowMenu(FMODManager manager, Rect position, System.Object addiData){}
    }

    public class SoundListMesnu : ScrollViewMenu
    {
        public int code = 0;
        Vector2 insideScroll;

        public void SoundList(FMODManager manager,Action<int> buttonPressed)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(200f));
            GUILayout.BeginHorizontal();
            GUILayout.Label("id");
            code = EditorGUILayout.IntField(code);
            if(GUILayout.Button("Add Sound"))
            {
                buttonPressed(code);
            }
            GUILayout.EndHorizontal();

            insideScroll = BeginScrollView(insideScroll);
            var data = manager.infoItem.soundData;

            if(data != null && data.Count >= 1)
            {
                for(int i = 1; i < data.Count; ++i)
                {
                    if(GUILayout.Button(data[i].name))
                    {
                        code = data[i].id;
                    }
                }
            }
            

            EndScrollView();
            GUILayout.EndVertical();
        }

        public override void ShowMenu(FMODManager manager, Rect position, System.Object addiData)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            SoundList(manager,(x)=>{
                var playData = new PlayData();
                playData.code = code;
                manager.startPlayList.Add(playData);
            });

            BeginScrollView();

            GUILayout.BeginHorizontal();

            for(int i = 0; i < manager.startPlayList.Count;)
            {
                var item = manager.startPlayList[i];

                var sound = manager.infoItem.FindSound(item.code);
                if(sound == null)
                {
                    Debug.Log("sound data is not found : " + item);
                    manager.startPlayList.RemoveAt(i);
                    continue;
                }

                GUILayout.BeginVertical(GUILayout.Width(100f));

                if(!DrawSoundItem(sound,100f,position.height - 40f))
                {
                    manager.startPlayList.RemoveAt(i);
                }
                else
                    ++i;

                item.dontStop = GUILayout.Toggle(item.dontStop,"Don't Stop");

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();

            EndScrollView();
            GUILayout.EndHorizontal();
        }

        public bool DrawSoundItem(SoundInfoItem.SoundInfo info, float width, float height, bool showParams = true)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(width),GUILayout.ExpandHeight(true));
            if(GUILayout.Button("delete"))
            {
                return false;
            }

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            GUILayout.Label(info.name,style,GUILayout.Width(width));
            GUILayout.Label("id : " + info.id.ToString(),GUILayout.Width(width));
            GUILayout.Label("type : " + info.type,GUILayout.Width(width));
            GUILayout.Label("volume : " + info.defaultVolume,GUILayout.Width(width));
            GUILayout.Label("attenu : " + info.overrideAttenuation,GUILayout.Width(width));

            if(info.overrideAttenuation)
            {
                GUILayout.Label(" min : " + info.overrideDistance.x ,GUILayout.Width(width));
                GUILayout.Label(" max : " + info.overrideDistance.y,GUILayout.Width(width));
            }

            if(showParams)
            {
                GUILayout.Space(10f);
                GUILayout.Label("parameters",style,GUILayout.Width(width));
                for(int i = 0; i < info.parameters.Count; ++i)
                {
                    GUILayout.Label(info.parameters[i].name,GUILayout.Width(width));
                }
            }
            

            GUILayout.EndVertical();

            return true;
        }
    }

    public class ParameterViewMenu : ScrollViewMenu
    {
        protected SoundInfoItem.SoundInfo _info;
        public ParameterViewMenu(SoundInfoItem.SoundInfo info)
        {
            _info = info;
        }

        public void ShowParameterMenu(FMODManager manager, Rect position)
        {
            BeginScrollView(position.height);
            GUILayout.BeginHorizontal();

            foreach(var item in _info.parameters)
            {
                GUILayout.BeginVertical("box");
                float value = 0f;

                if(EditorApplication.isPlaying)
                {
                    value = manager.GetParameterByName(item.name);
                }

                float modified = value;

                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.white;
                style.wordWrap = false;
        
                GUILayout.Label(item.name,style,GUILayout.Width(100f));
                modified = VerticalSlider(modified,item.min,item.max,100f,position.height - 97f);
                modified = EditorGUILayout.FloatField(modified,GUILayout.Width(100f));

                if(EditorApplication.isPlaying && modified != value)
                {
                    if(_info.id == 0)
                    {
                        manager.SetGlobalParam(item.id,modified);
                    }
                    else
                        manager.SetParam(_info.id,item.id,modified);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            EndScrollView();
        }

        public override void ShowMenu(FMODManager manager, Rect position, System.Object addiData)
        {
            ShowParameterMenu(manager,position);
        }
    }

    public class SoundItemView : ParameterViewMenu
    {
        public class MenuData
        {
            public SoundInfoItem.SoundInfo info;
            public List<FMODUnity.StudioEventEmitter> eventEmitter;
        }

        public List<FMODUnity.StudioEventEmitter> eventEmitter;

        public Vector2 insideScroll;
        public Vector2 playScroll;
        private int _menuSelect;

        private bool _selected = false;

        public SoundItemView(SoundInfoItem.SoundInfo info) : base(info)
        {

            
        }

        public void VolumeSlider(Rect position,float width)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(width));

            var result = eventEmitter[0].EventInstance.getVolume(out var volume);
            if(result != FMOD.RESULT.OK)
                Debug.Log("Volume not exists");
                
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            float modified = volume;
        
            GUILayout.Label("Volume",style,GUILayout.Width(width));
            modified = VerticalSlider(modified,0f,1f,width,position.height - 62f);
            modified = EditorGUILayout.FloatField(modified,GUILayout.Width(width));

            if(volume != modified)
            {
                _info.defaultVolume = modified;
                foreach(var item in eventEmitter)
                {
                    item.EventInstance.setVolume(modified);
                }
                
            }

            GUILayout.EndVertical();
        }

        public void Information(float width)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(width));
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            GUILayout.Label(_info.name,style,GUILayout.Width(width));

            var desc = eventEmitter[0].EventDescription;
            desc.is3D(out bool is3d);
            desc.isOneshot(out bool isOneshot);
            desc.isSnapshot(out bool isSnapshot);

            GUILayout.Label("count : " + eventEmitter.Count,GUILayout.Width(width));
            GUILayout.Label("id : " + _info.id.ToString(),GUILayout.Width(width));
            GUILayout.Label("type : " + _info.type,GUILayout.Width(width));

            GUILayout.Label("3D : " + is3d,GUILayout.Width(width));
            GUILayout.Label("Oneshot : " + isOneshot,GUILayout.Width(width));
            GUILayout.Label("Snapshot : " + isSnapshot,GUILayout.Width(width));

            Options(110f);

            GUILayout.EndVertical();
        }

        public void Options(float width)
        {
            GUILayout.BeginVertical(GUILayout.Width(width),GUILayout.ExpandHeight(true));

            playScroll = BeginScrollView(playScroll);
            
            if(GUILayout.Button("kill all"))
            {
                foreach(var item in eventEmitter)
                {
                    item.Stop();
                }
            }

            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleLeft;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            for(int i = 0; i < eventEmitter.Count; ++i)
            {
                GUILayout.BeginHorizontal();

                if(GUILayout.Button(i.ToString() ,style,GUILayout.Width(30f)))
                {
                    EditorGUIUtility.PingObject(eventEmitter[i].gameObject);
                }
                
                if(GUILayout.Button("kill"))
                {
                    eventEmitter[i].Stop();
                }

                GUILayout.EndHorizontal();
            }

            EndScrollView();

            GUILayout.EndVertical();
        }

        public void PlayingList(float width, FMODManager manager, Rect position)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(width));
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            GUILayout.Label("Now Playing",style,GUILayout.Width(width));

            insideScroll = BeginScrollView(insideScroll, position.height - 30f);
            var active = manager.GetActiveMap();
            var data = manager.infoItem.soundData;

            _selected = false;

            if(active != null)
            {
                foreach(var item in active)
                {
                    if(item.Value.Count > 0)
                    {
                        if(item.Key == _menuSelect)
                        {
                            GUI.enabled = false;
                            _selected = true;
                        }

                        var soundItem = manager.infoItem.FindSound(item.Key);
                        var itemName = soundItem.name;

                        if(GUILayout.Button(itemName))
                        {
                            _menuSelect = item.Key;
                            _selected = true;

                            _info = soundItem;
                            eventEmitter = item.Value;
                        }

                        GUI.enabled = true;
                    }
                }
            }
            

            EndScrollView();

            GUILayout.EndVertical();
        }

        public void ShowParameters(FMODManager manager, Rect position)
        {
            BeginScrollView();
            GUILayout.BeginHorizontal();

            foreach(var item in _info.parameters)
            {
                GUILayout.BeginVertical("box",GUILayout.Width(100f));
                float value = 0f;

                if(EditorApplication.isPlaying)
                {
                    if(eventEmitter[0].EventInstance.getParameterByName(item.name,out var param) != FMOD.RESULT.OK)
                    {
                        Debug.Log("paramter does not exists : " + item.name);
                        value = -1f;
                    }
                    else
                    {
                        value = param;
                    }
                    
                }

                float modified = value;

                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.white;
                style.wordWrap = false;
        
                GUILayout.Label(item.name,style,GUILayout.Width(100f));
                modified = VerticalSlider(modified,item.min,item.max,100f,position.height - 100f);
                modified = EditorGUILayout.FloatField(modified,GUILayout.Width(100f));

                if(EditorApplication.isPlaying && modified != value)
                {
                    manager.SetParam(_info.id,item.id,modified);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();
            EndScrollView();
        }

        public override void ShowMenu(FMODManager manager, Rect position, System.Object addiData)
        {
            PlayingList(200f,manager,position);

            if(_selected)
            {
                //GUILayout.BeginVertical();
                Information(110f);
                //GUILayout.EndVertical();

                VolumeSlider(position,70);
                ShowParameters(manager,position);
            }
        }
    }


    public class SoundPlayerViewMenu : SoundListMesnu
    {
        private Vector3 _soundPosition;
        private Transform _parent;

        public void VolumeSlider(SoundInfoItem.SoundInfo _info, Rect position,float width)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(width));

            var volume = _info.defaultVolume;
                
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            float modified = volume;
        
            GUILayout.Label("Volume",style,GUILayout.Width(width));
            modified = VerticalSlider(modified,0f,1f,width,position.height - 62f);
            modified = EditorGUILayout.FloatField(modified,GUILayout.Width(width));

            if(volume != modified)
            {
                _info.defaultVolume = modified;                
            }

            GUILayout.EndVertical();
        }

        public SoundPlayerViewMenu()
        {
            
        }

        public void PlaySetting()
        {
            GUILayout.BeginVertical("box",GUILayout.Width(100f),GUILayout.ExpandHeight(true));

            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;
            style.wordWrap = false;

            GUILayout.Label("PlaySettings",style,GUILayout.ExpandWidth(true));


            GUILayout.Space(10f);
            GUILayout.Label(_parent == null ? "world" : "local",style,GUILayout.ExpandWidth(true));

            GUILayout.BeginHorizontal();
            GUILayout.Label("x",GUILayout.Width(15f));
            _soundPosition.x = EditorGUILayout.FloatField(_soundPosition.x,GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("y",GUILayout.Width(15f));
            _soundPosition.y = EditorGUILayout.FloatField(_soundPosition.y,GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("z",GUILayout.Width(15f));
            _soundPosition.z = EditorGUILayout.FloatField(_soundPosition.z,GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();


            GUILayout.Space(10f);
            GUILayout.Label("parent",style,GUILayout.ExpandWidth(true));
            _parent = (Transform)EditorGUILayout.ObjectField(_parent,typeof(Transform),true);


            GUILayout.EndVertical();
        }

        public override void ShowMenu(FMODManager manager, Rect position, object addiData)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            SoundList(manager,(x)=>{
                if(manager.playerList.Find((y)=>{return x == y;}) != 0)
                {
                    Debug.Log("key already exists");
                    return;
                }
                manager.playerList.Add(x);
                EditorUtility.SetDirty(manager);
            });

            PlaySetting();

            BeginScrollView();
            GUILayout.BeginHorizontal();

            for(int i = 0; i < manager.playerList.Count;)
            {
                var item = manager.playerList[i];

                var sound = manager.infoItem.FindSound(item);
                if(sound == null)
                {
                    Debug.Log("sound data is not found : " + item);
                    manager.playerList.RemoveAt(i);
                    continue;
                }

                GUILayout.BeginVertical("box",GUILayout.Width(100f));

                if(!DrawSoundItem(sound,100f,position.height - 40f))
                {
                    manager.playerList.RemoveAt(i);
                }
                else
                {
                    ++i;

                    GUIStyle style = new GUIStyle(GUI.skin.box);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.normal.textColor = Color.white;
                    style.wordWrap = false;

                    //GUILayout.BeginVertical(GUILayout.Width(100f));

                    GUILayout.Label("SoundPlay",style,GUILayout.ExpandWidth(true));

                    GUI.enabled = EditorApplication.isPlaying;
                    if(GUILayout.Button("Play"))
                    {
                        Message msg = MessagePool.GetMessage();
                        

                        if(_parent == null)
                        {
                            msg.data = new SoundPlayData(item,Vector3.zero,false);
                            msg.title = MessageTitles.fmod_play;
                            //manager.Play(item,_soundPosition);
                        }
                        else
                        {
                            msg.data = new AttachSoundPlayData(item,Vector3.zero,_parent,false);
                            msg.title = MessageTitles.fmod_play;
                            //manager.Play(item,_soundPosition,_parent);
                        }

                        manager.ReceiveAndProcessMessage(msg);

                    }

                    GUI.enabled = true;

                    
                    
                    //GUILayout.EndVertical();
                }

                GUILayout.EndVertical();

                VolumeSlider(sound,position,70f);
            }

            GUILayout.EndHorizontal();
            EndScrollView();

            GUILayout.EndHorizontal();
        }
    }



    [SerializeField] public FMODManager _target;

    private FMODManager _prevTarget = null;
    private ScrollViewMenu _scrollView = new ScrollViewMenu();
    private ScrollViewMenu _playScrollView = new ScrollViewMenu();

    private int _menuSelect = 0;

    private string[] menuBase = {"StartPlayList","GlobalParameters","PlayingList", "SoundPlayer"};
    private System.Object[] menuData = {null,null,null,null};
    private List<ScrollViewMenu> menus = new List<ScrollViewMenu>();


    [MenuItem("CustomWindow/FMODSoundEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FMODSoundEditor),false,"FMODSoundEditor");
        
    }

    void Init()
    {
        menus.Clear();
        menus.Add(new SoundListMesnu());
        menus.Add(new ParameterViewMenu(_target.infoItem.FindSound(0)));
        menus.Add(new SoundItemView(null));
        menus.Add(new SoundPlayerViewMenu());
    }



    void OnGUI() 
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box",GUILayout.Width(200f),GUILayout.ExpandHeight(true));

        GUILayout.BeginHorizontal();
        _target = EditorGUILayout.ObjectField(_target,typeof(FMODManager),true) as FMODManager;

        if(_target == null)
        {
            var objs = GameObject.FindObjectsOfType<FMODManager>();

            if(objs != null && objs.Length != 0)
            {
                _target = objs[0];
                _prevTarget = null;
            }
        }

        if(GUILayout.Button("refresh"))
        {
            _prevTarget = null;
        }

        GUILayout.EndHorizontal();
        if(_target != _prevTarget && _target != null)
        {
            _prevTarget = _target;
            Init();
        }

        _menuSelect = GUILayout.SelectionGrid(_menuSelect,menuBase,1);

        GUILayout.EndVertical();

        if(_target == null || menus == null || menus.Count == 0)
            return;

        var menuPos = /*_menuSelect >= menuBase.Length ? 2 :*/ _menuSelect;
        menus[menuPos].ShowMenu(_target,position,menuData[menuPos]);


        // _scrollView.BeginScrollView(position.height);


        // GUILayout.BeginHorizontal();

        // for(int i = 0; i < 10; ++i)
        // {
        //     GUILayout.Label("check",GUILayout.Width(100f));
        // }

        // GUILayout.BeginVertical();

        // GUIStyle style = new GUIStyle(GUI.skin.box);
        // style.alignment = TextAnchor.MiddleCenter;
        // style.normal.textColor = Color.white;

        // GUILayout.Label("check",style,GUILayout.Width(100f));
        // testValue = VerticalSlider(testValue,100f,250f,0.7f);
        // testValue = EditorGUILayout.FloatField(testValue);

        // GUILayout.EndVertical();

        // GUILayout.EndHorizontal();

        // _scrollView.EndScrollView();

        GUILayout.EndHorizontal();

    }

    float VerticalSlider(float value,float width,float height,float alignPer = 0.5f)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(width * alignPer);
        value = GUILayout.VerticalSlider(value,1f,0f,GUILayout.Width(0f),GUILayout.Height(height));
        GUILayout.Space(width * (1f - alignPer));
        GUILayout.EndHorizontal();

        return value;
    }
}
