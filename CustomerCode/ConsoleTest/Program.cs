using Bingotao.Customer.BaseLib.Entity;
using Bingotao.Customer.GIS;
using Bingotao.Customer.GIS.Analysis;
using Bingotao.Customer.GIS.LicenseInitializer;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new LicenseInitializer().InitLicense();
            IFeatureWorkspace wk = WorkspaceUtilities.GetWorkspace(@"C:\Data\浙江省工业用地数据\富阳数据\fy_gyyd.gdb", enumWorkspaceType.GDB) as IFeatureWorkspace;
            IFeatureClass test = wk.OpenFeatureClass("TEST");
            IFeatureClass dltb = wk.OpenFeatureClass("DLTB");

            IFeature ftTest = test.GetFeature(1);

            var d = CommonAnalysis.Intersect(ftTest.ShapeCopy as IPolygon, dltb, "DLBM");
        }


    }
}
