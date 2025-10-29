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
        /// Identifies rectangular contours in the specified color image using adaptive thresholding and contour analysis.
        /// </summary>
        /// <param name="colorImage"></param>
        /// <param name="thresholdValue"></param>
        /// <param name="invert"></param>
        /// <param name="processedGray"></param>
        /// <param name="processedColor"></param>
        /// <param name="listRectangles"></param>
        /// <returns>The number of contours detected that meet the specified criteria.</returns>
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
                // Conversion To grayscale
                grayImage = colorImage.ToGrayImage();
                bi = new Image<Gray, byte>(grayImage.Width, grayImage.Height);
                color = colorImage.ToBgrImage();

                // tim gia tri thresh de co so ky tu lon nhat
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

                // Assigning output
                processedColor = finalColor.ToBitmap();
                processedGray = finalGrayImage.ToBitmap();

                // Dispose final images after converting to Bitmap
                finalColor.Dispose();
                finalGrayImage.Dispose();
                finalBi.Dispose();
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

    }
}
