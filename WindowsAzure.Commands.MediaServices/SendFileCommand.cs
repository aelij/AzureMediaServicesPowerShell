using System.IO;
using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommunications.Send, Constants.CmdletNounPrefix + "File")]
    public class SendFileCommand : CmdletWithCloudMediaContext
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AssetId { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public FileInfo LocalFilePath { get; set; }
        
        public override void ExecuteCmdlet()
        {
            IAsset asset = CloudMediaContext.FindAssetById(AssetId);

            // create the AssetFile with the name as the file being uploaded.
            // This is required by the service
            var assetFile = asset.AssetFiles.Create(Path.GetFileName(LocalFilePath.FullName));

            assetFile.Upload(LocalFilePath.FullName);
        }
    }
}
