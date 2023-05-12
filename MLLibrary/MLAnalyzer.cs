using System.Drawing;

namespace MLLibrary
{
    public static class MLAnalyzer
    {
        private static Dictionary<string, string> tags = new Dictionary<string, string>();

        static MLAnalyzer()
        {
            tags.Add("apple", "Яблуко");
            tags.Add("yodurt", "Йогурт");
            tags.Add("beer", "Пиво");
        }
        public static (string, string) Analyze(string path, string filename)
        {
            var image = (Bitmap)Bitmap.FromFile(path+"//"+filename);
            var sampleData = new MLModel.ModelInput()
            {
                ImageSource = image,
            };
            var result = MLModel.Predict(sampleData);
            using (Graphics g = Graphics.FromImage(image))
            {
                
                for (int i = 0; i < result.BoundingBoxes.Length; i++)
                {
                 
                    var pen = Pens.Black;
                    var font = new Font("Arial", 20);
                    g.DrawString(result.BoundingBoxes[i].Label + " " + result.BoundingBoxes[i].Score, font, Brushes.Black, result.BoundingBoxes[i].Left, result.BoundingBoxes[i].Top);
                    g.DrawRectangle(pen, result.BoundingBoxes[i].Left, result.BoundingBoxes[i].Top, result.BoundingBoxes[i].Right, result.BoundingBoxes[i].Bottom);
                   
                }
            }
            var list = result.BoundingBoxes.Where(x => x.Score > 0.1).Select(x => x.Label).GroupBy(x => x).Select(x => $"{tags[x.Key]} {x.Count()}");
            image.Save(path + "//1" + filename);
            return (path + "//1" + filename, string.Join('\n', list));
        }
    }
}