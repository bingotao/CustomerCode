using ESRI.ArcGIS.Geometry;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.GIS.Geometry
{
    /// <summary>
    /// Polygon构建器
    /// </summary>
    public class PolygonConstructor
    {
        /// <summary>
        /// 构造Polygon 
        /// </summary>
        /// <param name="ringArrayString">传入的字符串格式为："[[[1,1],[2,2],[2,1]],[[4,4],[5,5],[5,4]]"</param>
        /// <param name="wkit">空间参考ID</param>
        /// <returns></returns>
        public static IPolygon Construct(string ringArrayString, int wkit)
        {
            JArray jArray = JArray.Parse(ringArrayString);

            IPolygon polygon = new PolygonClass();
            ISpatialReferenceFactory spatialRefFact = new SpatialReferenceEnvironmentClass();
            ISpatialReference spatialRef = spatialRefFact.CreateProjectedCoordinateSystem(wkit);
            polygon.SpatialReference = spatialRef;

            IGeometryCollection ringCollection = polygon as IGeometryCollection;
            int ringCount = jArray.Count;
            for (int i = 0; i < ringCount; i++)
            {
                JArray jPointArray = JArray.Parse(jArray[i].ToString());
                IRing ring = new RingClass();
                IPointCollection pointCollection = ring as IPointCollection;
                int pointCount = jPointArray.Count;
                for (int j = 0; j < pointCount; j++)
                {
                    JArray jPoint = JArray.Parse(jPointArray[j].ToString());
                    double x = double.Parse(jPoint[0].ToString());
                    double y = double.Parse(jPoint[1].ToString());

                    IPoint point = new PointClass();
                    point.X = x;
                    point.Y = y;
                    pointCollection.AddPoint(point);
                    if (j == pointCount - 1)
                    {
                        ring.Close();
                    }
                }
                ringCollection.AddGeometry(ring);
            }
            (polygon as ITopologicalOperator).Simplify();
            return polygon;
        }
    }
}
