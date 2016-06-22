using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace Bingotao.Customer.BaseLib
{
    public class MakeThum
    {
        public MakeThum()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }


        public static Image PictureResize(Image image, int newMaxHeight, int newMaxWidth)
        {
            Image newImage = null;
            try
            {
                //原始高度
                int iOH = image.Height;
                //原始宽度
                int iOW = image.Width;
                //原始高宽比
                double dORatio = (double)iOH / iOW;
                //压缩后宽高比
                double dNRation = (double)newMaxHeight / newMaxWidth;
                //压缩比
                double dPressRatio = 1.0;
                if (newMaxHeight == 0)
                {
                    dPressRatio = (double)iOW / newMaxWidth;
                }
                else if (newMaxWidth == 0)
                {
                    dPressRatio = (double)iOH / newMaxHeight;
                }
                else
                {
                    dPressRatio = dORatio > dNRation ? (double)iOH / newMaxHeight : (double)iOW / newMaxWidth;
                }
                //新图片高度
                int iNH = (int)(iOH / dPressRatio);
                //新图片宽度
                int iNW = (int)(iOW / dPressRatio);
                //新图片
                newImage = new System.Drawing.Bitmap(image, iNW, iNH);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return newImage;
        }


        public static void PictureResize(string sOriginalImagePath, string sResizedImagePath, int iNewMaxHeight, int iNewMaxWidth, bool bPress = false)
        {
            //原始图片
            Image oImage = null;
            Image nImage = null;
            try
            {
                oImage = Image.FromFile(sOriginalImagePath);
                //原始高度
                int iOH = oImage.Height;
                //原始宽度
                int iOW = oImage.Width;
                //原始高宽比
                double dORatio = (double)iOH / iOW;
                //压缩后宽高比
                double dNRation = (double)iNewMaxHeight / iNewMaxWidth;
                //压缩比
                double dPressRatio = 1.0;
                if (iNewMaxHeight == 0)
                {
                    dPressRatio = (double)iOW / iNewMaxWidth;
                }
                else if (iNewMaxWidth == 0)
                {
                    dPressRatio = (double)iOH / iNewMaxHeight;
                }
                else
                {
                    dPressRatio = dORatio > dNRation ? (double)iOH / iNewMaxHeight : (double)iOW / iNewMaxWidth;
                }
                //新图片高度
                int iNH = (int)(iOH / dPressRatio);
                //新图片宽度
                int iNW = (int)(iOW / dPressRatio);
                //新图片
                nImage = new System.Drawing.Bitmap(oImage, iNW, iNH);

                if (bPress)
                {
                    ImageCodecInfo imageCodecInfo = null;
                    ImageFormat imageFormat = null;
                    string sExtension = Path.GetExtension(sOriginalImagePath);
                    GetEncodecInfo(sExtension, out imageCodecInfo, out imageFormat);
                    EncoderParameters encoderParams = new EncoderParameters(1);
                    encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 60L);
                    nImage.Save(sResizedImagePath, imageCodecInfo, encoderParams);
                }
                else
                {
                    nImage.Save(sResizedImagePath);
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                oImage.Dispose();
                nImage.Dispose();
            }
        }

        private static void GetEncodecInfo(string sExtension, out ImageCodecInfo imageCodecInfo, out ImageFormat imageFormat)
        {
            string sMIMEType = string.Empty;
            switch (sExtension.ToLower())
            {
                case ".gif":
                    sMIMEType = "image/gif";
                    imageFormat = ImageFormat.Gif;
                    break;
                case ".bmp":
                    sMIMEType = "image/bmp";
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".png":
                    sMIMEType = "image/png";
                    imageFormat = ImageFormat.Png;
                    break;
                case ".jpg":
                default:
                    sMIMEType = "image/jpeg";
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }
            imageCodecInfo = GetEncoderInfo(sMIMEType);
        }


        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sOriginalImagePath">原始路径</param>
        /// <param name="sThumbnailPath">生成缩略图路径</param>
        /// <param name="iWidth">缩略图的宽</param>
        /// <param name="iHeight">缩略图的高</param>
        /// <param name="bYes">是否压缩图片质量</param>
        public static void MakeThumbnail(string sOriginalImagePath, string sThumbnailPath, int iWidth, int iHeight, bool bYes)
        {
            //获取原始图片  
            System.Drawing.Image originalImage = System.Drawing.Image.FromFile(sOriginalImagePath);
            //缩略图画布宽高  
            int iToWidth = iWidth;
            int iToHeight = iHeight;
            //原始图片写入画布坐标和宽高(用来设置裁减溢出部分)  
            int iX = 0;
            int iY = 0;
            int iOW = originalImage.Width;
            int iOH = originalImage.Height;
            //原始图片画布,设置写入缩略图画布坐标和宽高(用来原始图片整体宽高缩放)  
            int bg_x = 0;
            int bg_y = 0;
            int bg_w = iToWidth;
            int bg_h = iToHeight;
            //倍数变量  
            double dMultiple = 0;
            //获取宽长的或是高长与缩略图的倍数  
            dMultiple = originalImage.Width >= originalImage.Height ? (double)originalImage.Width / (double)iWidth : (double)originalImage.Height / (double)iHeight;
            //上传的图片的宽和高小等于缩略图  
            if (iOW <= iWidth && iOH <= iHeight)
            {
                //缩略图按原始宽高  
                bg_w = originalImage.Width;
                bg_h = originalImage.Height;
                //空白部分用背景色填充  
                bg_x = Convert.ToInt32(((double)iToWidth - (double)iOW) / 2);
                bg_y = Convert.ToInt32(((double)iToHeight - (double)iOH) / 2);
            }
            //上传的图片的宽和高大于缩略图  
            else
            {
                //宽高按比例缩放  
                bg_w = Convert.ToInt32((double)originalImage.Width / dMultiple);
                bg_h = Convert.ToInt32((double)originalImage.Height / dMultiple);
                //空白部分用背景色填充  
                bg_y = Convert.ToInt32(((double)iHeight - (double)bg_h) / 2);
                bg_x = Convert.ToInt32(((double)iWidth - (double)bg_w) / 2);
            }
            //新建一个bmp图片,并设置缩略图大小.  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(iToWidth, iToHeight);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并设置背景色  
            //g.Clear(System.Drawing.ColorTranslator.FromHtml("#FFF"));
            SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0));
            g.FillPolygon(brush, new Point[] { new Point(0, 0), new Point(0, 1000), new Point(1000, 1000), new Point(1000, 0) });
            //g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分  
            //第一个System.Drawing.Rectangle是原图片的画布坐标和宽高,第二个是原图片写在画布上的坐标和宽高,最后一个参数是指定数值单位为像素  
            g.DrawImage(originalImage, new System.Drawing.Rectangle(bg_x, bg_y, bg_w, bg_h), new System.Drawing.Rectangle(iX, iY, iOW, iOH), System.Drawing.GraphicsUnit.Pixel);

            if (bYes)
            {
                System.Drawing.Imaging.ImageCodecInfo encoder = GetEncoderInfo("image/jpeg");
                try
                {
                    if (encoder != null)
                    {
                        System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                        //设置 jpeg 质量为 60
                        encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)60);
                        bitmap.Save(sThumbnailPath, encoder, encoderParams);
                        encoderParams.Dispose();
                    }
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
            else
            {
                try
                {
                    //获取图片类型  
                    string fileExtension = System.IO.Path.GetExtension(sOriginalImagePath).ToLower();
                    //按原图片类型保存缩略图片,不按原格式图片会出现模糊,锯齿等问题.  
                    switch (fileExtension)
                    {
                        case ".gif": bitmap.Save(sThumbnailPath, System.Drawing.Imaging.ImageFormat.Gif); break;
                        case ".jpg": bitmap.Save(sThumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                        case ".bmp": bitmap.Save(sThumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp); break;
                        case ".png": bitmap.Save(sThumbnailPath, System.Drawing.Imaging.ImageFormat.Png); break;
                    }
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
        }

        private static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //根据 mime 类型，返回编码器
            System.Drawing.Imaging.ImageCodecInfo result = null;
            System.Drawing.Imaging.ImageCodecInfo[] encoders = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    result = encoders[i];
                    break;
                }
            }
            return result;
        }

    }
}
