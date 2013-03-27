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
            if (args.Length != 3)
            {
                Console.Error.WriteLine("RunTask <AssetId> <Azure Media Processor> <task-preset-file>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            string mediaProcessor = args[1];
            IMediaProcessor processor = null;
            try
            {
                processor = cloudMediaContext.GetMediaProcessor(mediaProcessor);
            }

            catch (Exception)
            {
                Console.WriteLine("Invalid media processor name!");
                Console.WriteLine("List of available processors:");
                var procs = cloudMediaContext.MediaProcessors.ToList();
                foreach (var proc in procs)
                {
                    Console.WriteLine("{0} \n   with id: {1}", proc.Name, proc.Id);
                    Console.WriteLine();
                }
                return -1;
            }

            string filename = args[2];
            IJob job = cloudMediaContext.Jobs.Create("Run Task");

            

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
