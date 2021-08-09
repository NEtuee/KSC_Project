
public static partial class MessageTitles
{
    public const ushort system_registerRequest = 0x0100;
    public const ushort system_withdrawRequest = 0x0101;
    public const ushort testuimanager_activeimage = 0x2201;
    public const ushort testuimanager_messageTest = 0x2202;

    #region TimeManager
    /// <summary>
    /// data = SetTimeScaleMsg
    /// </summary>
    public const ushort timemanager_settimescale = 0x0201;

    /// <summary>
    /// data = bool
    /// </summary>
    public const ushort timemanager_timestop = 0x0202;

    #endregion

    #region UIManager

    /// <summary>
    /// data = bool
    /// </summary>
    public const ushort uimanager_activecrosshair = 0x0301;

    /// <summary>
    /// data = int(1, 2, 3)
    /// </summary>
    public const ushort uimanager_setcrosshairphase = 0x0302;

    /// <summary>
    /// data = StateBarSetValueType
    /// </summary>
    public const ushort uimanager_setvaluestatebar = 0x0303;

    /// <summary>
    /// data = bool
    /// </summary>
    public const ushort uimanager_setvisibleallstatebar = 0x0304;

    /// <summary>
    /// data = HpPackValueType
    /// </summary>
    public const ushort uimanager_setvaluehppackui = 0x0305;

    /// <summary>
    /// data = string
    /// </summary>
    public const ushort uimanager_settutorialdescription = 0x0306;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort uimanager_fadein = 0x0307;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort uimanager_fadeout = 0x0308;

    /// <summary>
    /// data = bool
    /// </summary>
    public const ushort uimanager_activeloadingui = 0x0309;

    /// <summary>
    /// data = float(0.0 ~ 1.0)
    /// </summary>
    public const ushort uimanager_setloadinggagevalue = 0x0310;

    /// <summary>
    /// data = string
    /// </summary>
    public const ushort uimanager_setloadingtiptext = 0x0311;

    /// <summary>
    /// data = CameraRotateSpeedData
    /// </summary>
    public const ushort uimanager_setvaluecamerarotatespeedslider = 0x0312;

    /// <summary>
    /// data = VolumeData
    /// </summary>
    public const ushort uimanager_setvaluevolumeslider = 0x0313;

    /// <summary>
    /// data = List<string>
    /// </summary>
    public const ushort uimanager_setresolutiondropdown = 0x0314;

    /// <summary>
    /// data = int
    /// </summary>
    public const ushort uimanager_setvalueresolutiondropdown = 0x0315;

    /// <summary>
    /// data = int
    /// </summary>
    public const ushort uimanager_setvaluescreenmodedropdown = 0x0316;

    /// <summary>
    /// data = int
    /// </summary>
    public const ushort uimanager_setvaluevsyncdropdown = 0x0317;

    /// <summary>
    /// data = action
    /// </summary>
    public const ushort uimanager_fadeinout = 0x0318;

    /// <summary>
    /// data = int
    /// </summary>
    public const ushort uimanager_setgunloadvalue = 0x0319;

    /// <summary>
    /// data = float
    /// </summary>
    public const ushort uimanager_setgunchargetimevalue = 0x0320;

    /// <summary>
    /// data = float;
    /// </summary>
    public const ushort uimanager_setgunenergyvalue = 0x0321;

    /// <summary>
    /// data = bool;
    /// </summary>
    public const ushort uimanager_activegunui = 0x0322;

    /// <summary>
    /// data = null;
    /// </summary>
    public const ushort uimanager_getUimanager = 0x0323;

    /// <summary>
    /// data = InGameTutorialCtrl.InGameTutorialType
    /// </summary>
    public const ushort uimanager_activeInGameTutorial = 0x0324;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort uimanager_damageEffect = 0x0325;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort uimanager_activeGameOverUi = 0x0326;
    #endregion

    #region CameraManager

