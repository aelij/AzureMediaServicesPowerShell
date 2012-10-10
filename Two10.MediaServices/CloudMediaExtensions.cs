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
using System.Linq;

namespace Two10.MediaServices
{
    public static class CloudMediaContextExtensions
    {


        public static void DeleteAllAssets(this CloudMediaContext cloudMediaContext)
        {
            foreach (var asset in cloudMediaContext.Assets)
            {
                try
                {
                    foreach (var key in asset.ContentKeys) {
                        asset.ContentKeys.Remove(key);
                    }
                    
                    cloudMediaContext.Assets.Update(asset);

                    cloudMediaContext.Assets.Delete(asset);
                } 
                catch (Exception e) {
                    Console.Error.WriteLine("Cant delete asset {0} {1}", asset.Id, e);
                }
            }

        }

        public static void DeleteAllJobs(this CloudMediaContext cloudMediaContext)
        {
            foreach (var job in cloudMediaContext.Jobs)
            {
                try
                {
                    job.Delete();
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Cant delete asset {0}", job.Id);
                }
            }
        }

        public static void DeleteAllAccessPolicies(this CloudMediaContext cloudMediaContext)
        {
            foreach (var accessPolicy in cloudMediaContext.AccessPolicies)
            {
                try
                {
                    cloudMediaContext.AccessPolicies.Delete(accessPolicy);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Cant delete policy {0}", accessPolicy.Id);
                }
            }
        }

        public static void DeleteAllContentKeys(this CloudMediaContext cloudMediaContext)
        {
            foreach (var contentKey in cloudMediaContext.ContentKeys)
            {
                try
                {
                    cloudMediaContext.ContentKeys.Delete(contentKey);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Cant delete content key {0} {1}", contentKey.Id, e);
                }
            }
        }

        public static void RevokeAllLocators(this CloudMediaContext cloudMediaContext)
        {
            foreach (var locator in cloudMediaContext.Locators)
            {
                try
                {
                    cloudMediaContext.Locators.Revoke(locator);
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Cant revoke locator {0}", locator.Id);
                }
            }
        }

        public static IAsset GetAssetById(this CloudMediaContext cloudMediaContext, string assetId)
        {
            var assets =
                from a in cloudMediaContext.Assets
                where a.Id == assetId
                select a;

            IAsset asset = assets.FirstOrDefault();

            return asset;
        }

        public static IJob GetJobById(this CloudMediaContext cloudMediaContext, string jobId)
        {
            var jobs =
                from a in cloudMediaContext.Jobs
                where a.Id == jobId
                select a;

            IJob job = jobs.FirstOrDefault();

            return job;
        }

        public static ITask GetTaskById(this CloudMediaContext cloudMediaContext, string taskId)
        {
            foreach (var job in cloudMediaContext.Jobs)
            {
                foreach (var task in job.Tasks)
                {
                    if (task.Id == taskId)
                        return task;
                }
            }

            return null;
        }

        public static void DeleteAssetById(this CloudMediaContext cloudMediaContext, string assetId)
        {
            var assets =
                from a in cloudMediaContext.Assets
                where a.Id == assetId
                select a;

            IAsset asset = assets.FirstOrDefault();

            cloudMediaContext.Assets.Delete(asset);

        }

        public static IMediaProcessor GetMediaProcessor(this CloudMediaContext cloudMediaContext, string mediaProcessor)
        {

            var processors = from p in cloudMediaContext.MediaProcessors
                                where p.Name == mediaProcessor
                                select p;

            IMediaProcessor processor = processors.First();

            if (processor == null)
            {
                throw new ArgumentException(string.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "Unknown processor",
                    mediaProcessor));
            }
            return processor;
        }
    }
}
