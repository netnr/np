using System;
using System.IO;
using System.Net;
using System.Text;

namespace Netnr;

/// <summary>
/// IPv6
/// </summary>
public class IPv6Query
{
    private Stream stream;
    /// <summary>
    /// 数据总量
    /// </summary>
    public int Total { get; set; }
    /// <summary>
    /// 开始索引
    /// </summary>
    public int IndexStartOffset { get; set; }
    /// <summary>
    /// 结束索引
    /// </summary>
    public int IndexEndOffset { get; set; }
    /// <summary>
    /// 地址长度（数据记录地址）
    /// </summary>
    public int OffLen { get; set; }
    /// <summary>
    /// IP长度
    /// </summary>
    public int IPLen { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="dbFile"></param>
    public IPv6Query(string dbFile)
    {
        stream = new FileStream(dbFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        IndexStartOffset = Read4(16);
        Total = Read4(8);
        OffLen = Read1(6);
        IPLen = Read1(7);
        IndexEndOffset = IndexStartOffset + (IPLen + OffLen) * Total;
    }

    /// <summary>
    /// 跳过的字节数
    /// </summary>
    /// <param name="v"></param>
    void Seek(long v)
    {
        stream.Seek(v, SeekOrigin.Begin);
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        if (stream != null)
        {
            stream.Flush();
            stream.Close();
            stream = null;
        }
    }

    /// <summary>
    /// 读数据：8字节
    /// </summary>
    /// <returns></returns>
    long Read8(int seek0)
    {
        if (seek0 > 0)
        {
            Seek(seek0);
        }
        //var bytes = BitConverter.GetBytes(8);
        var bytes = new byte[8];
        stream.Read(bytes, 0, bytes.Length);
        long te = BitConverter.ToInt64(bytes, 0);
        //BigInteger gi = new BigInteger(bytes);
        return te;
    }

    /// <summary>
    /// 读数据：4字节
    /// </summary>
    /// <returns></returns>
    int Read4(int seek0, int size = 4)
    {
        if (seek0 > 0)
        {
            Seek(seek0);
        }
        var bytes = BitConverter.GetBytes(size);
        stream.Read(bytes, 0, bytes.Length);
        int te = BitConverter.ToInt32(bytes, 0);
        return te;
    }

    /// <summary>
    /// 读数据：4字节
    /// </summary>
    /// <returns></returns>
    int Read3(int seek0)
    {
        if (seek0 > 0)
        {
            Seek(seek0);
        }
        var bytes = new byte[3];
        stream.Read(bytes, 0, bytes.Length);
        return bytes[0] + bytes[1] * 256 + bytes[2] * 256 * 256;
    }

    /// <summary>
    /// 读数据：1字节
    /// </summary>
    /// <returns></returns>
    int Read1(int seek0)
    {
        if (seek0 > 0)
        {
            Seek(seek0);
        }

        var bytes = new byte[2];
        stream.Read(bytes, 0, bytes.Length);
        int te = BitConverter.ToUInt16(bytes, 0);
        return te % 256;
    }

    /// <summary>
    /// 地址查找
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    string[] ReadRecord(int offset)
    {
        string[] record = new string[] { "", "" };
        int flag = Read1(offset); //读取offset的1字节
                                  //if (flag < 0) flag += 256;
        if (flag == 1)
        {
            // 地址重定向
            int location_offset = Read4(offset + 1, OffLen);
            return ReadRecord(location_offset);
        }
        else
        {
            record[0] = ReadLocation(offset); //获取真实地址名称
            if (flag == 2)
            {
                record[1] = ReadLocation(offset + OffLen + 1); //获取运营商
            }
            else
            {
                record[1] = ReadLocation(offset + (record[0]).ToString().Length + 1); //运营商跟在后面
            }
        }
        return record;
    }

    /// <summary>
    /// 读取地区
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    string ReadLocation(int offset)
    {
        if (offset == 0)
        {
            return "";
        }
        int flag = Read1(offset);
        if (flag < 0) flag += 256;
        // 出错
        if (flag == 0)
        {
            return "";
        }
        // 仍然为重定向
        if (flag == 2)
        {
            //地址ID
            offset = Read3(offset + 1);
            return ReadLocation(offset);
        }
        string location = Readstr(offset);
        return location;
    }

    // 读取文字
    string Readstr(int offset = -1)
    {
        if (offset > 0)
        {
            Seek(offset);
        }
        var strs = "";
        var chr = Read1(offset);
        while (chr != 0)
        {
            if (chr < 0)
            {
                chr += 256;
            }
            string hex = Convert.ToString(chr, 16);
            if (hex.Length == 1)
            {
                hex = "0" + hex;
            }
            strs += hex;
            offset++;
            chr = Read1(offset);
        }

        string str_aa = Encoding.UTF8.GetString(HexStringToByteArray(strs.ToUpper()));

        //删除乱码（符号）
        if (str_aa[0] == 65533)
        {
            str_aa = str_aa.Substring(1);
        }
        return str_aa;
    }

    byte[] HexStringToByteArray(string s)
    {
        s = s.Replace(" ", "").Trim().ToUpper();
        byte[] buffer = new byte[s.Length / 2];
        for (int i = 0; i < s.Length; i += 2)
            buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
        return buffer;
    }

    // +-------------------------
    // | 以下是解析部分
    // +-------------------------

    /// <summary>
    /// 查询IP信息
    /// </summary>
    /// <param name="strIP"></param>
    /// <returns></returns>
    public IPQueryResult Search(string strIP)
    {
        long aa = IP2Int64(strIP);
        long ip_num2 = 0;
        int ip_find = Find(aa, ip_num2, 0, Total);

        int ip_offset = IndexStartOffset + ip_find * (IPLen + OffLen);

        int ip_record_offset = Read4(ip_offset + IPLen, OffLen);
        string[] ip_addr = ReadRecord(ip_record_offset);

        var result = new IPQueryResult
        {
            Addr = ip_addr[0],
            ISP = ip_addr[1]
        };

        return result;
    }

    /// <summary>
    /// 把IP地址转换为数字 int64
    /// </summary>
    /// <returns></returns>
    public long IP2Int64(string address)
    {
        IPAddress ipa = IPAddress.Parse(address);
        byte[] ipbyte = ipa.GetAddressBytes();
        Array.Reverse(ipbyte);
        long tex = BitConverter.ToInt64(ipbyte, 8);
        string te0 = System.Convert.ToString(tex, 16);
        return tex;
    }

    /// <summary>
    /// IPv6位置查找：二分法
    /// </summary>
    /// <param name="ip_num1">IPv6的前int64</param>
    /// <param name="ip_num2">IPv6的后int64</param>
    /// <param name="l">数据位置</param>
    /// <param name="r">数据总量</param>
    public int Find(long ip_num1, long ip_num2, int l, int r)
    {
        if (l + 1 >= r)
        {
            return l;
        }
        int m = (l + r + 0) / 2;

        long m_ip1 = Read8(IndexStartOffset + m * (IPLen + OffLen));
        long m_ip2 = 0;
        if (IPLen <= 8)
        {
            m_ip1 <<= 8 * (8 - IPLen); //左移运算符
        }
        else
        {
            m_ip2 = Read8(IndexStartOffset + m * (IPLen + OffLen) + 8);
            m_ip2 <<= 8 * (16 - IPLen);
        }
        if (UInt64Cmp(ip_num1, m_ip1) < 0)
        {
            return Find(ip_num1, ip_num2, l, m);
        }
        else if (UInt64Cmp(ip_num1, m_ip1) > 0)
        {
            return Find(ip_num1, ip_num2, m, r);
        }
        else if (UInt64Cmp(ip_num2, m_ip2) < 0)
        {
            return Find(ip_num1, ip_num2, l, m);
        }
        else
        {
            return Find(ip_num1, ip_num2, m, r);
        }
    }
    /// <summary>
    /// 数据比较
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int UInt64Cmp(long a, long b)
    {
        if (a >= 0 && b >= 0 || a < 0 && b < 0)
        {
            if (a > b)
            {
                return 1;
            }
            else if (a == b)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else if (a >= 0 && b < 0)
        {
            return -1;
        }
        else if (a < 0 && b >= 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}