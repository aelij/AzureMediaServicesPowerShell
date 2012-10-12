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
using Two10.MediaServices;

namespace StreamingUrl
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("StreamingUrl <asset-id>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];

            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            var manifest = from f in asset.Files
                                where f.Name.EndsWith(".ism")
                                select f;

            IFileInfo manifestFile = manifest.First();

            IAccessPolicy streamingPolicy = cloudMediaContext.AccessPolicies.Create("Streaming policy",
                TimeSpan.FromDays(1),
                AccessPermissions.Read);

            ILocator originLocator = cloudMediaContext.Locators.CreateOriginLocator(asset,
                streamingPolicy,
                DateTime.UtcNow.AddMinutes(-5));

            string streamingUrl = originLocator.Path + manifestFile.Name + "/manifest";

            Console.WriteLine(streamingUrl);

            return 0;
        }
    }
}
