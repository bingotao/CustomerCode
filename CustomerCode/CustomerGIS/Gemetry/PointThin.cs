using Bingotao.Customer.GIS.Releaser;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bingotao.Customer.GIS.Gemetry
{
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

        public void Start()
        {
            Thin(envelope);
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
            else {
                double minDis = 0;
                IPoint cPnt = GeometryUtilities.GetCenterPoint(points, out minDis);
                IFeature newFt = ftClsTarget.CreateFeature();
                newFt.Shape = cPnt;
                newFt.Value[tFieldIndex] = value;
                newFt.Store();
            }
        }
    }
}
