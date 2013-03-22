using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Two10.MediaServices;

namespace RunTask
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("RunTask <AssetId> <task-preset-file>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            string filename = args[1];

            IJob job = cloudMediaContext.Jobs.Create("Run Task");

            IMediaProcessor processor = cloudMediaContext.GetMediaProcessor("Windows Azure Media Encoder");


            string configuration = File.ReadAllText(Path.GetFullPath(filename));

            ITask task = job.Tasks.AddNew("My Task",
                processor,
                configuration,
               Microsoft.WindowsAzure.MediaServices.Client.TaskOptions.None);

            // Specify the input asset to be encoded.
            task.InputAssets.Add(asset);
            // Add an output asset to contain the results of the job. Since the
            // asset is already protected with PlayReady, we won't encrypt. 
            task.OutputAssets.AddNew("Output asset",
                AssetCreationOptions.None);


            // Launch the job. 
            job.Submit();

            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", job.Id, job.Name, job.State, job.RunningDuration, job.LastModified);


            return 0;
        }
    }
}
