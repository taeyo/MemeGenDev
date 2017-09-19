using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("meme-que");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            var info = new VisionInfo()
            {
                id = "4711",
                BlobName = "Bill.jpg",
                BubbleInfo = new BubbleInfo()
                {
                    Figure = "Normal",
                    Filter = "Comic",
                    Position = "TopLeft",
                    Text = "Bill Bill"
                }
            };

            string messageJson = JsonConvert.SerializeObjectAsync(info).Result;

            // send message to queue
            var message = new CloudQueueMessage(messageJson);
            queue.AddMessageAsync(message).Wait();

            Console.WriteLine(messageJson);

            Console.ReadKey();
        }

    }

    public class VisionInfo
    {
        public string id { get; set; }
        public string BlobName { get; set; }
        public BubbleInfo BubbleInfo { get; set; }
    }

    public class BubbleInfo
    {
        public string Figure { get; set; }
        public string Position { get; set; }
        public string Filter { get; set; }
        public string Text { get; set; }
    }
}
