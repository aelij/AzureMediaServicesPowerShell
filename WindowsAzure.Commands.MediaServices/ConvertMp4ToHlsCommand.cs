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
using Two10.MediaServices;

namespace MP4ToHLS
{
    class ConvertMp4ToHlsCommand
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("MP4ToHLS <asset-id>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            IJob job = cloudMediaContext.Jobs.Create(string.Format("Convert {0} to HLS Asset", asset.Name));

            IMediaProcessor processor = cloudMediaContext.GetMediaProcessor("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew("MP4 to HLS",
                processor,
                "H264 Adaptive Bitrate MP4 Set SD 16x9",
                Microsoft.WindowsAzure.MediaServices.Client.TaskOptions.ProtectedConfiguration);


            task.InputAssets.Add(asset);


            task.OutputAssets.AddNew(string.Format("HLS for {0}", asset.Name),
                AssetCreationOptions.None);

            job.Submit();

            Console.WriteLine(job.Id);
            Console.WriteLine("Once job is complete, retrieve StreamingURL and append (format=m3u8-aapl)");
            return 0;
        }
    }
}
