using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;

namespace Auto_parking
{
    class FindContours
    {
        public int count = 0;
        /// <summary>
        /// Method used to process the image and set the output result images.
        /// </summary>
        /// <param name="colorImage">Source color image.</param>
        /// <param name="thresholdValue">Value used for thresholding.</param>
        /// <param name="processedGray">Resulting gray image.</param>
        /// <param name="processedColor">Resulting color image.</param>
        public int IdentifyContours(Bitmap colorImage, int thresholdValue, bool invert, out Bitmap processedGray, out Bitmap processedColor, out List<Rectangle> list)
        {
            List<Rectangle> listR = new List<Rectangle>();
            #region Conversion To grayscale
            Image<Gray, byte> grayImage = colorImage.ToGrayImage();
            //grayImage = grayImage.Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear);
            Image<Gray, byte> bi = new Image<Gray, byte>(grayImage.Width, grayImage.Height);
            Image<Bgr, byte> color = colorImage.ToBgrImage();

            #endregion


            #region tim gia tri thresh de co so ky tu lon nhat

            double thr = 0;
            if (thr == 0)
            {
                thr = grayImage.GetAverage().Intensity;
            }

            Rectangle[] li = new Rectangle[9];
            Image<Bgr, byte> color_b = colorImage.ToBgrImage();
            Image<Gray, byte> src_b = grayImage.Clone();
            Image<Gray, byte> bi_b = bi.Clone();
            Image<Bgr, byte> color2;
            Image<Gray, byte> src;
            Image<Gray, byte> bi2;
            int c = 0, c_best = 0;
            
            for (double value = 0; value <= 127; value += 3)
            {
                for (int s = -1; s <= 1 && s + value != 1; s += 2)
                {
                    color2 = colorImage.ToBgrImage();
                    bi2 = bi.Clone();
                    listR.Clear();
                    c = 0;
                    double t = 127 + value * s;
                    src = grayImage.ThresholdBinary(new Gray(t), new Gray(255));

                    // Use FindContours with Emgu.CV 3.x API
                    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                    {
                        CvInvoke.FindContours(src, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                        
                        for (int i = 0; i < contours.Size; i++)
                        {
                            using (VectorOfPoint contour = contours[i])
                            {
                                Rectangle rect = CvInvoke.BoundingRectangle(contour);
                                
                                // Draw contours
                                CvInvoke.DrawContours(color2, contours, i, new MCvScalar(255, 255, 0), 1);
                                
                                double ratio = (double)rect.Width / rect.Height;
                                if (rect.Width > 20 && rect.Width < 150
                                    && rect.Height > 80 && rect.Height < 180
                                    && ratio > 0.1 && ratio < 1.1 && rect.X > 20)
                                {
                                    c++;
                                    CvInvoke.DrawContours(color2, contours, i, new MCvScalar(0, 255, 255), 3);

                                    color2.Draw(rect, new Bgr(Color.Green), 2);
                                    CvInvoke.DrawContours(bi2, contours, i, new MCvScalar(255), -1);
                                    listR.Add(rect);
                                }
                            }
                        }
                    }
                    
                    double avg_h = 0;
                    double dis = 0;
                    for (int i = 0; i < c; i++)
                    {
                        avg_h += listR[i].Height;
                        for (int j = i + 1; j < c; j++)
                        {
                            if ((listR[j].X < (listR[i].X + listR[i].Width) && listR[j].X > listR[i].X)
                                && (listR[j].Y < (listR[i].Y + listR[i].Width) && listR[j].Y > listR[i].Y))
                            {
                                listR.RemoveAt(j);
                                c--;
                                j--;
                            }
                            else if ((listR[i].X < (listR[j].X + listR[j].Width) && listR[i].X > listR[j].X)
                                && (listR[i].Y < (listR[j].Y + listR[j].Width) && listR[i].Y > listR[j].Y))
                            {
                                avg_h -= listR[i].Height;
                                listR.RemoveAt(i);
                                c--;
                                i--;
                                break;
                            }

                        }
                    }
                    avg_h = avg_h / c;
                    for (int i = 0; i < c; i++)
                    {
                        dis += Math.Abs(avg_h - listR[i].Height);
                    }

                    if (c <= 8 && c > 1 && c > c_best && dis <= c * 8)
                    {
                        listR.CopyTo(li);
                        c_best = c;
                        color_b = color2;
                        bi_b = bi2;
                        src_b = src;
                    }
                }
                if (c_best == 8) break;
            }

            count = c_best;
            grayImage = src_b;
            color = color_b;
            bi = bi_b;
            listR.Clear();
            for (int i = 0; i < li.Length; i++)
            {
                if (li[i].Height != 0) listR.Add(li[i]);
            }

            #endregion

            #region Asigning output
            processedColor = color.ToBitmap();
            processedGray = grayImage.ToBitmap();
            list = listR;
            #endregion
            return count;
        }
        
        private double cout_avg(Image<Gray, byte> src)
        {
            double d = 0;
            List<Rectangle> lsR = new List<Rectangle>();
            Image<Gray, byte> grayImage = new Image<Gray, byte>(src.Width, src.Height);
            CvInvoke.AdaptiveThreshold(src, grayImage, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 21, 2);
            grayImage = grayImage.Dilate(3);
            grayImage = grayImage.Erode(3);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(grayImage, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contour);

                        if (rect.Width > 50 && rect.Width < 150
                            && rect.Height > 80 && rect.Height < 150)
                        {
                            lsR.Add(rect);
                        }
                    }
                }
            }

