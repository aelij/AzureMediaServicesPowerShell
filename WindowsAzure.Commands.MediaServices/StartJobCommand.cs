﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsLifecycle.Start, Constants.CmdletNounPrefix + "Job")]
    public class StartJobCommand : CmdletWithCloudMediaContext
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AssetId { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public FileInfo PresetFile { get; set; }

        public override void ExecuteCmdlet()
        {
            IAsset asset = CloudMediaContext.FindAssetById(AssetId);

            IMediaProcessor processor = null;

            Console.WriteLine("Choose Media processor: ");
            try
            {
                var processors = new Dictionary<int, IMediaProcessor>();
                var procs = CloudMediaContext.MediaProcessors.ToList();
                int i = 1;
                foreach (var proc in procs)
                {
                    Console.WriteLine("{0}. {1} \n      with id: {2}", i, proc.Name, proc.Id);
                    Console.WriteLine();
                    processors.Add(i, proc);
                    i++;
                }
                Console.Write("Enter [1..n]:");
                var key = Console.ReadKey();
                Console.WriteLine();
                if (char.IsDigit(key.KeyChar))
                {
                    int result = key.KeyChar - '0';
                    processor = processors[result];
                }

            }
            catch
            {
                throw new CmdletInvocationException("Could not get processor(s)!");
            }

            if (processor == null)
            {
                throw new CmdletInvocationException("Invalid choice");
            }

            IJob job = CloudMediaContext.Jobs.Create("Run Task");

            string configuration = File.ReadAllText(PresetFile.FullName);

            ITask task = job.Tasks.AddNew("My Task",
                processor,
                configuration,
               TaskOptions.None);

            task.InputAssets.Add(asset);
            task.OutputAssets.AddNew("Output asset", AssetCreationOptions.None);

            job.Submit();

            WriteObject(job);
        }
    }
}
