using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.GIS.Gemetry
{
    public class GeometryUtilities
    {
        /// <summary>
        /// 获取Polygon的中心点
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static IPoint GetCenterPoint(IPolygon polygon)
        {
            IArea pArea = polygon as IArea;
            IPoint pPoint = new PointClass();
            pArea.QueryCentroid(pPoint);
            return pPoint;
        }

        /// <summary>
        /// 从点集中选出一个点，该点到所有其他点的距离最小
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPoint GetCenterPoint(IEnumerable<IPoint> points, out double minDistance)
        {
            IPoint pCenterPoint = null;
            minDistance = double.MaxValue;
            foreach (IPoint p1 in points)
            {
                double dis = 0;
                foreach (IPoint p2 in points)
                {
                    dis += Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                }
                if (dis < minDistance)
                {
                    minDistance = dis;
                    pCenterPoint = p1;
                }
            }
            return pCenterPoint;
        }

        /// <summary>
        /// 求点集的均值点
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPoint GetMeanPoint(IEnumerable<IPoint> points)
        {
            int count = points.Count();
            double x = 0;
            double y = 0;
            if (count != 0)
            {
                foreach (IPoint pnt in points)
                {
                    x += pnt.X / count;
                    y += pnt.Y / count;
                }
            }
            return new PointClass() { X = x, Y = y };
        }

        /// <summary>
        /// 求点集的中位数点
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IPoint GetMedianPoint(IEnumerable<IPoint> points)
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();

            foreach (IPoint pnt in points)
            {
                xList.Add(pnt.X);
                yList.Add(pnt.Y);
            }
            xList.Sort((a, b) => (int)(a - b));
            double x = xList[xList.Count / 2];
            yList.Sort((a, b) => (int)(a - b));
            double y = yList[yList.Count / 2];
            return new PointClass() { X = x, Y = y };
        }
    }
}
