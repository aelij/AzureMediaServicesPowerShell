#region Copyright (c) 2012 - 2013 Two10 Degrees Ltd
//
// (C) Copyright 2012 - 2013 Two10 Degrees Ltd
//      All rights reserved.
//
// This software is provided "as is" without warranty of any kind,
// express or implied, including but not limited to warranties as to
// quality and fitness for a particular purpose. Two10 Degrees Ltd
// does not support the Software, nor does it warrant that the Software
// will meet your requirements or that the operation of the Software will
// be uninterrupted or error free or that any defects will be
// corrected. Nothing in this statement is intended to limit or exclude
// any liability for personal injury or death caused by the negligence of
// Two10 Degrees Ltd, its employees, contractors or agents.
//
#endregion

using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.IO;
using Two10.MediaServices;

namespace PlayReady
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 6)
            {
                Console.Error.WriteLine("PlayReady <asset-id> <contentKey> <customAttributes> <keyId> <keySeedValue> <licenseAcquisitionUrl>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            string contentKey = args[1];
            string customAttributes = args[2];
            string keyId = args[3];
            string keySeedValue = args[4];
            string licenseAcquisitionUrl = args[5];

            IJob job = cloudMediaContext.Jobs.Create("PlayReady Protection Job");

            IMediaProcessor processor = cloudMediaContext.GetMediaProcessor("PlayReady Protection Task");

            string configuration = File.ReadAllText(Path.GetFullPath("PlayReady Protection.xml"));
    
            configuration = String.Format(configuration, contentKey, customAttributes, keyId, keySeedValue, licenseAcquisitionUrl);

            ITask task = job.Tasks.AddNew("My PlayReady Protection Task",
                processor,
                configuration,
               Microsoft.WindowsAzure.MediaServices.Client.TaskOptions.ProtectedConfiguration);

            // Specify the input asset to be encoded.
            task.InputAssets.Add(asset);    
            // Add an output asset to contain the results of the job. Since the
            // asset is already protected with PlayReady, we won't encrypt. 
            task.OutputAssets.AddNew("Output asset",
                AssetCreationOptions.None);

            // Launch the job. 
            job.Submit();

            return 0;
        }
    }
}
