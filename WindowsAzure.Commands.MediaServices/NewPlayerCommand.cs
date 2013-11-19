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
using System.IO;
using System.Linq;
using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices
{        
    /// <summary>
    /// Creates an Origin locator for a streaming asset and creates
    /// an HTML file that wraps a Silverlight Player to play it.
    /// </summary>
    public class NewPlayerCommand : CmdletWithCloudMediaContext
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AssetId { get; set; }

        public override void ExecuteCmdlet()
        {
            IAsset asset = CloudMediaContext.FindAssetById(AssetId);

            var manifest = from f in asset.AssetFiles
                           where f.Name.EndsWith(".ism")
                           select f;

            var manifestFile = manifest.First();

            IAccessPolicy streamingPolicy = CloudMediaContext.AccessPolicies.Create("Streaming policy",
                TimeSpan.FromDays(1),
                AccessPermissions.Read);

            ILocator originLocator = CloudMediaContext.Locators.CreateLocator(LocatorType.OnDemandOrigin, asset,
                streamingPolicy);

            string urlForClientStreaming = originLocator.Path + manifestFile.Name + "/manifest";

            string buffer = File.ReadAllText("..\\SmoothStreamingPlayer\\SmoothStreamingPlayer.html");

            buffer = buffer.Replace("http://streams.smooth.vertigo.com/elephantsdream/Elephants_Dream_1024-h264-st-aac.ism/manifest",
                urlForClientStreaming);

            string filename = string.Format("..\\SmoothStreamingPlayer\\{0}.html", asset.Id.Replace(":", ""));

            File.WriteAllText(filename, buffer);
            
            WriteObject(filename);
        }
    }
}
