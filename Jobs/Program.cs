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

using Microsoft.WindowsAzure.MediaServices.Client;
using System;

namespace Jobs
{
    class Program
    {
        static void Main(string[] args)
        {
            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");

            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            foreach (var job in cloudMediaContext.Jobs)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", job.Id, job.Name, job.State, job.RunningDuration, job.LastModified);
            }


        }
    }
}