    /// <summary>
    /// data = SetRadialBlurData
    /// </summary>
    public const ushort cameramanager_setradialblur = 0x0401;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort cameramanager_activeplayerfollocamera = 0x0402;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort cameramanager_activeaimcamera = 0x0403;

    /// <summary>
    /// data = float
    /// </summary>
    public const ushort cameramanager_setaimcameradistance = 0x0404;

    /// <summary>
    /// data = ActiveVirtualCameraData
    /// </summary>
    public const ushort cameramanager_activevirtualcamera = 0x0405;

    /// <summary>
    /// data = string(keyName)
    /// </summary>
    public const ushort cameramanager_setfollowcameradistance = 0x0406;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort cameramanager_setzerodamping = 0x0407;

    /// <summary>
    /// data = float(lateTime)
    /// </summary>
    public const ushort cameramanager_restoredamping = 0x0408;

    /// <summary>
    /// data = vector3
    /// </summary>
    public const ushort cameramanager_setdamping = 0x0409;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort cameramanager_generaterecoilimpluse = 0x0410;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort cameramanager_getCameraManager = 0x0411;

    /// <summary>
    /// data = PitchYawData
    /// </summary>
    public const ushort cameramanager_setYawPitch = 0x0412;

    /// <summary>
    /// data = PitchYawPositionData
    /// </summary>
    public const ushort cameramanager_setYawPitchPosition = 0x0413;

    /// <summary>
    /// data = vector3
    /// </summary>
    public const ushort cameramanager_setBrainCameraPosition = 0x0414;

    #endregion

    #region EffectManager

    /// <summary>
    /// data = EffectActiveData
    /// </summary>
    public const ushort effectmanager_activeeffect = 0x0701;

    /// <summary>
    /// data = EffectActiveData
    /// </summary>
    public const ushort effectmanager_activeeffectwithrotation = 0x0702;

    /// <summary>
    /// data = EffectActiveData
    /// </summary>
    public const ushort effectmanager_activeeffectsetparent = 0x0703;

    #endregion

    #region VideoManager

    /// <summary>
    /// data = RawImage
    /// </summary>
    public const ushort videomanager_settargetimage = 0x0801;

    /// <summary>
    /// data = string(key)
    /// </summary>
    public const ushort videomanager_playvideo = 0x0802;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort videomanager_stopvideo = 0x0803;

    #endregion

    #region SettingManager

    /// <summary>
    /// data = CameraRotateSpeedData
    /// </summary>
    public const ushort setting_savecamerarotatespeed = 0x0901;

    /// <summary>
    /// data = VolumeData
    /// </summary>
    public const ushort setting_saveVolume = 0x0902;

    /// <summary>
    /// data = IntData
    /// </summary>
    public const ushort setting_setScreenMode = 0x0903;

    /// <summary>
    /// data = IntData
    /// </summary>
    public const ushort setting_setResolution = 0x0904;

    /// <summary>
    /// data = IntData
    /// </summary>
    public const ushort setting_setVsync = 0x0905;
    #endregion

    #region Player

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort player_initalizemove = 0x0903;

    /// <summary>
    /// data = bool
    /// </summary>
    public const ushort player_visibledrone = 0x0904;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort player_initVelocity = 0x0905;

    #endregion

    #region PlayerManager

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort playermanager_sendplayerctrl = 0x0910;

    /// <summary>
    /// data = PositionRotation
    /// </summary>
    public const ushort playermanager_setPlayerTransform = 0x0911;

    /// <summary>
    /// data = float
    /// </summary>
    public const ushort playermanager_addDamageToPlayer = 0x0912;

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort playermanager_initPlayerStatus = 0x0913;

    #endregion

    #region Setter

    /// <summary>
    /// data = PlayerCtrl_ver2
    /// </summary>
    public const ushort set_setplayer = 0x1001;

    /// <summary>
    /// data = UIManager
    /// </summary>
    public const ushort set_setUimanager = 0x1002;

    /// <summary>
    /// data = CameraManager
    /// </summary>
    public const ushort set_setCameraManager = 0x1003;

    #endregion
}
