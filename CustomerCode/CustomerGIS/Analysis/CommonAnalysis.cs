using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bingotao.Customer.GIS.Feature;

namespace Bingotao.Customer.GIS.Analysis
{
    public class CommonAnalysis
    {
        /// <summary>
        /// 相交分析
        /// </summary>
        /// <param name="polygon">面</param>
        /// <param name="ftCls">所要分析的要素类</param>
        /// <param name="typeField">分析的字段</param>
        /// <returns></returns>
        public static Dictionary<string, double> Intersect(IPolygon polygon, IFeatureClass ftCls, params string[] typeFields)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            IFeatureCursor ftCursor = null;
            IFeature ft = null;
            try
            {
                (polygon as ITopologicalOperator).Simplify();

                Dictionary<string, int> fieldNameAndIndex = new Dictionary<string, int>();
                foreach (string fieldName in typeFields)
                {
                    fieldNameAndIndex.Add(fieldName, FeatureUtilities.GetFieldIndex(ftCls, fieldName));
                }

                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = polygon;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                ftCursor = ftCls.Search(spatialFilter, true);
                while ((ft = ftCursor.NextFeature()) != null)
                {
                    string type = string.Empty;
                    IGeometry geo = ft.Shape;
                    IGeometry geoIntersect = (geo as ITopologicalOperator).Intersect(polygon, esriGeometryDimension.esriGeometry2Dimension);
                    double intersectArea = (geoIntersect as IArea).Area;
                    foreach (string t in fieldNameAndIndex.Keys)
                    {
                        string s = ft.Value[fieldNameAndIndex[t]].ToString();
                        type = string.Format("{0}_{1}", type, s).Trim('_');
                        result[type] = result.ContainsKey(type) ? (result[type] + intersectArea) : intersectArea;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Releaser.AEComReleaser.Release(ftCursor, ft);
            }
            return result;
        }
    }
}
