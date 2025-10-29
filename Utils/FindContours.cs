using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;

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
        public int IdentifyContours(
            Bitmap colorImage,
            int thresholdValue,
            bool invert,
            out Bitmap processedGray,
            out Bitmap processedColor,
            out List<Rectangle> listRectangles)
        {
            listRectangles = new List<Rectangle>();
            Image<Gray, byte> grayImage = null;
            Image<Gray, byte> bi = null;
            Image<Bgr, byte> color = null;
            Image<Gray, byte> src_b = null;
            Image<Gray, byte> bi_b = null;
            Image<Bgr, byte> color_b = null;

            try
            {
                #region Conversion To grayscale
                grayImage = colorImage.ToGrayImage();
                bi = new Image<Gray, byte>(grayImage.Width, grayImage.Height);
                color = colorImage.ToBgrImage();
                #endregion

                #region tim gia tri thresh de co so ky tu lon nhat

                double thr = grayImage.GetAverage().Intensity;
                if (thr == 0)
                {
                    thr = 128; // Default fallback
                }

                Rectangle[] li = new Rectangle[9];
                color_b = colorImage.ToBgrImage();
                src_b = grayImage.Clone();
                bi_b = bi.Clone();

                int c_best = 0;

                for (double value = 0; value <= 127; value += 3)
                {
                    for (int s = -1; s <= 1 && s + value != 1; s += 2)
                    {
                        Image<Bgr, byte> color2 = null;
                        Image<Gray, byte> bi2 = null;
                        Image<Gray, byte> src = null;

                        try
                        {
                            color2 = colorImage.ToBgrImage();
                            bi2 = bi.Clone();
                            listRectangles.Clear();
                            int c = 0;
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
                                            listRectangles.Add(rect);
                                        }
                                    }
                                }
                            }

                            // Remove overlapping rectangles
                            double avg_h = 0;
                            double dis = 0;
                            for (int i = 0; i < c; i++)
                            {
                                avg_h += listRectangles[i].Height;
                                for (int j = i + 1; j < c; j++)
                                {
                                    if ((listRectangles[j].X < (listRectangles[i].X + listRectangles[i].Width) && listRectangles[j].X > listRectangles[i].X)
                                 && (listRectangles[j].Y < (listRectangles[i].Y + listRectangles[i].Width) && listRectangles[j].Y > listRectangles[i].Y))
                                    {
                                        listRectangles.RemoveAt(j);
                                        c--;
                                        j--;
                                    }
                                    else if ((listRectangles[i].X < (listRectangles[j].X + listRectangles[j].Width) && listRectangles[i].X > listRectangles[j].X)
                                              && (listRectangles[i].Y < (listRectangles[j].Y + listRectangles[j].Width) && listRectangles[i].Y > listRectangles[j].Y))
                                    {
                                        avg_h -= listRectangles[i].Height;
                                        listRectangles.RemoveAt(i);
                                        c--;
                                        i--;
                                        break;
                                    }
                                }
                            }

                            if (c > 0)
                            {
                                avg_h = avg_h / c;
                                for (int i = 0; i < c; i++)
                                {
                                    dis += Math.Abs(avg_h - listRectangles[i].Height);
                                }
                            }

                            if (c <= 8 && c > 1 && c > c_best && dis <= c * 8)
                            {
                                listRectangles.CopyTo(li);
                                c_best = c;

                                // Dispose old best images before replacing
                                if (color_b != null) color_b.Dispose();
                                if (bi_b != null) bi_b.Dispose();
                                if (src_b != null) src_b.Dispose();

                                color_b = color2;
                                bi_b = bi2;
                                src_b = src;

                                // Set to null to prevent disposal in finally block
                                color2 = null;
                                bi2 = null;
                                src = null;
                            }
                        }
                        finally
                        {
                            // Dispose temporary images if not saved as best
                            if (color2 != null) color2.Dispose();
                            if (bi2 != null) bi2.Dispose();
                            if (src != null) src.Dispose();
                        }
                    }
                    if (c_best == 8) break;
                }

                count = c_best;

                // Transfer ownership to output images
                Image<Gray, byte> finalGrayImage = src_b;
                Image<Bgr, byte> finalColor = color_b;
                Image<Gray, byte> finalBi = bi_b;

                // Prevent disposal of transferred images
                src_b = null;
                color_b = null;
                bi_b = null;

                listRectangles.Clear();
                for (int i = 0; i < li.Length; i++)
                {
                    if (li[i].Height != 0) listRectangles.Add(li[i]);
                }

                #endregion

                #region Assigning output
                processedColor = finalColor.ToBitmap();
                processedGray = finalGrayImage.ToBitmap();

                // Dispose final images after converting to Bitmap
                finalColor.Dispose();
                finalGrayImage.Dispose();
                finalBi.Dispose();
                #endregion
            }
            finally
            {
                // Cleanup all remaining images
                if (grayImage != null) grayImage.Dispose();
                if (bi != null) bi.Dispose();
                if (color != null) color.Dispose();
                if (src_b != null) src_b.Dispose();
                if (bi_b != null) bi_b.Dispose();
                if (color_b != null) color_b.Dispose();
            }

            return count;
        }

        private double cout_avg(Image<Gray, byte> src)
        {
            double d = 0;
            List<Rectangle> lsR = new List<Rectangle>();
            Image<Gray, byte> grayImage = null;
            Image<Gray, byte> dilated = null;
            Image<Gray, byte> eroded = null;

            try
            {
                grayImage = new Image<Gray, byte>(src.Width, src.Height);
                CvInvoke.AdaptiveThreshold(src, grayImage, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 21, 2);
                dilated = grayImage.Dilate(3);
                eroded = dilated.Erode(3);

                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(eroded, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

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

                Bitmap tmpBitmap = null;
                try
                {
                    tmpBitmap = src.ToBitmap();

                    for (int i = 0; i < lsR.Count; i++)
                    {
                        Bitmap tmp2 = null;
                        Image<Gray, byte> tmp3 = null;

                        try
                        {
                            tmp2 = tmpBitmap.Clone(lsR[i], tmpBitmap.PixelFormat);
                            tmp3 = tmp2.ToGrayImage();
                            d += tmp3.GetAverage().Intensity / lsR.Count;
                        }
                        finally
                        {
                            if (tmp2 != null) tmp2.Dispose();
                            if (tmp3 != null) tmp3.Dispose();
                        }
                    }
                }
                finally
                {
                    if (tmpBitmap != null) tmpBitmap.Dispose();
                }
            }
            finally
            {
                if (grayImage != null) grayImage.Dispose();
                if (dilated != null) dilated.Dispose();
                if (eroded != null) eroded.Dispose();
            }

            return d;
        }

        private double cout_avg_new(Image<Gray, byte> src)
        {
            double d = 0;
            List<Rectangle> lsR = new List<Rectangle>();
            Image<Gray, byte> grayImage = null;
            Image<Gray, byte> dilated = null;
            Image<Gray, byte> eroded = null;

            try
            {
                grayImage = new Image<Gray, byte>(src.Width, src.Height);
                CvInvoke.AdaptiveThreshold(src, grayImage, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 21, 2);

                dilated = grayImage.Dilate(3);
                eroded = dilated.Erode(3);

                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(eroded, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);

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

                Bitmap tmpBitmap = null;
                try
                {
                    tmpBitmap = src.ToBitmap();

                    for (int i = 0; i < lsR.Count; i++)
                    {
                        Bitmap tmp2 = null;
                        Image<Gray, byte> tmp3 = null;

                        try
                        {
                            tmp2 = tmpBitmap.Clone(lsR[i], tmpBitmap.PixelFormat);
                            tmp3 = tmp2.ToGrayImage();

                            int T = 0;
                            int T0 = 128;
                            do
                            {
                                T = T0;
                                int m = 0, M = 0;
                                int min = 0, max = 0;
                                for (int y = 0; y < tmp3.Rows; y++)
                                {
                                    for (int x = 0; x < tmp3.Cols; x++)
                                    {
                                        int value = tmp3.Data[y, x, 0];
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
                                }

                                if (m > 0 && M > 0)
                                {
                                    T0 = (min / m + max / M) / 2;
                                }
                                else
                                {
                                    break;
                                }
                            } while (T - T0 > 1 || T0 - T > 1);

                            d += T0 / (double)lsR.Count;
                        }
                        finally
                        {
                            if (tmp2 != null) tmp2.Dispose();
                            if (tmp3 != null) tmp3.Dispose();
                        }
                    }
                }
                finally
                {
                    if (tmpBitmap != null) tmpBitmap.Dispose();
                }
            }
            finally
            {
                if (grayImage != null) grayImage.Dispose();
                if (dilated != null) dilated.Dispose();
                if (eroded != null) eroded.Dispose();
            }

            return d;
        }

        private Image<Gray, byte> search(double thr, Image<Gray, byte> grayImage, double min, double max
   , out List<Rectangle> list_out, out int count, Image<Bgr, byte> color, out Image<Bgr, byte> color_out,
  Image<Gray, byte> bi, out Image<Gray, byte> bi_out)
        {
            List<Rectangle> listR = new List<Rectangle>();
            List<Rectangle> list_best = new List<Rectangle>();
            Image<Bgr, byte> color2 = null;
            Image<Gray, byte> src = null;
            Image<Gray, byte> bi2 = null;
            Image<Bgr, byte> color_best = null;
            Image<Gray, byte> bi_best = null;
            Image<Gray, byte> src_best = null;

            int c_best = 0;

            try
            {
                for (double value = min; value <= max; value += 0.1)
                {
                    // Dispose previous iteration objects
                    if (color2 != null) color2.Dispose();
                    if (src != null) src.Dispose();
                    if (bi2 != null) bi2.Dispose();

                    listR.Clear();
                    int c = 0;

                    double t = thr / value;
                    src = grayImage.ThresholdBinary(new Gray(t), new Gray(255));
                    color2 = color.Clone();
                    bi2 = bi.Clone();

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

                        // Remove overlapping rectangles
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

                        // Dispose old best images
                        if (color_best != null) color_best.Dispose();
                        if (bi_best != null) bi_best.Dispose();
                        if (src_best != null) src_best.Dispose();

                        // Save current as best
                        color_best = color2;
                        bi_best = bi2;
                        src_best = src;

                        // Prevent disposal
                        color2 = null;
                        bi2 = null;
                        src = null;

                        if (c == 8)
                        {
                            break;
                        }
                    }
                }

                // Set output values
                color_out = color_best ?? color.Clone();
                bi_out = bi_best ?? bi.Clone();
                list_out = list_best;
                count = c_best;

                Image<Gray, byte> result = src_best ?? src;

                // Prevent disposal of returned object
                src_best = null;
                src = null;
                color_best = null;
                bi_best = null;

                return result;
            }
            finally
            {
                // Cleanup
                if (color2 != null) color2.Dispose();
                if (src != null) src.Dispose();
                if (bi2 != null) bi2.Dispose();
                if (color_best != null) color_best.Dispose();
                if (bi_best != null) bi_best.Dispose();
                if (src_best != null) src_best.Dispose();
            }
        }
    }
}
