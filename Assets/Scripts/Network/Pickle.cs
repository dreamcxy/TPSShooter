
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Pickle
{
    public static byte[] Serialize(object param)
    {
        List<byte> datas = new List<byte>();
        var len = 0;
        byte[] data = null;

        if (param == null)
        {
            len = 0;
        }
        else
        {
            if (param is string) data = Encoding.UTF8.GetBytes((string)param);
            else if (param is byte) data = new byte[] { (byte)param };
            else if (param is bool) data = BitConverter.GetBytes((bool)param);
            else if (param is short) data = BitConverter.GetBytes((short)param);
            else if (param is ushort) data = BitConverter.GetBytes((ushort)param);
            else if (param is int) data = BitConverter.GetBytes((int)param);
            else if (param is uint) data = BitConverter.GetBytes((uint)param);
            else if (param is long) data = BitConverter.GetBytes((long)param);
            else if (param is float) data = BitConverter.GetBytes((float)param);
            else if (param is double) data = BitConverter.GetBytes((double)param);
            else if (param is byte[]) data = (byte[])param;
            else throw new ArgumentException("no object type found...");

            if (data != null) len = data.Length;

        }
        datas.AddRange(BitConverter.GetBytes(len));
        if(len > 0)
        {
            datas.AddRange(data);
        }
        return datas.Count == 0 ? null : datas.ToArray();
    }

}