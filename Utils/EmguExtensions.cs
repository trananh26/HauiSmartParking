using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Auto_parking
{
    /// <summary>
    /// Extension methods for Emgu.CV 3.x Image to Bitmap conversions
    /// </summary>
    public static class EmguExtensions
    {
        /// <summary>
        /// Convert Image<Gray, byte> to Bitmap
        /// </summary>
        public static Bitmap ToBitmap(this Image<Gray, byte> image)
        {
            // Create a new bitmap
            Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);

            // Set grayscale palette
            ColorPalette palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bitmap.Palette = palette;

            // Lock bitmap data
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            // Copy data from Image to Bitmap
            Marshal.Copy(image.Bytes, 0, bitmapData.Scan0, image.Bytes.Length);

            // Unlock and return
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        /// <summary>
        /// Convert Image<Bgr, byte> to Bitmap
        /// </summary>
        public static Bitmap ToBitmap(this Image<Bgr, byte> image)
        {
            // Create a new bitmap
            Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Lock bitmap data
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            // Copy data from Image to Bitmap
            Marshal.Copy(image.Bytes, 0, bitmapData.Scan0, image.Bytes.Length);

            // Unlock and return
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
