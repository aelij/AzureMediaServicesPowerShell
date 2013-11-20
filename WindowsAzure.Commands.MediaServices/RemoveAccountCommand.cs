using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommon.Remove, Constants.CmdletNounPrefix + "Account")]
    public class RemoveAccountCommand : CmdletBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudMediaAccountStore.Instance.Remove(Name);
        }
    }
}