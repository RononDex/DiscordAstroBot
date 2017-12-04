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
        public static void AddCrossMarker(Bitmap image, int x, int y)
        {
            var g = Graphics.FromImage(image);
            float lineSize = image.Size.Width * 0.01f;
            var pen = (new Pen(Color.GhostWhite, 0.75f));

            // Horizontal line
            g.DrawLine(pen, x - (lineSize / 2), Convert.ToSingle(y), x + (lineSize / 2), Convert.ToSingle(y));

            // Vertical line
            g.DrawLine(pen, Convert.ToSingle(x), y - (lineSize / 2), Convert.ToSingle(x), y + (lineSize / 2));
        }

        /// <summary>
        /// Adds a label at the given position
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddLabel(Bitmap image, int x, int y, float fontSize, bool bold, string text)
        {
            var g = Graphics.FromImage(image);
            var font = new Font(FontFamily.GenericSansSerif, fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
            var brush = new SolidBrush(Color.GhostWhite);

            g.DrawString(text, font, brush, x, y);
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
        public static void AddEllipse(Bitmap image, float x, float y, float sizeX, float sizeY, float rotation, float thickness)
        {
            var g = Graphics.FromImage(image);

            var radius = new[] { sizeX, sizeY }.Max();
            g.DrawEllipse(new Pen(Color.GhostWhite, thickness), x - radius / 2, y - radius / 2, radius, radius);
        }
    }
}
