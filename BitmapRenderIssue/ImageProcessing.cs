using Android.Graphics;

namespace BitmapRenderIssue
{
    public static class ImageProcessing
    {
        public static Bitmap Grayscale_ColorMatrixColorFilter(Bitmap src)
        {
            int width = src.Width;
            int height = src.Height;

            Bitmap dest = Bitmap.CreateBitmap(width, height, Bitmap.Config.Rgb565);

            Canvas canvas = new Canvas(dest);

            Paint paint = new Paint();
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.SetSaturation(0); //value of 0 maps the color to gray-scale
            ColorMatrixColorFilter filter = new ColorMatrixColorFilter(colorMatrix);
            paint.SetColorFilter(filter);
            canvas.DrawBitmap(src, 0, 0, paint);

            return dest;
        }
    }
}