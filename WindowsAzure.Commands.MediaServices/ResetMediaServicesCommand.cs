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

using System;
using System.Management.Automation;
using WindowsAzure.Commands.MediaServices.Utilities;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices
{
    /// <summary>
    /// Clears down a media services account prior to demo
    /// USE WITH CARE
    /// </summary>
    [Cmdlet(VerbsCommon.Reset, Constants.CmdletNounPrefix + "Services", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class ResetMediaServicesCommand : CmdletWithCloudMediaContext
    {
        public override void ExecuteCmdlet()
        {
            if (ShouldProcess("Reset all media services"))
            {
                if (ShouldContinue("", ""))
                {

                    DeleteAllJobs(CloudMediaContext);
                    RevokeAllLocators(CloudMediaContext);
                    DeleteAllAssets(CloudMediaContext);
                    DeleteAllAccessPolicies(CloudMediaContext);
                }
            }
        }

        private void WriteError(string messageFormat, string id, Exception e)
        {
            WriteError(
                new ErrorRecord(new Exception(string.Format(messageFormat, id, e), e),
                    string.Empty, ErrorCategory.WriteError, null));
        }

        private void DeleteAllAssets(CloudMediaContext context)
        {
            foreach (var asset in context.Assets)
            {
                try
                {
                    foreach (var key in asset.ContentKeys)
                    {
                        asset.ContentKeys.Remove(key);
                    }

                    asset.Update();

                }
                catch (Exception e)
                {
                    WriteError("Cant remove content keys for asset {0}, {1}", asset.Id, e);
                }
            }

            foreach (var asset in context.Assets)
            {
                try
                {
                    asset.Delete();
                }
                catch (Exception e)
                {
                    WriteError("Cant delete asset {0}, {1}", asset.Id, e);
                }
            }

        }

        private void DeleteAllJobs(CloudMediaContext context)
        {
            foreach (var job in context.Jobs)
            {
                try
                {
                    job.Delete();
                }
                catch (Exception e)
                {
                    WriteError("Cant delete Job {0}, {1}", job.Id, e);
                }
            }
        }

        private void DeleteAllAccessPolicies(CloudMediaContext context)
        {
            foreach (var accessPolicy in context.AccessPolicies)
            {
                try
                {
                    accessPolicy.Delete();
                }
                catch (Exception e)
                {
                    WriteError("Cant delete access policy {0}, {1}", accessPolicy.Id, e);
                }
            }
        }

        private void RevokeAllLocators(CloudMediaContext context)
        {
            foreach (var locator in context.Locators)
            {
                try
                {
                    //context.Locators.Revoke(locator);
                }
                catch (Exception e)
                {
                    WriteError("Cant revoke locator {0}, {1}", locator.Id, e);
                }
            }
        }
    }
}
