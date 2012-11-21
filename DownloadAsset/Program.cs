using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.IO;
using Two10.MediaServices;

namespace DownloadAsset
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("DownloadAsset <asset-id> - downloads all asset files.");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            string folder = assetId.ToString().Replace(":","");

            Directory.CreateDirectory(folder);


            //foreach (var a in cloudMediaContext.AccessPolicies)
            //{
            //    a.Delete();

            //}

         

            foreach (var file in asset.AssetFiles)
            {
                file.Download(string.Format(@"{0}\{1}" ,folder ,file.Name));
            }

            return 0;
        }
    }
}
