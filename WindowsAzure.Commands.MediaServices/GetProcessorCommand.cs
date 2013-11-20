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

namespace WindowsAzure.Commands.MediaServices
{
    [Cmdlet(VerbsCommon.Get, Constants.CmdletNounPrefix + "Processor")]
    public class GetProcessorCommand : CmdletWithCloudMediaContext
    {
        public override void ExecuteCmdlet()
        {
            foreach (var processor in CloudMediaContext.MediaProcessors)
            {
                WriteObject(processor);
            }
        }
    }
}
