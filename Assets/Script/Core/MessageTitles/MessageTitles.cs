
public static partial class MessageTitles
{
    public const ushort system_registerRequest = 0x0100;
    public const ushort system_withdrawRequest = 0x0101;
    public const ushort testuimanager_activeimage = 0x2201;

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

    #region Player

    /// <summary>
    /// data = null
    /// </summary>
    public const ushort player_initalizemove = 0x0901;

    #endregion
}
