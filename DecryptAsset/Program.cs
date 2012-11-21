#region Copyright (c) 2012 Two10degrees and Active Web Solutions Ltd
//
// (C) Copyright 2012 Two10degrees and Active Web Solutions Ltd
//      All rights reserved.
//
// This software is provided "as is" without warranty of any kind,
// express or implied, including but not limited to warranties as to
// quality and fitness for a particular purpose. Active Web Solutions Ltd
// does not support the Software, nor does it warrant that the Software
// will meet your requirements or that the operation of the Software will
// be uninterrupted or error free or that any defects will be
// corrected. Nothing in this statement is intended to limit or exclude
// any liability for personal injury or death caused by the negligence of
// Active Web Solutions Ltd, its employees, contractors or agents.
//
#endregion

using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using Two10.MediaServices;

namespace DecryptAsset
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("DecryptAsset <asset-id>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            IJob job = cloudMediaContext.Jobs.Create(string.Format("Decrypt {0}", asset.Name));

            IMediaProcessor decryptProcessor = cloudMediaContext.GetMediaProcessor("Storage Decryption");

            ITask decryptTask = job.Tasks.AddNew(string.Format("Decrypt {0}", asset.Name),
                    decryptProcessor,string.Empty,
                    Microsoft.WindowsAzure.MediaServices.Client.TaskOptions.None);

            decryptTask.InputAssets.Add(asset);

            decryptTask.OutputAssets.AddNew(string.Format("{0} decrypted", asset.Name),
                true,
                AssetCreationOptions.None);

            job.Submit();

            Console.WriteLine(job.Id);

            return 0;       
        }
    }
}
