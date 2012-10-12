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
    }
}
