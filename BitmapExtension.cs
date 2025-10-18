using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;

namespace Auto_parking
{
    /// <summary>
    /// Helper class for Emgu.CV 3.4.1 conversions between Bitmap and Image
    /// </summary>
    public static class BitmapExtension
    {
        /// <summary>
        /// Convert a Bitmap to Image<Gray, byte>
        /// </summary>
        public static Image<Gray, byte> ToGrayImage(this Bitmap bitmap)
        {
            using (Mat mat = new Mat())
            {
                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    bitmap.PixelFormat);

                int channels = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                DepthType depth = DepthType.Cv8U;

                using (Mat tempMat = new Mat(bitmap.Height, bitmap.Width, DepthType.Cv8U, channels, data.Scan0, data.Stride))
                {
                    if (channels == 1)
                    {
                        tempMat.CopyTo(mat);
                    }
                    else
                    {
                        CvInvoke.CvtColor(tempMat, mat, ColorConversion.Bgr2Gray);
                    }
                }

                bitmap.UnlockBits(data);
                return mat.ToImage<Gray, byte>();
            }
        }

        /// <summary>
        /// Convert a Bitmap to Image<Bgr, byte>
        /// </summary>
        public static Image<Bgr, byte> ToBgrImage(this Bitmap bitmap)
        {
            using (Mat mat = new Mat())
            {
                BitmapData data = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    bitmap.PixelFormat);

                int channels = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;

                using (Mat tempMat = new Mat(bitmap.Height, bitmap.Width, DepthType.Cv8U, channels, data.Scan0, data.Stride))
                {
                    if (channels == 3 || channels == 4)
                    {
                        tempMat.CopyTo(mat);
                    }
                    else
                    {
                        CvInvoke.CvtColor(tempMat, mat, ColorConversion.Gray2Bgr);
                    }
                }

                bitmap.UnlockBits(data);
                return mat.ToImage<Bgr, byte>();
            }
        }
    }
}
