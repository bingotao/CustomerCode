using Bingotao.Customer.GIS;
using Bingotao.Customer.GIS.Analysis;
using Bingotao.Customer.GIS.LicenseInitializer;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class KeyValue
    {
        public string Key;
        public double Value;
    }

    public class Analysis
    {
        public static Dictionary<string, object> DoAnalysis(string dksyh)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            new LicenseInitializer().InitLicense();
            IFeatureWorkspace wk = WorkspaceUtilities.GetWorkspace(@"C:\Data\浙江省工业用地数据\富阳数据\fy_gyyd.gdb", enumWorkspaceType.GDB) as IFeatureWorkspace;

            IFeatureClass test = wk.OpenFeatureClass("TEST");
            IFeature ftTest = test.GetFeature(2);

            IFeatureClass dltb = wk.OpenFeatureClass("DLTB");
            IFeatureClass tdytq = wk.OpenFeatureClass("TDYTQ");
            IFeatureClass jsydgzq = wk.OpenFeatureClass("JSYDGZQ");

            var dltbResult = Analysis.DLAnalysis(ftTest.ShapeCopy as IPolygon, dltb);
            var tdytqResult = Analysis.TDYTQAnalysis(ftTest.ShapeCopy as IPolygon, tdytq);
            var jsydgzqResult = Analysis.JSYDGZQAnalysis(ftTest.ShapeCopy as IPolygon, jsydgzq);
            dict["DLTB"] = dltbResult;
            dict["TDYTQ"] = tdytqResult;
            dict["JSYDGZQ"] = jsydgzqResult;
            return dict;
        }

        public static Dictionary<string, double> DLAnalysis(IPolygon dkGeometry, IFeatureClass dltbCls)
        {
            var dt = CommonAnalysis.Intersect(dkGeometry, dltbCls, "DLBM", "QSXZ");

            DataColumn dcQSXZ2 = new DataColumn("QSXZ2");
            dcQSXZ2.Expression = "iif(QSXZ in('10','20'),'G','J')";
            dt.Columns.Add(dcQSXZ2);

            DataColumn dcSDL = new DataColumn("SDL");
            dcSDL.Expression = "iif(DLBM in('011','012','013','021','022','023','031','032','033','041','042','104','114','117','122','123'),'N',iif(DLBM in ('201','202','203','204','205','101','102','105','106','107','113','117','118'),'J','W'))";
            dt.Columns.Add(dcSDL);
            //按地类、按权属汇总
            var dlbm_qsxz = from c in dt.AsEnumerable()
                            group c by new { SDL = c.Field<string>("SDL"), DLBM = c.Field<string>("DLBM"), QSXZ = c.Field<string>("QSXZ2") } into g

                            select new
                            {
                                SDL = g.Key.SDL,
                                DLBM = g.Key.DLBM,
                                QSXZ = g.Key.QSXZ,
                                MJ = g.Sum(p => p.Field<double>("MJ"))
                            };
            var dlbm_qsxzList = (from c in dlbm_qsxz
                                 select new KeyValue
                                 {
                                     Key = string.Format("{0}_{1}", c.DLBM, c.QSXZ),
                                     Value = c.MJ
                                 }
                                 ).ToList();

            //按地类汇总
            var dlbm = from c in dlbm_qsxz
                       group c by c.DLBM into g
                       select new KeyValue
                       {
                           Key = string.Format("{0}_Z", g.Key),
                           Value = g.Sum(p => p.MJ)
                       };
            var dlbmList = dlbm.ToList();

            //按三大类、按权属汇总
            var sdl_qsxz = from c in dlbm_qsxz
                           group c by new { SDL = c.SDL, QSXZ = c.QSXZ } into g
                           select new KeyValue
                           {
                               Key = string.Format("{0}_{1}", g.Key.SDL, g.Key.QSXZ),
                               Value = g.Sum(p => p.MJ)
                           };
            var sdl_qsxzList = sdl_qsxz.ToList();

            //按三大类
            var sdl = from c in dlbm_qsxz
                      group c by new { SDL = c.SDL } into g
                      select new KeyValue
                      {
                          Key = string.Format("{0}_Z", g.Key.SDL),
                          Value = g.Sum(p => p.MJ)
                      };
            var sdlList = sdl.ToList();

            List<KeyValue> list = new List<KeyValue>();

            //全部、分权属汇总
            var z_qsxz = from c in dlbm_qsxz
                         group c by c.QSXZ into g
                         select new KeyValue
                         {
                             Key = string.Format("Z_{0}", g.Key),
                             Value = g.Sum(p => p.MJ)
                         };
            var z_qsxzList = z_qsxz.ToList();
            //全部汇总
            var z = new KeyValue
            {
                Key = string.Format("Z_Z"),
                Value = dlbm_qsxz.Sum(p => p.MJ)
            };

            list.AddRange(sdlList);
            list.AddRange(sdl_qsxzList);
            list.AddRange(dlbmList);
            list.AddRange(dlbm_qsxzList);
            list.AddRange(z_qsxzList);
            list.Add(z);

            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (var keyValue in list)
            {
                dict[keyValue.Key] = keyValue.Value;
            }
            return dict;
        }

        public static Dictionary<string, double> TDYTQAnalysis(IPolygon dkGeometry, IFeatureClass tdytqCls)
        {
            var dt = CommonAnalysis.Intersect(dkGeometry, tdytqCls, "TDYTQLXDM");

            var tdytQuery = from r in dt.AsEnumerable()
                            group r by new { TDYTQLXDM = r.Field<string>("TDYTQLXDM").Substring(0, 2) + "0" } into g
                            select new KeyValue
                            {
                                Key = g.Key.TDYTQLXDM,
                                Value = g.Sum(p => p.Field<double>("MJ"))
                            };
            var tdytList = (from c in tdytQuery
                            select new KeyValue
                            {
                                Key = c.Key + "_MJ",
                                Value = c.Value
                            }).ToList();

            var total = new KeyValue()
            {
                Key = "Z_MJ",
                Value = tdytQuery.Sum(p => p.Value)
            };

            var tdytblList = (from c in tdytQuery
                              select new KeyValue
                              {
                                  Key = c.Key + "_BL",
                                  Value = c.Value == 0 ? 0 : 100 * c.Value / total.Value
                              }).ToList<KeyValue>();
            var list = new List<KeyValue>();

            list.AddRange(tdytList);
            list.AddRange(tdytblList);

            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (var keyValue in list)
            {
                dict[keyValue.Key] = keyValue.Value;
            }
            return dict;
        }


        public static Dictionary<string, double> JSYDGZQAnalysis(IPolygon dkGeometry, IFeatureClass tdytqCls)
        {
            var dt = CommonAnalysis.Intersect(dkGeometry, tdytqCls, "GZQLXDM");

            var tdytQuery = from r in dt.AsEnumerable()
                            group r by new { GZQLXDM = r.Field<string>("GZQLXDM").Substring(0, 2) + "0" } into g
                            select new KeyValue
                            {
                                Key = g.Key.GZQLXDM,
                                Value = g.Sum(p => p.Field<double>("MJ"))
                            };
            var tdytList = (from c in tdytQuery
                            select new KeyValue
                            {
                                Key = c.Key + "_MJ",
                                Value = c.Value
                            }).ToList();

            var total = new KeyValue()
            {
                Key = "Z_MJ",
                Value = tdytQuery.Sum(p => p.Value)
            };

            var tdytblList = (from c in tdytQuery
                              select new KeyValue
                              {
                                  Key = c.Key + "_BL",
                                  Value = c.Value == 0 ? 0 : 100 * c.Value / total.Value
                              }).ToList<KeyValue>();
            var list = new List<KeyValue>();

            list.AddRange(tdytList);
            list.AddRange(tdytblList);

            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (var keyValue in list)
            {
                dict[keyValue.Key] = keyValue.Value;
            }
            return dict;
        }
    }
}
