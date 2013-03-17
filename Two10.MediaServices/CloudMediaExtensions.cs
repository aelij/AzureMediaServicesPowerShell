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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Two10.MediaServices
{
    public static class CloudMediaContextExtensions
    {

        public static IAsset FindAssetById(this CloudMediaContext cloudMediaContext, string assetId)
        {

            foreach (var asset in cloudMediaContext.Assets)
            {
                if (asset.Id.EndsWith(assetId))
                    return asset;
            }

            return null;
        }

        public static IAccessPolicy FindAcessPolicyById(this CloudMediaContext cloudMediaContext, string acessPolicyId)
        {
            foreach (var accessPolicy in cloudMediaContext.AccessPolicies)
            {
                if (accessPolicy.Id.EndsWith(acessPolicyId))
                    return accessPolicy;
            }

            return null;
        }

        public static IJob FindJobById(this CloudMediaContext cloudMediaContext, string jobId)
        {
            foreach (var job in cloudMediaContext.Jobs)
            {
                if (job.Id.EndsWith(jobId))
                    return job;
            }

            return null;
        }

        public static IAssetFile FindFileById(this CloudMediaContext cloudMediaContext, string fileId)
        {
            foreach (var file in cloudMediaContext.Files)
            {
                if (file.Id.EndsWith(fileId))
                    return file;
            }

            return null;
        }

        public static ITask FindTaskById(this CloudMediaContext cloudMediaContext, string taskId)
        {
            foreach (var job in cloudMediaContext.Jobs)
            {
                foreach (var task in job.Tasks)
                {
                    if (task.Id.EndsWith(taskId))
                        return task;
                }
            }

            return null;
        }

        public static IMediaProcessor GetMediaProcessor(this CloudMediaContext cloudMediaContext, string mediaProcessorName)
        {

            var mediaProcessors = from p in cloudMediaContext.MediaProcessors
                where p.Name == mediaProcessorName
                select p;

            IMediaProcessor mediaProcessor = mediaProcessors.First();

            if (mediaProcessor == null)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "Unknown media processor",
                    mediaProcessorName));
            }
            return mediaProcessor;
        }

        static private IAsset CreateEmptyAsset(this CloudMediaContext cloudMediaContext, string assetName, AssetCreationOptions assetCreationOptions)
        {
            var asset = cloudMediaContext.Assets.Create(assetName, assetCreationOptions);

            //Console.WriteLine("Asset name: " + asset.Name);
            //Console.WriteLine("Time created: " + asset.Created.Date.ToString());

            return asset;
        }

        static public IAsset CreateAssetAndUploadSingleFile(this CloudMediaContext cloudMediaContext, AssetCreationOptions assetCreationOptions, string singleFilePath)
        {
            var fileName = Path.GetFileName(singleFilePath);

            var assetName = fileName; // +"-" + DateTime.UtcNow.ToString("o");

            var asset = cloudMediaContext.CreateEmptyAsset(assetName, assetCreationOptions);

            var assetFile = asset.AssetFiles.Create(fileName);

            //Console.WriteLine("Created assetFile {0}", assetFile.Name);

            var accessPolicy = cloudMediaContext.AccessPolicies.Create(assetName, TimeSpan.FromDays(3),
                                                                AccessPermissions.Write | AccessPermissions.List);

            var locator = cloudMediaContext.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);

            //Console.WriteLine("Upload {0}", assetFile.Name);

            assetFile.Upload(singleFilePath);
            //Console.WriteLine("Done uploading of {0} using Upload()", assetFile.Name);

            locator.Delete();
            accessPolicy.Delete();

            return asset;
        }

        static public IAsset CreateAssetAndUploadMultipleFiles(this CloudMediaContext cloudMediaContext, AssetCreationOptions assetCreationOptions, string folderPath)
        {
            var assetName = "UploadMultipleFiles_" + DateTime.UtcNow.ToString();

            var asset = cloudMediaContext.CreateEmptyAsset(assetName, assetCreationOptions);

            var accessPolicy = cloudMediaContext.AccessPolicies.Create(assetName, TimeSpan.FromDays(30),
                                                                AccessPermissions.Write | AccessPermissions.List);
            var locator = cloudMediaContext.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);

            var blobTransferClient = new BlobTransferClient();
            blobTransferClient.NumberOfConcurrentTransfers = 20;
            blobTransferClient.ParallelTransferThreadCount = 20;

            blobTransferClient.TransferProgressChanged += blobTransferClient_TransferProgressChanged;

            var filePaths = Directory.EnumerateFiles(folderPath);

            Console.WriteLine("There are {0} files in {1}", filePaths.Count(), folderPath);

            if (!filePaths.Any())
            {
                throw new FileNotFoundException(String.Format("No files in directory, check folderPath: {0}", folderPath));
            }

            var uploadTasks = new List<Task>();
            foreach (var filePath in filePaths)
            {
                var assetFile = asset.AssetFiles.Create(Path.GetFileName(filePath));
                Console.WriteLine("Created assetFile {0}", assetFile.Name);

                // It is recommended to validate AccestFiles before upload. 
                Console.WriteLine("Start uploading of {0}", assetFile.Name);
                uploadTasks.Add(assetFile.UploadAsync(filePath, blobTransferClient, locator, CancellationToken.None));
            }

            Task.WaitAll(uploadTasks.ToArray());
            Console.WriteLine("Done uploading the files");

            blobTransferClient.TransferProgressChanged -= blobTransferClient_TransferProgressChanged;

            locator.Delete();
            accessPolicy.Delete();

            return asset;
        }

        static void blobTransferClient_TransferProgressChanged(object sender, BlobTransferProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 4) // Avoid startup jitter, as the upload tasks are added.
            {
                Console.WriteLine("{0}% upload competed for {1}.", e.ProgressPercentage, e.LocalFile);
            }
        }

        public static IJob CreateThumbnails(this CloudMediaContext cloudMediaContext, IAsset asset)
        {     
            IJob job = cloudMediaContext.Jobs.Create(string.Format("Thumbnails for {0}", asset.Name));

            IMediaProcessor processor = cloudMediaContext.GetMediaProcessor("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew(string.Format("Thumbnails for {0}", asset.Name),
                processor,
                "Thumbnails",TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(asset);

            task.OutputAssets.AddNew("Output asset",
                AssetCreationOptions.None);

            job.Submit();

            return job;
        }
    }
}
