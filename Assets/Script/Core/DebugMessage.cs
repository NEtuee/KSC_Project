using System;

public class DebugMessage
{
    public ushort title;
    public int target;

    public bool data;
    public int senderNumber;
    public string senderName;

    public void Set(ushort title, int target, bool data, int senderNumber, string senderName)
    {
        this.title = title;
        this.target = target;
        this.data = data;
        this.senderNumber = senderNumber;
        this.senderName = senderName;
    }
}
