#if Full || Core

using System;
using System.Collections.Generic;

namespace Netnr;

/// <summary>
/// 地理、几何
/// </summary>
public partial class GeoTo
{
    /// <summary>
    /// 判断经纬度点是否在区域内
    /// 
    /// 引射线法：就是从该点出发引一条射线，看这条射线和所有边的交点数目。
    ///           如果有奇数个交点，则说明在内部，如果有偶数个交点，则说明在外部。
    ///           这是所有方法中计算量最小的方法，在光线追踪算法中有大量的应用。
    /// </summary>
    /// <param name="point"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static bool IsPointInRing(KeyValuePair<double, double> point, KeyValuePair<double, double>[] polygon)
    {
        if (polygon.Length < 3)
        {
            return false;
        }

        int count = polygon.Length;
        double lon1, lon2, lat1, lat2, lon;
        int intersectionCount = 0;

        for (int i = 0; i < count; i++)
        {
            if (point.Equals(polygon[i]))  // A点在多边形上    
            {
                return true;
            }

            if (i == count - 1)
            {
                lon1 = polygon[i].Key;
                lat1 = polygon[i].Value;
                lon2 = polygon[0].Key;
                lat2 = polygon[0].Value;
            }
            else
            {
                lon1 = polygon[i].Key;
                lat1 = polygon[i].Value;
                lon2 = polygon[i + 1].Key;
                lat2 = polygon[i + 1].Value;
            }

            // 判断A点是否在边的两端点的纬度之间，在则可能有交点
            if (((point.Value > lat1) && (point.Value < lat2)) || ((point.Value > lat2) && (point.Value < lat1)))
            {
                if (Math.Abs(lat1 - lat2) > 0)
                {
                    // 获取A点向左射线与边的交点的x坐标
                    lon = lon1 - ((lon1 - lon2) * (lat1 - point.Value)) / (lat1 - lat2);
                    // 如果交点在A点左侧，则射线与边的全部交点数加一
                    if (lon < point.Key)
                    {
                        intersectionCount++;
                    }
                    // 如果相等，则说明A点在边上
                    if (lon == point.Key)
                    {
                        return true;
                    }
                }
            }
        }

        if (intersectionCount % 2 != 0)
        {
            return true;
        }

        return false;
    }

    #region 点到线或多边形 最短距离

    /// <summary>
    /// 点到多边形最短距离
    /// </summary>
    /// <param name="point"></param>
    /// <param name="polygon"></param>
    /// <returns></returns>
    public static double PointToPolygonDistance(KeyValuePair<double, double> point, KeyValuePair<double, double>[] polygon)
    {
        var minDistance = double.MaxValue;

        for (var i = 0; i < polygon.Length; i++)
        {
            var lineStart = polygon[i];
            var lineEnd = polygon[(i + 1) % polygon.Length];

            var distance = PointToLineDistance(point, lineStart, lineEnd);
            minDistance = Math.Min(minDistance, distance);
        }

        return minDistance;
    }

    /// <summary>
    /// 点到线最短距离
    /// </summary>
    /// <param name="point">点</param>
    /// <param name="lineStart">线段开始点</param>
    /// <param name="lineEnd">线段结束点</param>
    /// <returns></returns>
    public static double PointToLineDistance(KeyValuePair<double, double> point, KeyValuePair<double, double> lineStart, KeyValuePair<double, double> lineEnd)
    {
        double pointLat = DegreesToRadians(point.Value);
        double pointLng = DegreesToRadians(point.Key);
        double lineStartLat = DegreesToRadians(lineStart.Value);
        double lineStartLng = DegreesToRadians(lineStart.Key);
        double lineEndLat = DegreesToRadians(lineEnd.Value);
        double lineEndLng = DegreesToRadians(lineEnd.Key);

        double A = pointLng - lineStartLng;
        double B = pointLat - lineStartLat;
        double C = lineEndLng - lineStartLng;
        double D = lineEndLat - lineStartLat;

        double dotProduct = A * C + B * D;
        double lineLengthSq = C * C + D * D;
        double param = dotProduct / lineLengthSq;

        double nearestPointLng = param <= 0 ? lineStartLng : (param >= 1 ? lineEndLng : lineStartLng + param * C);
        double nearestPointLat = param <= 0 ? lineStartLat : (param >= 1 ? lineEndLat : lineStartLat + param * D);

        double dLng = pointLng - nearestPointLng;
        double dLat = pointLat - nearestPointLat;
        double distanceInRadians = Math.Sqrt(dLng * dLng + dLat * dLat);
        double distanceInMeters = distanceInRadians * EarthRadius;

        return distanceInMeters;
    }

    /// <summary>
    /// 地球半径，单位：米
    /// </summary>
    private const double EarthRadius = 6378137;

    /// <summary>
    /// 度数转弧度
    /// </summary>
    /// <param name="degrees"></param>
    /// <returns></returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    #endregion
}

#endif