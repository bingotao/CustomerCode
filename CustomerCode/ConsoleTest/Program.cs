using Bingotao.Customer.BaseLib.Entity;
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
    class Program
    {
        class KeyValue
        {
            public string Key;
            public double Value;
        }
        static void Main(string[] args)
        {

            new LicenseInitializer().InitLicense();
            //IFeatureWorkspace wk = WorkspaceUtilities.GetWorkspace(@"C:\Data\浙江省工业用地数据\富阳数据\fy_gyyd.gdb", enumWorkspaceType.GDB) as IFeatureWorkspace;
            //IFeatureClass test = wk.OpenFeatureClass("TEST");
            //IFeatureClass dltb = wk.OpenFeatureClass("DLTB");

            //IFeature ftTest = test.GetFeature(1);

            //var d = CommonAnalysis.Intersect(ftTest.ShapeCopy as IPolygon, dltb, "DLBM");


            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("DLBM"), new DataColumn("QSXZ"), new DataColumn("MJ", typeof(double)) });

            dt.LoadDataRow(new object[] { "011", "30", 100 }, true);
            dt.LoadDataRow(new object[] { "011", "31", 220 }, true);
            dt.LoadDataRow(new object[] { "011", "10", 80 }, true);
            dt.LoadDataRow(new object[] { "012", "10", 64 }, true);
            dt.LoadDataRow(new object[] { "012", "20", 32 }, true);
            dt.LoadDataRow(new object[] { "206", "10", 55 }, true);
            dt.LoadDataRow(new object[] { "203", "10", 55 }, true);

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

        }
    }
}
