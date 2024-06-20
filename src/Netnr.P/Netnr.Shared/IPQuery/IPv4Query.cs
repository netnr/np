#if Full || IPQuery

namespace Netnr;

/// <summary>
/// IPv4
/// </summary>
public class IPv4Query
{
    ///<summary>
    ///第一种模式
    ///</summary>
    const byte REDIRECT_MODE_1 = 0x01;

    ///<summary>
    ///第二种模式
    ///</summary>
    const byte REDIRECT_MODE_2 = 0x02;

    ///<summary>
    ///每条记录长度
    ///</summary>
    const int IP_RECORD_LENGTH = 7;

    ///<summary>
    ///文件对象
    ///</summary>
    FileStream DataStream { get; set; }

    ///<summary>
    ///索引开始位置
    ///</summary>
    long IPBegin { get; set; }

    ///<summary>
    ///索引结束位置
    ///</summary>
    long IPEnd { get; set; }

    ///<summary>
    /// IP对象
    ///</summary>
    IPQueryResult Result { get; set; }

    ///<summary>
    ///存储文本内容
    ///</summary>
    byte[] Buf { get; set; }

    ///<summary>
    ///存储4字节IP地址
    ///</summary>
    byte[] B4 { get; set; }

    ///<summary>
    ///构造函数
    ///</summary>
    ///<param name="dbFile">IP数据库文件绝对路径</param>
    public IPv4Query(string dbFile)
    {
        Buf = new byte[500];
        B4 = new byte[4];

        DataStream = new FileStream(dbFile, FileMode.Open);

        IPBegin = ReadLong4(0);
        IPEnd = ReadLong4(4);
        Result = new IPQueryResult();
    }

    ///<summary>
    ///搜索IP地址搜索
    ///</summary>
    ///<param name="strIP"></param>
    ///<returns></returns>
    public IPQueryResult Search(string strIP)
    {
        //将字符IP转换为字节
        string[] ipSp = strIP.Split('.');
        byte[] IP = new byte[4];
        for (int i = 0; i < IP.Length; i++)
        {
            IP[i] = (byte)(int.Parse(ipSp[i]) & 0xFF);
        }

        IPQueryResult local = null;
        long offset = LocateIP(IP);

        if (offset != -1)
        {
            local = GetIPLocation(offset);
        }

        return local;
    }

    ///<summary>
    ///取得具体信息
    ///</summary>
    ///<param name="offset"></param>
    ///<returns></returns>
    IPQueryResult GetIPLocation(long offset)
    {
        DataStream.Position = offset + 4;
        //读取第一个字节判断是否是标志字节
        byte one = (byte)DataStream.ReadByte();
        if (one == REDIRECT_MODE_1)
        {
            //第一种模式
            //读取国家偏移
            long countryOffset = ReadLong3();
            //转至偏移处
            DataStream.Position = countryOffset;
            //再次检查标志字节
            byte b = (byte)DataStream.ReadByte();
            if (b == REDIRECT_MODE_2)
            {
                Result.Addr = ReadString(ReadLong3());
                DataStream.Position = countryOffset + 4;
            }
            else
                Result.Addr = ReadString(countryOffset);

            //读取地区标志
            Result.ISP = ReadArea(DataStream.Position);

        }
        else if (one == REDIRECT_MODE_2)
        {
            //第二种模式
            Result.Addr = ReadString(ReadLong3());
            Result.ISP = ReadArea(offset + 8);
        }
        else
        {
            //普通模式
            Result.Addr = ReadString(--DataStream.Position);
            Result.ISP = ReadString(DataStream.Position);
        }
        return Result;
    }

    ///<summary>
    ///读取地区名称
    ///</summary>
    ///<param name="offset"></param>
    ///<returns></returns>
    string ReadArea(long offset)
    {
        DataStream.Position = offset;
        byte one = (byte)DataStream.ReadByte();
        if (one == REDIRECT_MODE_1 || one == REDIRECT_MODE_2)
        {
            long areaOffset = ReadLong3(offset + 1);
            if (areaOffset == 0)
                return "";
            else
            {
                return ReadString(areaOffset);
            }
        }
        else
        {
            return ReadString(offset);
        }
    }

    ///<summary>
    ///读取字符串（GBK）
    ///</summary>
    ///<param name="offset"></param>
    ///<returns></returns>
    string ReadString(long offset)
    {
        DataStream.Position = offset;

        int i = 0;
        byte b = (byte)DataStream.ReadByte();
        while (b != 0 && i < Buf.Length)
        {
            Buf[i++] = b;
            b = (byte)DataStream.ReadByte();
        }

        if (i > 0 && i != Buf.Length)
        {
            return Encoding.GetEncoding("gbk").GetString(Buf, 0, i);
        }
        return "";
    }

