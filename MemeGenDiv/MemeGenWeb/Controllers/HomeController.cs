using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MemeGenWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public enum BubblePosition
        {
            TopLeft, TopRight, BottomLeft, BottomRight,
            TopLeftStar, TopRightStar, BottomLeftStar
        }

        public enum Filter
        {
            None, BlackWhite, Comic, Gotham, GreyScale, Lomograph, Polaroid, HiSatch, Sepia
        }

        public IMatrixFilter GetFilterByName(Filter filter)
        {
            switch (filter)
            {
                case Filter.None:
                    return null;
                case Filter.Comic:
                    return MatrixFilters.Comic;
                case Filter.BlackWhite:
                    return MatrixFilters.BlackWhite;
                case Filter.Gotham:
                    return MatrixFilters.Gotham;
                case Filter.GreyScale:
                    return MatrixFilters.GreyScale;
                case Filter.Lomograph:
                    return MatrixFilters.Lomograph;
                case Filter.Polaroid:
                    return MatrixFilters.Polaroid;
                case Filter.HiSatch:
                    return MatrixFilters.HiSatch;
                case Filter.Sepia:
                    return MatrixFilters.Sepia;
                default:
                    return MatrixFilters.Lomograph;
            }
        }

        public class BubbleInfo
        {
            public Point Startpoint { get; set; }
            public string ImageName { get; set; }
            public Rectangle TextArea { get; set; }
        }

        public BubbleInfo GetBubbleInfo(BubblePosition position)
        {
            BubbleInfo info = new BubbleInfo();
            //Point p = new Point();
            switch (position)
            {
                case BubblePosition.TopLeft:
                    info.Startpoint = new Point(0, 0);
                    info.ImageName = "bubble-tl.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
                case BubblePosition.TopLeftStar:
                    info.Startpoint = new Point(0, 0);
                    info.ImageName = "bubble-star2tl.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
                case BubblePosition.TopRight:
                    info.Startpoint = new Point(200, 0);
                    info.ImageName = "bubble-tr.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
                case BubblePosition.TopRightStar:
                    info.Startpoint = new Point(200, 0);
                    info.ImageName = "bubble-star2tr.png";
                    info.TextArea = new Rectangle(70, 0, 200, 140);
                    return info;
                case BubblePosition.BottomLeft:
                    info.Startpoint = new Point(0, 400);
                    info.ImageName = "bubble-bl.png";
                    info.TextArea = new Rectangle(10, 60, 200, 120);
                    return info;
                case BubblePosition.BottomLeftStar:
                    info.Startpoint = new Point(0, 400);
                    info.ImageName = "bubble-star2bl.png";
                    info.TextArea = new Rectangle(5, 60, 200, 130);
                    return info;
                case BubblePosition.BottomRight:
                    info.Startpoint = new Point(200, 400);
                    info.ImageName = "bubble-star2br.png";
                    info.TextArea = new Rectangle(70, 60, 200, 140);
                    return info;
                default:
                    info.Startpoint = new Point(0, 0);
                    info.ImageName = "bubble-tl.png";
                    info.TextArea = new Rectangle(10, 10, 200, 120);
                    return info;
            }
        }

        private byte[] ReadBlobInByteArray(string fileName)
        {
            var connectionString = ConfigurationManager.AppSettings["AzureWebJobsStorage"];
            var containerName = ConfigurationManager.AppSettings["BlobContainerName"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the destination blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            var blob = container.GetBlockBlobReference(fileName);
            long fileByteLength = blob.StreamWriteSizeInBytes;
            Byte[] myByteArray = new Byte[fileByteLength];

            try
            {
                blob.DownloadToByteArray(myByteArray, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("a");
            }
            return myByteArray;
        }

        public ActionResult About(string t)
        {
            // BLOB에서 바이너리를 가져오는 메서드
            string originFileName = "choihun.jpg";
            string targetFileName = "choihun-Sepia.jpg";
            //byte[] photoBytes = ReadBlobInByteArray(fileName);

            // 필요한 데이터,
            // 1. BLOB 파일명
            // 2. 밈 텍스트
            // 3. 버블 모양
            // 4. 버블 위치

            string assetsPath = ConfigurationManager.AppSettings["ASSETS_ROOT"] ?? @"C:\Temp\images\";
            string file = Path.Combine(assetsPath, originFileName);
            byte[] photoBytes = System.IO.File.ReadAllBytes(file);

            // 이미지 변환 메서드 호출
            byte[] outBytes = ImageGenerate(photoBytes);
            
            // 웹 테스트인 경우, 이미지를 파일로 로컬 디렉토리에 저장
            using (var fileStream = System.IO.File.Create(Path.Combine(assetsPath, targetFileName)))
            {
                fileStream.Write(outBytes, 0, outBytes.Length);
            }

            return View();
        }

        private byte[] ImageGenerate(byte[] photoBytes)
        {
            string assetsPath = ConfigurationManager.AppSettings["ASSETS_ROOT"] ?? @"C:\Temp\images\";

            string textDiplay = "빌 클린턴";
            //  None, BlackWhite, Comic, Gotham, GreyScale, Lomograph, Polaroid, HiSatch, Sepia
            IMatrixFilter appliedFilter = MatrixFilters.Sepia;

            // 가정
            // 1. 이미지의 widht 는 400으로 한다. (resize 필요)
            // 2. 버블은 좌,우 및 상,하로 구분한다

            var bubbleInfo = GetBubbleInfo(BubblePosition.TopLeftStar);

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new JpegFormat { Quality = 70 };
            //Size size = new Size(150, 0);

            Bitmap bitmap = (Bitmap)Image.FromFile(Path.Combine(assetsPath, bubbleInfo.ImageName));

            #region 강제 텍스트 출력예
            //string firstText = "내 이름은";
            //string secondText = "빌 게이츠";

            //PointF firstLocation = new PointF(10f, 10f);
            //PointF secondLocation = new PointF(10f, 50f);

            //using (Graphics graphics = Graphics.FromImage(bitmap))
            //{
            //    using (Font arialFont = new Font("Malgun Gothic", 20, FontStyle.Bold))
            //    {
            //        graphics.DrawString(firstText, arialFont, Brushes.Black, firstLocation);
            //        graphics.DrawString(secondText, arialFont, Brushes.Black, secondLocation);
            //    }
            //}
            #endregion
            
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (Font font1 = new Font("Segoe UI", 120, FontStyle.Bold, GraphicsUnit.Pixel))
            {
                Rectangle rect1 = bubbleInfo.TextArea;

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Font goodFont = FindFont(graphics, textDiplay, rect1.Size, font1);
                graphics.DrawString(textDiplay, goodFont, Brushes.Black, rect1, stringFormat);
            }

            //Image img = (Image)bitmap;
            Image img = bitmap as Image;

            using (MemoryStream inStream = new MemoryStream(photoBytes))
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
                        layer.Position = bubbleInfo.Startpoint;

                        //필터 적용?
                        // Comic, Gotham, GreyScale, Lomograph, Polaroid, HiSatch, Sepia
                        var filter = appliedFilter;

                        // Load, resize, set the format and quality and save an image.
                        var factory = imageFactory.Load(inStream)
                                                //.Resize(size)
                                                .Format(format);

                        if (appliedFilter != null)
                            factory.Filter(filter);

                        factory
                            .Overlay(layer)
                            .Save(outStream);
                    }

                    return outStream.ToArray();

                    //// Do something with the stream.
                    //using (var fileStream = System.IO.File.Create(Path.Combine(assetsPath, targetFileName)))
                    //{
                    //    outStream.Seek(0, SeekOrigin.Begin);
                    //    outStream.CopyTo(fileStream);
                    //}
                }
            }
        }

        private Font FindFont(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont)
        {
            //you should perform some scale functions!!!
            SizeF RealSize = g.MeasureString(longString, PreferedFont);

            //font 길이 보정
            RealSize.Width += 150;

            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;

            if (ScaleFontSize < 12) ScaleFontSize = 12f;
            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}