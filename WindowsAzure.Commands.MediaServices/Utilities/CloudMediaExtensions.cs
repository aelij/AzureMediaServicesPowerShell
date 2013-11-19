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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices.Utilities
{
    public static class CloudMediaContextExtensions
    {
        public static IAsset FindAssetById(this CloudMediaContext context, string assetId)
        {
            return Enumerable.FirstOrDefault(context.Assets, asset => asset.Id.EndsWith(assetId));
        }

        public static IAccessPolicy FindAcessPolicyById(this CloudMediaContext context, string acessPolicyId)
        {
            return Enumerable.FirstOrDefault(context.AccessPolicies, accessPolicy => accessPolicy.Id.EndsWith(acessPolicyId));
        }

        public static IJob FindJobById(this CloudMediaContext context, string jobId)
        {
            return Enumerable.FirstOrDefault(context.Jobs, job => job.Id.EndsWith(jobId));
        }

        public static IAssetFile FindFileById(this CloudMediaContext context, string fileId)
        {
            return Enumerable.FirstOrDefault(context.Files, file => file.Id.EndsWith(fileId));
        }

        public static ITask FindTaskById(this CloudMediaContext context, string taskId)
        {
            return Enumerable.FirstOrDefault(context.Jobs.SelectMany(job => job.Tasks), task => task.Id.EndsWith(taskId));
        }

        public static IMediaProcessor GetMediaProcessor(this CloudMediaContext context, string mediaProcessorName)
        {
            var mediaProcessors = from p in context.MediaProcessors
                where p.Name == mediaProcessorName
                select p;

            IMediaProcessor mediaProcessor = mediaProcessors.First();

            if (mediaProcessor == null)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "Unknown media processor {0}",
                    mediaProcessorName));
            }
            return mediaProcessor;
        }

        static private IAsset CreateEmptyAsset(this CloudMediaContext context, string assetName, AssetCreationOptions assetCreationOptions)
        {
            var asset = context.Assets.Create(assetName, assetCreationOptions);

            //Console.WriteLine("Asset name: " + asset.Name);
            //Console.WriteLine("Time created: " + asset.Created.Date.ToString());

            return asset;
        }

        static public IAsset CreateAssetAndUploadSingleFile(this CloudMediaContext context, AssetCreationOptions assetCreationOptions, string singleFilePath)
        {
            var fileName = Path.GetFileName(singleFilePath);

            var assetName = fileName; // +"-" + DateTime.UtcNow.ToString("o");

            var asset = context.CreateEmptyAsset(assetName, assetCreationOptions);

            var assetFile = asset.AssetFiles.Create(fileName);

            //Console.WriteLine("Created assetFile {0}", assetFile.Name);

            var accessPolicy = context.AccessPolicies.Create(assetName, TimeSpan.FromDays(3),
                                                                AccessPermissions.Write | AccessPermissions.List);

            var locator = context.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);

            //Console.WriteLine("Upload {0}", assetFile.Name);

            assetFile.Upload(singleFilePath);
            //Console.WriteLine("Done uploading of {0} using Upload()", assetFile.Name);

            locator.Delete();
            accessPolicy.Delete();

            return asset;
        }

        static public IAsset CreateAssetAndUploadMultipleFiles(this CloudMediaContext context, AssetCreationOptions assetCreationOptions, string folderPath)
        {
            var assetName = "UploadMultipleFiles_" + DateTime.UtcNow;

            var asset = context.CreateEmptyAsset(assetName, assetCreationOptions);

            var accessPolicy = context.AccessPolicies.Create(assetName, TimeSpan.FromDays(30),
                                                                AccessPermissions.Write | AccessPermissions.List);
            var locator = context.Locators.CreateLocator(LocatorType.Sas, asset, accessPolicy);

            var blobTransferClient = new BlobTransferClient
            {
                NumberOfConcurrentTransfers = 20,
                ParallelTransferThreadCount = 20
            };

            blobTransferClient.TransferProgressChanged += blobTransferClient_TransferProgressChanged;

            var filePaths = Directory.GetFiles(folderPath);

            Console.WriteLine("There are {0} files in {1}", filePaths.Length, folderPath);

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

        public static IJob CreateThumbnails(this CloudMediaContext context, IAsset asset)
        {
            IJob job = context.Jobs.Create(string.Format("Thumbnails for {0}", asset.Name));

            IMediaProcessor processor = context.GetMediaProcessor("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew(string.Format("Thumbnails for {0}", asset.Name),
                processor,
                "Thumbnails",TaskOptions.ProtectedConfiguration);

            task.InputAssets.Add(asset);

            task.OutputAssets.AddNew("Output asset",
                AssetCreationOptions.None);

            job.Submit();

            return job;
        }

        public static ILocator GetStreamingOriginLocator(this CloudMediaContext context, IAsset assetToStream)
        {
            

            // Get a reference to the streaming manifest file from the  
            // collection of files in the asset. 
            var theManifest =
                                from f in assetToStream.AssetFiles
                                where f.Name.EndsWith(".ism")
                                select f;
            // Cast the reference to a true IAssetFile type. 
            IAssetFile manifestFile = theManifest.First();

            // Create a 30-day readonly access policy. 
            IAccessPolicy policy = context.AccessPolicies.Create("Streaming policy",
                TimeSpan.FromDays(30),
                AccessPermissions.Read);

            // Create a locator to the streaming content on an origin. 
            ILocator originLocator = context.Locators.CreateLocator(LocatorType.OnDemandOrigin, assetToStream,
                policy,
                DateTime.UtcNow.AddMinutes(-5));

            // Display some useful values based on the locator.
            // Display the base path to the streaming asset on the origin server.
            Console.WriteLine("Streaming asset base path on origin: ");
            Console.WriteLine(originLocator.Path);
            Console.WriteLine();
            // Create a full URL to the manifest file. Use this for playback
            // in streaming media clients. 
            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest";
            Console.WriteLine("URL to manifest for client streaming: ");
            Console.WriteLine(urlForClientStreaming);
            Console.WriteLine();
            // Display the ID of the origin locator, the access policy, and the asset.
            Console.WriteLine("Origin locator Id: " + originLocator.Id);
            Console.WriteLine("Access policy Id: " + policy.Id);
            Console.WriteLine("Streaming asset Id: " + assetToStream.Id);

            // Return the locator. 
            return originLocator;
        }

        public static IMediaProcessor GetLatestMediaProcessorByName(this CloudMediaContext context, string mediaProcessorName)
        {
            // The possible strings that can be passed into the
            // method for the mediaProcessor parameter:
            //   Windows Azure Media Encoder
            //   Windows Azure Media Packager
            //   Windows Azure Media Encryptor
            //   Storage Decryption

            var processor = context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException(string.Format("Unknown media processor {0}", mediaProcessorName));

            return processor;
        }

    }
}