    ///<summary>
    ///查找IP地址所在的绝对偏移量
    ///</summary>
    ///<param name="ip"></param>
    ///<returns></returns>
    long LocateIP(byte[] ip)
    {
        long m = 0;
        int r;

        //比较第一个IP项
        ReadIP(IPBegin, B4);
        r = CompareIP(ip, B4);
        if (r == 0)
            return IPBegin;
        else if (r < 0)
            return -1;
        //开始二分搜索
        for (long i = IPBegin, j = IPEnd; i < j;)
        {
            m = GetMiddleOffset(i, j);
            ReadIP(m, B4);
            r = CompareIP(ip, B4);
            if (r > 0)
                i = m;
            else if (r < 0)
            {
                if (m == j)
                {
                    j -= IP_RECORD_LENGTH;
                    m = j;
                }
                else
                {
                    j = m;
                }
            }
            else
                return ReadLong3(m + 4);
        }
        m = ReadLong3(m + 4);
        ReadIP(m, B4);
        r = CompareIP(ip, B4);
        if (r <= 0)
            return m;
        else
            return -1;
    }

    ///<summary>
    ///从当前位置读取四字节,此四字节是IP地址
    ///</summary>
    ///<param name="offset"></param>
    ///<param name="ip"></param>
    void ReadIP(long offset, byte[] ip)
    {
        DataStream.Position = offset;
        DataStream.Read(ip, 0, ip.Length);
        byte tmp = ip[0];
        ip[0] = ip[3];
        ip[3] = tmp;
        tmp = ip[1];
        ip[1] = ip[2];
        ip[2] = tmp;
    }

    ///<summary>
    ///比较IP地址是否相同
    ///</summary>
    ///<param name="ip"></param>
    ///<param name="beginIP"></param>
    ///<returns>0:相等,1:ip大于beginIP,-1:小于</returns>
    static int CompareIP(byte[] ip, byte[] beginIP)
    {
        for (int i = 0; i < 4; i++)
        {
            int r = CompareByte(ip[i], beginIP[i]);
            if (r != 0)
                return r;
        }
        return 0;
    }

    ///<summary>
    ///比较两个字节是否相等
    ///</summary>
    ///<param name="bsrc"></param>
    ///<param name="bdst"></param>
    ///<returns></returns>
    static int CompareByte(byte bsrc, byte bdst)
    {
        if ((bsrc & 0xFF) > (bdst & 0xFF))
            return 1;
        else if ((bsrc ^ bdst) == 0)
            return 0;
        else
            return -1;
    }

    ///<summary>
    ///从当前位置读取4字节,转换为长整型
    ///</summary>
    ///<param name="offset"></param>
    ///<returns></returns>
    long ReadLong4(long offset)
    {
        long ret = 0;
        DataStream.Position = offset;
        ret |= (long)(DataStream.ReadByte() & 0xFF);
        ret |= (long)((DataStream.ReadByte() << 8) & 0xFF00);
        ret |= (long)((DataStream.ReadByte() << 16) & 0xFF0000);
        ret |= ((DataStream.ReadByte() << 24) & 0xFF000000);
        return ret;
    }

    ///<summary>
    ///根据当前位置,读取3字节
    ///</summary>
    ///<param name="offset"></param>
    ///<returns></returns>
    long ReadLong3(long offset)
    {
        long ret = 0;
        DataStream.Position = offset;
        ret |= (long)(DataStream.ReadByte() & 0xFF);
        ret |= (long)((DataStream.ReadByte() << 8) & 0xFF00);
        ret |= (long)((DataStream.ReadByte() << 16) & 0xFF0000);
        return ret;
    }

    ///<summary>
    ///从当前位置读取3字节
    ///</summary>
    ///<returns></returns>
    long ReadLong3()
    {
        long ret = 0;
        ret |= (long)(DataStream.ReadByte() & 0xFF);
        ret |= (long)((DataStream.ReadByte() << 8) & 0xFF00);
        ret |= (long)((DataStream.ReadByte() << 16) & 0xFF0000);
        return ret;
    }

    ///<summary>
    ///取得begin和end中间的偏移
    ///</summary>
    ///<param name="begin"></param>
    ///<param name="end"></param>
    ///<returns></returns>
    static long GetMiddleOffset(long begin, long end)
    {
        long records = (end - begin) / IP_RECORD_LENGTH;
        records >>= 1;
        if (records == 0)
            records = 1;
        return begin + records * IP_RECORD_LENGTH;
    }
}

#endif