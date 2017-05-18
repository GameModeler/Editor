using System.Windows;
using System.Windows.Media.Imaging;

namespace Editor.Utilities
{
    public static class AssetUtils
    {
        public static CroppedBitmap GetCroppedBitmap(int width, int height, BitmapImage source)
        {
            return new CroppedBitmap(source, new Int32Rect(0, 0, width, height));
        }
    }
}
