using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using ImageProcessor;
using ImageProcessor.Imaging;
using System.Configuration;

namespace MemeGen
{
    public static class MemeGen
    {
        [StorageAccount("AzureWebJobsStorage")]
        [FunctionName("MemeBlob")]
        public static async void Run(
            [BlobTrigger("meme-input/{name}")]byte[] inputBlob,
            string name,
            [Blob("meme-output/{name}", FileAccess.Write)]Stream outputBlob,
            TraceWriter log)
        {
            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            Size size = new Size(150, 0);

            string firstText = "내 이름은";
            string secondText = "빌 게이츠";

            PointF firstLocation = new PointF(10f, 10f);
            PointF secondLocation = new PointF(10f, 50f);

            string assetsPath = ConfigurationManager.AppSettings["ASSETS_ROOT"] ?? @"C:\Temp\images\";
            assetsPath = Path.Combine(assetsPath, "bubble-tp.png");

            Bitmap bitmap = (Bitmap)Image.FromFile(assetsPath);

            //using (Graphics graphics = Graphics.FromImage(bitmap))
            //{
            //    using (Font arialFont = new Font("Malgun Gothic", 20, FontStyle.Bold))
            //    {
            //        graphics.DrawString(firstText, arialFont, Brushes.Black, firstLocation);
            //        graphics.DrawString(secondText, arialFont, Brushes.Black, secondLocation);
            //    }
            //}
            string textDiplay = "visual";

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (Font font1 = new Font("Impact", 120, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                Rectangle rect1 = new Rectangle(10, 0, 200, 100);

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Font goodFont = FindFont(graphics, textDiplay, rect1.Size, font1);
                graphics.DrawString(textDiplay, goodFont, Brushes.Black, rect1, stringFormat);
            }
            
            Image img = bitmap as Image;
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream inStream = new MemoryStream(inputBlob))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        //Image img = Image.FromFile(@"C:\Temp\images\bubble-tp.png");
                        ImageLayer layer = new ImageLayer();
                        layer.Image = img;
                        layer.Opacity = 80;
                        layer.Position = new Point(0, 0);

                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                                    .Format(format)
                                    .Overlay(layer)
                                    .Save(outStream);
                    }

                    var byteArray = outStream.ToArray();
                    await outputBlob.WriteAsync(byteArray, 0, byteArray.Length);

                    // Do something with the stream.
                    //using (var fileStream = System.IO.File.Create(@"C:\Temp\images\bill2.jpg"))
                    //{
                    //    outStream.Seek(0, SeekOrigin.Begin);
                    //    outStream.CopyTo(fileStream);
                    //}
                }
            }

        }

        private static Font FindFont(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
            //you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }
    }
}
