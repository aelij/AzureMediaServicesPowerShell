using System;
using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommon.Switch, Constants.CmdletNounPrefix + "Account")]
    public class SwitchAccountCommand : CmdletBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            var accountData = CloudMediaAccountStore.Instance.Find(Name);
            if (accountData == null)
            {
                throw new InvalidOperationException("Invalid account name.");
            }
            CloudMediaAccount.Current = accountData;
        }
    }
}