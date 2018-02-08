using SkiaSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAstroBot.Utilities
{
    public static class ImageUtility
    {
        /// <summary>
        /// Creates a grey scale image out of a single channel RGB image (where only R channel has data)
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Bitmap MakeGrayscaleFromRGB_R(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }

        public static void ConvertImageToJpg(string filePath, string savePath)
        {
            using (var image = Image.FromFile(filePath))
            {
                image.Save(savePath, ImageFormat.Jpeg);
            }
        }

        /// <summary>
        /// Crops the given image to the defined parameters
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CropImage(Bitmap b, int x, int y, int width, int height)
        {
            var r = new Rectangle(x, y, width, height);
            return b.Clone(r, b.PixelFormat);
        }

        /// <summary>
        /// Adds a cross marker into the image at the given position
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sizeInPixels"></param>
        public static void AddCrossMarker(SKBitmap image, int x, int y, float width)
        {
            using (var canvas = new SKCanvas(image))
            {
                using (var paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.IsAntialias = true;
                    paint.Color = SKColors.GhostWhite;
                    paint.StrokeWidth = width;
                    paint.StrokeCap = SKStrokeCap.Round;

                    var lineSize = image.Width * 0.01f;

                    canvas.DrawLine(Convert.ToSingle(x - (lineSize / 2)), Convert.ToSingle(y), x + (lineSize / 2), Convert.ToSingle(y), paint);
                    canvas.DrawLine(Convert.ToSingle(x), Convert.ToSingle(y - (lineSize / 2)), Convert.ToSingle(x), y + (lineSize / 2), paint);
                }                
            }
        }

        /// <summary>
        /// Adds a label at the given position
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddLabel(SKBitmap image, int x, int y, float fontSize, bool bold, string text)
        {
            using (var canvas = new SKCanvas(image))
            {
                using (var paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Fill;
                    paint.IsAntialias = true;
                    paint.Color = SKColors.GhostWhite;
                    paint.TextAlign = SKTextAlign.Left;
                    paint.TextSize = fontSize;
                    paint.FakeBoldText = bold;

                    canvas.DrawText(text, x, y, paint);
                }
            }
        }

        /// <summary>
        /// Adds an ellipse onto the image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="rotation"></param>
        public static void AddEllipse(SKBitmap image, float x, float y, float sizeX, float sizeY, float rotation, float thickness)
        {
            using (var canvas = new SKCanvas(image))
            {
                using (var paint = new SKPaint())
                {
                    paint.Style = SKPaintStyle.Stroke;
                    paint.IsAntialias = true;
                    paint.Color = SKColors.GhostWhite;
                    paint.StrokeWidth = thickness;

                    canvas.RotateDegrees(rotation, x, y);
                    canvas.DrawOval(x, y, sizeX, sizeY, paint);
                }
            }
        }
    }
}
