using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace MemeGen
{
    public static class MemeBlob
    {
        //[StorageAccount("AzureWebJobsStorage")]
        //[FunctionName("MemeBlob")]
        //public static async void Run(
        //    [BlobTrigger("meme-input/{name}")]Stream inputBlob, 
        //    string name,
        //    [Blob("meme-output/{name}", FileAccess.Write)]Stream outputBlob,
        //    TraceWriter log)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        inputBlob.CopyTo(ms);
        //        var byteArray = ms.ToArray();
        //        await outputBlob.WriteAsync(byteArray, 0, byteArray.Length);
        //    }

        //    log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");
        //    //outputBlob.
        //    //await inputBlob.CopyToAsync(outputBlob);
        //}
    }
}
