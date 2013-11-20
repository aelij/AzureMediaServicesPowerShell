using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommon.Get, Constants.CmdletNounPrefix + "Account")]
    public class GetAccountCommand : CmdletBase
    {
        public override void ExecuteCmdlet()
        {
            foreach (var accountData in CloudMediaAccountStore.Instance.Accounts)
            {
                WriteObject(accountData);
            }
        }
    }
}