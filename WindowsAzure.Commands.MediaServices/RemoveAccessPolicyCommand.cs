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

using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommon.Remove, "AccessPolicy")]
    public class RemoveAccessPolicyCommand : CmdletWithCloudMediaContext
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string AccessPolicyId { get; set; }

        public override void ExecuteCmdlet()
        {
            IAccessPolicy accessPolicy = CloudMediaContext.FindAcessPolicyById(AccessPolicyId);
            if (accessPolicy != null)
            {
                accessPolicy.Delete();
            }
            else
            {
                WriteWarning("Access policy was not found");
            }
        }
    }
}
