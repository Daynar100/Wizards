
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MessageEncoder
{
    private List<byte> data = new List<byte>();
    
    public void Clear()
    {
        data.Clear();
    }
    public byte[] ToArray()
    {
        return GetByteArray();
    }
    public byte[] GetByteArray()
    {
        return data.ToArray();
    }
    public void Encode(MessageType v)
    {
        data.AddRange(System.BitConverter.GetBytes((int)v));
    }
    public void Encode(int v)
    {
        data.AddRange(System.BitConverter.GetBytes(v));
    }
    public void Encode(bool v)
    {
        data.AddRange(System.BitConverter.GetBytes(v));
    }
    public void Encode(string v)
    {
        byte[] vStr = System.Text.UnicodeEncoding.Unicode.GetBytes(v);
        data.AddRange(System.BitConverter.GetBytes(vStr.Length));
        data.AddRange(vStr);
    }
    public void Encode(float v)
    {
        data.AddRange(System.BitConverter.GetBytes(v));
    }
    public void Encode(ulong v)
    {
        data.AddRange(System.BitConverter.GetBytes(v));
    }
    public void Encode(Vector3 v)
    {
        data.AddRange(System.BitConverter.GetBytes(v.x));
        data.AddRange(System.BitConverter.GetBytes(v.y));
        data.AddRange(System.BitConverter.GetBytes(v.z));
    }
    public void Encode(MessageEncoder v)
    {
        Encode(v.GetByteArray());
    }
    public void Encode(byte[] v)
    {
        data.AddRange(v);
    }
}