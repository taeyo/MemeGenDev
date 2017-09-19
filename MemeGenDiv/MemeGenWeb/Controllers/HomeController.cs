using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Filters.Photo;
using ImageProcessor.Imaging.Formats;
using MemeImgGen;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private byte[] ReadBlobInByteArray(string fileName)
        {
            var connectionString = ConfigurationManager.AppSettings["AzureWebJobsStorage"];
            var containerName = ConfigurationManager.AppSettings["BlobContainerName"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the destination blob client
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);

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
            // 필요한 데이터,
            // 1. BLOB 파일명
            // 2. 밈 텍스트
            // 3. 버블 모양
            // 4. 버블 위치
            // 5. 필터 이름

            // BLOB에서 바이너리를 가져오는 메서드
            string originFileName = @"input\taeyoung.jpg";
            string targetFileName = @"output\taeyoung-out.jpg";
            //byte[] photoBytes = ReadBlobInByteArray(fileName);
            string textDiplay = "Happy Happy Happy";
            string bubbleFigure = "Normal";
            string bubblePos = "BottomRight";
            string filter = "polaroid";
            //none, blackwhite, comic, gotham, greyscale, lomograph, polaroid, hisatch, sepia

            string assetsPath = ConfigurationManager.AppSettings["ASSETS_ROOT"] ?? @"C:\Temp\images\";
            string file = Path.Combine(assetsPath, originFileName);
            byte[] photoBytes = System.IO.File.ReadAllBytes(file);

            byte[] outBytes = CallFunction(photoBytes, textDiplay, bubbleFigure, bubblePos, filter);

            // 웹 테스트인 경우, 이미지를 파일로 로컬 디렉토리에 저장

            using (var fileStream = System.IO.File.Create(Path.Combine(assetsPath, targetFileName)))
            {
                fileStream.Write(outBytes, 0, outBytes.Length);
            }

            return View();
        }

        private static byte[] CallFunction(byte[] photoBytes, string textDiplay, 
            string bubbleFigure, string bubblePos, string filter)
        {
            string assetsPath = ConfigurationManager.AppSettings["ASSETS_ROOT"] ?? @"C:\Temp\images\";

            bubbleFigure = bubbleFigure.ToUpper();
            bubblePos = bubblePos.ToUpper();
            filter = filter.ToUpper();

            var imgGen = new MemeImgGen.ImageProcessor(assetsPath);

            BubbleFigure figure = (BubbleFigure)Enum.Parse(typeof(BubbleFigure), bubbleFigure);
            BubblePosition position = (BubblePosition)Enum.Parse(typeof(BubblePosition), bubblePos);
            BubbleFilter bFilter = (BubbleFilter)Enum.Parse(typeof(BubbleFilter), filter);
            IMatrixFilter bubbleFilter = imgGen.GetFilterByName(bFilter);
                        
            // 이미지 변환 메서드 호출
            byte[] outBytes = imgGen.ImageGenerate(photoBytes, textDiplay, figure, position, bubbleFilter);
            return outBytes;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}