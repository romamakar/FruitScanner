using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace MLLibrary
{
    public static class MLAnalyzer
    {
        private static Dictionary<string, string> tags = new Dictionary<string, string>();
          
        static MLAnalyzer()
        {
            tags.Add("apple", "Яблуко");
            tags.Add("banana", "Банан");
            tags.Add("egg", "Яйце");
            tags.Add("milk", "Молоко");
            tags.Add("chese", "Чир");
            tags.Add("cucumber", "Огірок");
            tags.Add("tomato", "Помідор");
            tags.Add("pepper", "Перець");
            tags.Add("carrot", "Морква");
        }   
        public static (string, string) Analyze(string filename)
        {
            MLModel1.ModelOutput result;
            using (var image = (Bitmap)System.Drawing.Image.FromFile(filename))
            {
                var sampleData = new MLModel1.ModelInput()
                {
                    ImageSource = image,
                };
                result = MLModel1.Predict(sampleData);
            }
            var newImage = System.Drawing.Image.FromFile(filename);
            var originalWidth = newImage.Width;
            var originalHeight = newImage.Height;
            using (Graphics g = Graphics.FromImage(newImage))
            {
                foreach(var boundingBox in result.BoundingBoxes.Where(x => x.Score > 0.7).ToList())
                {
                    float x = Math.Max(boundingBox.Left, 0);
                    float y = Math.Max(boundingBox.Top, 0);
                    float width = Math.Min(originalWidth - x, boundingBox.Right);
                    float height = Math.Min(originalHeight - y, boundingBox.Bottom);

                    // fit to current image size
                    x = originalWidth * x / 800;
                    y = originalHeight * y / 600;
                    width = originalWidth * width / 800;
                    height = originalHeight * height / 600;

                    var pen = Pens.Black;
                    var font = new Font("Arial", 20);
                    g.DrawString(boundingBox.Label + " " + boundingBox.Score, font, Brushes.Black, x, y);
                    g.DrawRectangle(pen, x, y, width, height);

                }
            }
            var newName = "1"+ Path.GetFileName(filename);
            var newPath = Path.GetDirectoryName(filename);
            var list = result.BoundingBoxes.Where(x => x.Score > 0.7).Select(x => x.Label).GroupBy(x => x).Select(x => $"{tags[x.Key]} {x.Count()} шт.");
            newImage.Save(newPath + "//" + newName);
            return (newPath + "//" + newName, string.Join('\n', list));
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

    }
}