            for (int i = 0; i < lsR.Count; i++)
            {
                Bitmap tmp = src.ToBitmap();
                Bitmap tmp2 = tmp.Clone(lsR[i], tmp.PixelFormat);
                Image<Gray, byte> tmp3 = tmp2.ToGrayImage();
                d += tmp3.GetAverage().Intensity / lsR.Count;
            }

            return d;
        }
        
        private double cout_avg_new(Image<Gray, byte> src)
        {
            double d = 0;
            List<Rectangle> lsR = new List<Rectangle>();
            Image<Gray, byte> grayImage = new Image<Gray, byte>(src.Width, src.Height);

            CvInvoke.AdaptiveThreshold(src, grayImage, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 21, 2);

            grayImage = grayImage.Dilate(3);
            grayImage = grayImage.Erode(3);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(grayImage, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                
                for (int i = 0; i < contours.Size; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contour);
                        if (rect.Width > 50 && rect.Width < 150
                            && rect.Height > 80 && rect.Height < 150)
                        {
                            lsR.Add(rect);
                        }
                    }
                }
            }

            for (int i = 0; i < lsR.Count; i++)
            {
                Bitmap tmp = src.ToBitmap();
                Bitmap tmp2 = tmp.Clone(lsR[i], tmp.PixelFormat);
                Image<Gray, byte> tmp3 = tmp2.ToGrayImage();
                int T = 0;
                int T0 = 128;
                do
                {
                    T = T0;
                    int m = 0, M = 0;
                    int min = 0, max = 0;
                    for (int y = 0; y < tmp3.Rows; y++)
                        for (int x = 0; x < tmp3.Cols; x++)
                        {
                            int value = (int)tmp3.Data[y, x, 0];
                            if (value <= T)
                            {
                                m++;
                                min += value;
                            }
                            else
                            {
                                M++;
                                max += value;
                            }
                        }
                    T0 = (min / m + max / M) / 2;
                } while (T - T0 > 1 || T0 - T > 1);


                d += (double)T0 / (double)lsR.Count;
            }

            return d;
        }

        private Image<Gray, byte> search(double thr, Image<Gray, byte> grayImage, double min, double max
            , out List<Rectangle> list_out, out int count, Image<Bgr, byte> color, out Image<Bgr, byte> color_out,
            Image<Gray, byte> bi, out Image<Gray, byte> bi_out)
        {
            List<Rectangle> listR = new List<Rectangle>(), list_best = new List<Rectangle>();
            Image<Bgr, byte> color2 = color;
            Image<Gray, byte> src = grayImage;
            Image<Gray, byte> bi2 = bi;

            int c = 0, c_best = 0;
            for (double value = min; value <= max; value += 0.1)
            {

                double t = thr / value;
                src = grayImage.ThresholdBinary(new Gray(t), new Gray(255));

                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(src, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
                    
                    for (int i = 0; i < contours.Size; i++)
                    {
                        using (VectorOfPoint contour = contours[i])
                        {
                            Rectangle rect = CvInvoke.BoundingRectangle(contour);
                            CvInvoke.DrawContours(color2, contours, i, new MCvScalar(255, 255, 0), 1);
                            
                            if (rect.Width > 20 && rect.Width < 150
                                && rect.Height > 80 && rect.Height < 150)
                            {
                                c++;
                                CvInvoke.DrawContours(color2, contours, i, new MCvScalar(0, 255, 255), 3);

                                color2.Draw(rect, new Bgr(Color.Green), 2);
                                CvInvoke.DrawContours(bi2, contours, i, new MCvScalar(255), -1);
                                listR.Add(rect);
                            }
                        }
                    }
                    
                    for (int i = 0; i < c; i++)
                    {
                        for (int j = i + 1; j < c; j++)
                        {
                            if ((listR[j].X < (listR[i].X + listR[i].Width) && listR[j].X > listR[i].X)
                                && (listR[j].Y < (listR[i].Y + listR[i].Width) && listR[j].Y > listR[i].Y))
                            {
                                listR.RemoveAt(j);
                                c--;
                                j--;
                            }
                            else if ((listR[i].X < (listR[j].X + listR[j].Width) && listR[i].X > listR[j].X)
                                && (listR[i].Y < (listR[j].Y + listR[j].Width) && listR[i].Y > listR[j].Y))
                            {
                                listR.RemoveAt(i);
                                c--;
                                i--;
                                break;
                            }

                        }
                    }
                }
                if (c <= 8 && c > c_best)
                {
                    list_best = new List<Rectangle>(listR);
                    c_best = c;
                    if (c == 8)
                    {
                        color_out = color2;
                        bi_out = bi2;
                        list_out = list_best;
                        count = c_best;
                        return src;
                    }
                }
            }
            color_out = color2;
            bi_out = bi2;
            list_out = list_best;
            count = c_best;
            return src;
        }
    }
}
