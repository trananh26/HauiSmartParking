using Emgu.CV;
using Emgu.CV.Structure;
using System;
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
            if (image == null)
                throw new ArgumentNullException(nameof(image));

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

            try
            {
                // Get stride information
                int imageStride = image.MIplImage.WidthStep;
                int bitmapStride = Math.Abs(bitmapData.Stride);
                int copyWidth = Math.Min(imageStride, bitmapStride);

                // Get pointers
                IntPtr srcPtr = image.MIplImage.ImageData;
                IntPtr dstPtr = bitmapData.Scan0;

                // Copy row by row to handle stride differences
                for (int y = 0; y < image.Height; y++)
                {
                    CopyMemory(dstPtr, srcPtr, (uint)copyWidth);

                    srcPtr = IntPtr.Add(srcPtr, imageStride);
                    dstPtr = IntPtr.Add(dstPtr, bitmapStride);
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        /// <summary>
        /// Convert Image<Bgr, byte> to Bitmap
        /// </summary>
        public static Bitmap ToBitmap(this Image<Bgr, byte> image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            // Create a new bitmap
            Bitmap bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);

            // Lock bitmap data
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            try
            {
                // Get stride information
                int imageStride = image.MIplImage.WidthStep;
                int bitmapStride = Math.Abs(bitmapData.Stride);
                int copyWidth = Math.Min(imageStride, bitmapStride);

                // Get pointers
                IntPtr srcPtr = image.MIplImage.ImageData;
                IntPtr dstPtr = bitmapData.Scan0;

                // Copy row by row to handle stride differences
                for (int y = 0; y < image.Height; y++)
                {
                    CopyMemory(dstPtr, srcPtr, (uint)copyWidth);

                    srcPtr = IntPtr.Add(srcPtr, imageStride);
                    dstPtr = IntPtr.Add(dstPtr, bitmapStride);
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
    }
}
