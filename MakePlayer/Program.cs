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
using System.IO;
using System.Linq;
using Two10.MediaServices;

namespace MakePlayer
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("MakePlayer <streaming-asset-id>");
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

            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest";

            

            string buffer = File.ReadAllText("c:\\dropbox\\code\\MediaServicesCommandLineTools\\SmoothStreamingPlayer\\SmoothStreamingPlayer.html");

            buffer = buffer.Replace("http://streams.smooth.vertigo.com/elephantsdream/Elephants_Dream_1024-h264-st-aac.ism/manifest",
                urlForClientStreaming);

            string filename = string.Format("c:\\dropbox\\code\\MediaServicesCommandLineTools\\SmoothStreamingPlayer\\{0}.html", asset.Id.Replace(":", ""));

            File.WriteAllText(filename, buffer);

            Console.WriteLine(filename);

            return 0;
        }
    }
}
