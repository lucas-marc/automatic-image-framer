using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AutomaticImageFramer
{
    public static class ImageHelper
    {
        public static Bitmap DrawBitmapWithBorder(Bitmap bmp, int borderSize = 10, System.Windows.Media.Color? color = null)
        {
            int newWidth = bmp.Width + (borderSize * 2);
            int newHeight = bmp.Height + (borderSize * 2);
            Color colorToUse = color.HasValue ?
                                    Color.FromArgb(color.Value.A,
                                                    color.Value.R,
                                                    color.Value.G,
                                                    color.Value.B)
                                    : Color.Black;

            System.Drawing.Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics gfx = Graphics.FromImage(newImage))
            {
                using (Brush border = new SolidBrush(colorToUse))
                {
                    gfx.FillRectangle(border, 0, 0,
                        newWidth, newHeight);
                }
                gfx.DrawImage(bmp, new Rectangle(borderSize, borderSize, bmp.Width, bmp.Height));

            }

            return (Bitmap)newImage;
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        [DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource GetBitmapSourceFromBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }

        public static BitmapSource FrameBitmapSource(BitmapSource image, int borderWidth, System.Windows.Media.Color? color)
        {
            var bmp = BitmapFromSource(image);
            bmp = DrawBitmapWithBorder(bmp, borderWidth,color);
            return GetBitmapSourceFromBitmap(bmp);
        }

    }
}
