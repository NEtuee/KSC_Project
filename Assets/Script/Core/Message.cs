using System;

public class Message
{
    public short title;
    public int target;

    public Object data;
    public Object sender;

    public void Set(short title, int target, Object data, Object sender)
    {
        this.title = title;
        this.target = target;
        this.data = data;
        this.sender = sender;
    }
}
