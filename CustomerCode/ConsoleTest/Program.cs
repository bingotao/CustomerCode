using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using System.Data.OleDb;
using Bingotao.Customer.BaseLib;

namespace ConsoleTest
{
    class Program
    {

        static void Main(string[] args)
        {

            var v = Convert.ChangeType(new List<int> { 1, 2, 3 }, typeof(List<double>));

        }

        public void Region()
        {
            string sql = @"select tdmj as v,point_x as x,point_y as y from points";
            string con = @"provider=microsoft.jet.oledb.4.0; Data Source=E:\Data\test.mdb;";

            OleDbDataAdapter da = new OleDbDataAdapter(sql, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            List<Point> pnts = new List<Point>();
            foreach (DataRow dr in dt.Rows)
            {
                pnts.Add(new Point()
                {
                    x = (double)dr["x"],
                    y = (double)dr["y"],
                    value = (double)dr["v"]
                });

            }
            Extent et = Extent.GetExtent(pnts, true);

            List<Point> newPoints = new List<Point>();
            Region rg = new Region(et, pnts, 0, 50, p =>
            {
                if (p.PointsCount != 0)
                {
                    var max = p.Points[0];
                    double value = 0;
                    foreach (Point pnt in p.Points)
                    {
                        if (pnt.value > max.value)
                        {
                            max = pnt;
                        }
                        value += pnt.value;
                    }
                    newPoints.Add(new Point() { x = max.x, y = max.y, value = value });
                }
            });
            rg.DoRegion();

            StringBuilder sb = new StringBuilder();
            foreach (var p in newPoints)
            {
                sb.AppendFormat("{0}\t{1}\t{2}\n", p.x, p.y, p.value);
            }
            string s = sb.ToString();
        }
    }




    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }
        public double value { get; set; }

    }

    public class Extent
    {
        public double xMin { get; set; }
        public double xMax { get; set; }
        public double yMin { get; set; }
        public double yMax { get; set; }

        public bool ContainsPoint(Point pnt)
        {
            return this.xMax >= pnt.x && this.xMin <= pnt.x && this.yMax >= pnt.y && this.yMin <= pnt.y;
        }

        public static Extent GetExtent(List<Point> points, bool isIntExtent)
        {
            double xMin = points[0].x;
            double xMax = points[0].x;
            double yMin = points[0].y;
            double yMax = points[0].y;

            foreach (var pnt in points)
            {
                xMax = pnt.x > xMax ? pnt.x : xMax;
                xMin = pnt.x < xMin ? pnt.x : xMin;
                yMin = pnt.y < yMin ? pnt.y : yMin;
                yMax = pnt.y > yMax ? pnt.y : yMax;
            }

            return isIntExtent ?
                new Extent()
                {
                    xMax = Math.Ceiling(xMax),
                    xMin = Math.Floor(xMin),
                    yMax = Math.Ceiling(yMax),
                    yMin = Math.Floor(yMin)
                } :
                new Extent()
                {
                    xMax = xMax,
                    xMin = xMin,
                    yMax = yMax,
                    yMin = yMin
                };
        }
    }

    public delegate void onRegionReady(Region region);

    public class Region
    {
        public Extent Extent { get; set; }

        public List<Point> Points { get; set; }

        public int Level { get; set; }

        public int RegionPointsMaxCount { get; set; }

        public int PointsCount
        {
            get
            {
                return this.Points == null ? 0 : this.Points.Count;
            }
        }

        public Region Region1 { get; set; }

        public Region Region2 { get; set; }

        public Region Region3 { get; set; }

        public Region Region4 { get; set; }

        public onRegionReady onRegionReady;

        public Region(Extent extent, List<Point> points, int level, int regionPointsMaxCount, onRegionReady onRegionReady)
        {
            this.Extent = extent;
            this.Points = points;
            this.Level = level;
            this.RegionPointsMaxCount = regionPointsMaxCount;
            this.onRegionReady = onRegionReady;
            this.OnRegionReady += onRegionReady;
        }

        public event onRegionReady OnRegionReady;

        public void DoRegion()
        {
            if (this.PointsCount > this.RegionPointsMaxCount)
            {
                Extent et = this.Extent;

                double xMid = (et.xMin + et.xMax) / 2;
                double yMid = (et.yMin + et.yMax) / 2;

                Extent et1 = new Extent() { xMin = et.xMin, xMax = xMid, yMin = yMid, yMax = et.yMax };
                Extent et2 = new Extent() { xMin = xMid, xMax = et.xMax, yMin = yMid, yMax = et.yMax };
                Extent et3 = new Extent() { xMin = xMid, xMax = et.xMax, yMin = et.yMin, yMax = yMid };
                Extent et4 = new Extent() { xMin = et.xMin, xMax = xMid, yMin = et.yMin, yMax = yMid };

                List<Point> pnts1 = new List<Point>();
                List<Point> pnts2 = new List<Point>();
                List<Point> pnts3 = new List<Point>();
                List<Point> pnts4 = new List<Point>();

                foreach (Point pnt in this.Points)
                {
                    if (et1.ContainsPoint(pnt))
                        pnts1.Add(pnt);
                    else if (et2.ContainsPoint(pnt))
                        pnts2.Add(pnt);
                    else if (et3.ContainsPoint(pnt))
                        pnts3.Add(pnt);
                    else
                        pnts4.Add(pnt);
                }
                int level = this.Level + 1;
                this.Region1 = new Region(et1, pnts1, level, this.RegionPointsMaxCount, this.onRegionReady);
                this.Region1.DoRegion();
                this.Region2 = new Region(et2, pnts2, level, this.RegionPointsMaxCount, this.onRegionReady);
                this.Region2.DoRegion();
                this.Region3 = new Region(et3, pnts3, level, this.RegionPointsMaxCount, this.onRegionReady);
                this.Region3.DoRegion();
                this.Region4 = new Region(et4, pnts4, level, this.RegionPointsMaxCount, this.onRegionReady);
                this.Region4.DoRegion();
            }
            else if (this.OnRegionReady != null)
            {
                this.OnRegionReady(this);
            }
        }
    }


    /*
    public class PointThin
    {
        private IFeatureClass ftClsSource = null;
        private IFeatureClass ftClsTarget = null;
        private int sFieldIndex = -1;
        private int tFieldIndex = -1;
        private int count = 0;
        private IEnvelope envelope = null;

        public PointThin(IFeatureClass ftClsSource, IFeatureClass ftClsTarget, int count, string field)
        {
            this.ftClsSource = ftClsSource;
            this.ftClsTarget = ftClsTarget;
            sFieldIndex = ftClsSource.Fields.FindField(field);
            tFieldIndex = ftClsTarget.Fields.FindField(field);
            ((IFeatureClassManage)ftClsSource).UpdateExtent();
            envelope = (ftClsSource as IGeoDataset).Extent;
        }

        public void Thin(IEnvelope envelope)
        {
            ISpatialFilter pFilter = new SpatialFilterClass();
            pFilter.Geometry = envelope;
            pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor ftCursor = this.ftClsSource.Search(pFilter, false);
            IFeature ft = null;
            List<IPoint> points = new List<IPoint>();
            double value = 0;
            while ((ft = ftCursor.NextFeature()) != null)
            {
                points.Add(ft.Shape as IPoint);
                value += (double)ft.Value[this.sFieldIndex];
                if (points.Count > this.count)
                {
                    break;
                }
            }

            if (points.Count > this.count)
            {
                AEComReleaser.Release(pFilter, ftCursor);
                double xCenter = (envelope.XMax - envelope.XMin) / 2 + envelope.XMin;
                double yCenter = (envelope.YMax - envelope.YMin) / 2 + envelope.YMin;

                //左上角
                IEnvelope enLeftTop = new EnvelopeClass();
                enLeftTop.SpatialReference = envelope.SpatialReference;
                enLeftTop.XMin = envelope.XMin;
                enLeftTop.XMax = xCenter;
                enLeftTop.YMin = yCenter;
                enLeftTop.YMax = envelope.YMax;
                Thin(enLeftTop);

                //右上角
                IEnvelope enRightTop = new EnvelopeClass();
                enRightTop.SpatialReference = envelope.SpatialReference;
                enRightTop.XMin = xCenter;
                enRightTop.XMax = envelope.XMax;
                enRightTop.YMin = yCenter;
                enRightTop.YMax = envelope.YMax;
                Thin(enRightTop);

                //左下角
                IEnvelope enLeftBottom = new EnvelopeClass();
                enLeftBottom.SpatialReference = envelope.SpatialReference;
                enLeftBottom.XMin = envelope.XMin;
                enLeftBottom.XMax = xCenter;
                enLeftBottom.YMin = envelope.YMin;
                enLeftBottom.YMax = yCenter;
                Thin(enLeftBottom);

                //右下角
                IEnvelope enRightBottom = new EnvelopeClass();
                enRightBottom.SpatialReference = envelope.SpatialReference;
                enRightBottom.XMin = xCenter;
                enRightBottom.XMax = envelope.XMax;
                enRightBottom.YMin = envelope.YMin;
                enRightBottom.YMax = yCenter;
                Thin(enRightBottom);
            }
            else
            {
                double minDis = 0;
                IPoint cPnt = GeometryUtilities.GetCenterPoint(points, out minDis);
                IFeature newFt = ftClsTarget.CreateFeature();
                newFt.Shape = cPnt;
                newFt.Value[tFieldIndex] = value;
                newFt.Store();
            }
        }
    }*/
}
