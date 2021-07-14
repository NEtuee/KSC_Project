
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
    #endregion
}
