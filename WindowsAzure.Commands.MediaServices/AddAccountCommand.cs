using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommon.Add, Constants.CmdletNounPrefix + "Account")]
    public class AddAccountCommand : CmdletBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Key { get; set; }

        public override void ExecuteCmdlet()
        {
            var accountData = new MediaAccountData { Name = Name, Key = Key };
            CloudMediaAccountStore.Instance.Add(accountData);
            if (CloudMediaAccount.Current == null)
            {
                CloudMediaAccount.Current = accountData;
            }
        }
    }
}
