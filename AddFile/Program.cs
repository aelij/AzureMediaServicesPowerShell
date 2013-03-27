using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Two10.MediaServices;

namespace AddFile
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("AddFile <AssetId> <filename>, Adds a file to an existing asset");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.FindAssetById(assetId);

            string fileName = args[1];

            // create the AssetFile with the name as the file being uploaded.
            // This is required by the service
            var assetFile = asset.AssetFiles.Create(System.IO.Path.GetFileName(fileName));

            assetFile.Upload(fileName);

            return 0;
        }
    }
}
