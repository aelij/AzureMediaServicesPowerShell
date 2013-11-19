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

namespace CreateAccessPolicy
{
    class NewAccessPolicyCommand
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("CreateAccessPolicy <name> <timeSpan> <accessPermissions>");
                return -1;
            }

            string name = args[0];
            TimeSpan timeSpan = TimeSpan.Parse(args[1]);
            AccessPermissions accessPermissions = AccessPermissions.None;

            switch (args[2].ToLower())
            {
                case "delete": accessPermissions = AccessPermissions.Delete;
                    break;
                case "list": accessPermissions = AccessPermissions.List;
                    break;
                case "none": accessPermissions = AccessPermissions.None;
                    break;
                case "read": accessPermissions = AccessPermissions.Read;
                    break;
                case "write": accessPermissions = AccessPermissions.Write;
                    break;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            IAccessPolicy streamingPolicy = cloudMediaContext.AccessPolicies.Create(name,
                timeSpan,
                accessPermissions);

            Console.WriteLine(streamingPolicy.Id);

            return 0;
        }
    }
}
