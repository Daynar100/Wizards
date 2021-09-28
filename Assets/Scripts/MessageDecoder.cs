using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MessageDecoder
{
    private byte[] data;
    private int i = 0;
    
    public bool HasData()
    {
        return (i < data.Length);
    }
    public MessageDecoder(MessageEncoder bytes)
    {
        if (bytes != null)
            data = bytes.GetByteArray();
        else
            data = new byte[0];
    }
    public MessageDecoder(byte[] bytes)
    {
        data = bytes;
    }
    public MessageType ReadMessageType()
    {
        MessageType v = (MessageType)System.BitConverter.ToInt32(data, i);
        i += 4;
        return v;
    }
    public int PeakInt()
    {
        if (HasData())
            return System.BitConverter.ToInt32(data, i);
        else
        {
            return int.MinValue;
        }
    }
    public int ReadInt()
    {
        int v = System.BitConverter.ToInt32(data, i);
        i += 4;
        return v;
    }
    public bool ReadBool()
    {
        return System.BitConverter.ToBoolean(data, i++);
    }
    public string ReadString()
    {
        int l = ReadInt();
        string v = System.Text.Encoding.Unicode.GetString(data, i, l);
        i += l;
        return v;
    }
    public float ReadFloat()
    {
        float v = System.BitConverter.ToSingle(data, i);
        i += 4;
        return v;
    }
    public ulong ReadULong()
    {
        var v = System.BitConverter.ToUInt64(data,i);
        i += 8;
        return v;
    }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadFloat(),ReadFloat(),ReadFloat());
    }

    public byte[] DumpUnread()
    {
        List<byte> l = new List<byte>(data)
                    .GetRange(i, data.Length - i);
        return l.ToArray();
    }
}