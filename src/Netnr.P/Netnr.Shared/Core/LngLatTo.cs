using System;

namespace Netnr;

/// <summary>
/// 经纬度
/// </summary>
public partial class LngLatTo
{
    #region 坐标转换转译 https://github.com/wandergis/coordtransform

    private const double x_PI = 3.14159265358979324 * 3000.0 / 180.0;
    private const double PI = 3.1415926535897932384626;
    private const double a = 6378245.0;
    private const double ee = 0.00669342162296594323;

    /// <summary>
    /// 百度坐标系 (BD-09) 与 火星坐标系 (GCJ-02) 的转换
    /// 即 百度 转 谷歌、高德
    /// </summary>
    /// <param name="bd_lng"></param>
    /// <param name="bd_lat"></param>
    /// <returns></returns>
    public static double[] Bd09ToGcj02(double bd_lng, double bd_lat)
    {
        double x = bd_lng - 0.0065;
        double y = bd_lat - 0.006;
        double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_PI);
        double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_PI);
        double gg_lng = z * Math.Cos(theta);
        double gg_lat = z * Math.Sin(theta);
        return [gg_lng, gg_lat];
    }

    /// <summary>
    /// 火星坐标系 (GCJ-02) 与百度坐标系 (BD-09) 的转换
    /// 即 谷歌、高德 转 百度
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] Gcj02ToBd09(double lng, double lat)
    {
        double z = Math.Sqrt(lng * lng + lat * lat) + 0.00002 * Math.Sin(lat * x_PI);
        double theta = Math.Atan2(lat, lng) + 0.000003 * Math.Cos(lng * x_PI);
        double bd_lng = z * Math.Cos(theta) + 0.0065;
        double bd_lat = z * Math.Sin(theta) + 0.006;
        return [bd_lng, bd_lat];
    }

    /// <summary>
    /// WGS-84 转 GCJ-02
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] Wgs84ToGcj02(double lng, double lat)
    {
        if (OutOfChina(lng, lat))
        {
            return [lng, lat];
        }
        else
        {
            double dlat = TransformLat(lng - 105.0, lat - 35.0);
            double dlng = TransformLng(lng - 105.0, lat - 35.0);
            double radlat = lat / 180.0 * PI;
            double magic = Math.Sin(radlat);
            magic = 1 - ee * magic * magic;
            double sqrtmagic = Math.Sqrt(magic);
            dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
            dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
            double mglat = lat + dlat;
            double mglng = lng + dlng;
            return [mglng, mglat];
        }
    }

    /// <summary>
    /// GCJ-02 转换为 WGS-84
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static double[] Gcj02ToWgs84(double lng, double lat)
    {
        if (OutOfChina(lng, lat))
        {
            return [lng, lat];
        }
        else
        {
            double dlat = TransformLat(lng - 105.0, lat - 35.0);
            double dlng = TransformLng(lng - 105.0, lat - 35.0);
            double radlat = lat / 180.0 * PI;
            double magic = Math.Sin(radlat);
            magic = 1 - ee * magic * magic;
            double sqrtmagic = Math.Sqrt(magic);
            dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
            dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
            double mglat = lat + dlat;
            double mglng = lng + dlng;
            return [lng * 2 - mglng, lat * 2 - mglat];
        }
    }

    private static double TransformLat(double lng, double lat)
    {
        double ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lat * PI) + 40.0 * Math.Sin(lat / 3.0 * PI)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(lat / 12.0 * PI) + 320 * Math.Sin(lat * PI / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    private static double TransformLng(double lng, double lat)
    {
        double ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lng * PI) + 40.0 * Math.Sin(lng / 3.0 * PI)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(lng / 12.0 * PI) + 300.0 * Math.Sin(lng / 30.0 * PI)) * 2.0 / 3.0;
        return ret;
    }

    /// <summary>
    /// 判断是否在国内，不在国内则不做偏移
    /// </summary>
    /// <param name="lng"></param>
    /// <param name="lat"></param>
    /// <returns></returns>
    public static bool OutOfChina(double lng, double lat)
    {
        // 纬度 3.86~53.55, 经度 73.66~135.05
        return !(lng > 73.66 && lng < 135.05 && lat > 3.86 && lat < 53.55);
    }

    #endregion

    #region 点到线段的垂直距离

    #endregion
}
