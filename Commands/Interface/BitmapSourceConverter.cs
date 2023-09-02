using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace bimtab.Commands.Interface
{
    public static class BitmapSourceConverter
    {
        public static BitmapSource ConvertFromImage(Bitmap image)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(),IntPtr.Zero,System.Windows.Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
