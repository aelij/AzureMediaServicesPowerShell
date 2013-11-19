using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices.Utilities
{
    public class CmdletWithCloudMediaContext : CmdletWithSubscriptionBase
    {
        protected CloudMediaContext CloudMediaContext
        {
            get; set;            
        }
    }
}
