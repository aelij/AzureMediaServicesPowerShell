using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.WindowsAzure.Commands.Utilities.Common;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace WindowsAzure.Commands.MediaServices.Utilities
{
    public class CmdletWithCloudMediaContext : CmdletBase
    {
        private readonly Lazy<CloudMediaContext> _cloudMediaContext = new Lazy<CloudMediaContext>(() =>
        {
            var accountData = CloudMediaAccount.Current;
            if (accountData == null)
            {
                throw new InvalidOperationException("No account selected. Use Add-MediaAccount.");
            }
            return new CloudMediaContext(accountData.Name, accountData.Key);
        });

        protected CloudMediaContext CloudMediaContext
        {
            get { return _cloudMediaContext.Value; }
        }
    }

    [Cmdlet(VerbsCommon.Add, "MediaAccount")]
    public class AddMediaAccountCommand : CmdletBase
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

    [Cmdlet(VerbsCommon.Remove, "MediaAccount")]
    public class RemoveMediaAccountCommand : CmdletBase
    {
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            CloudMediaAccountStore.Instance.Remove(Name);
        }
    }

    [Cmdlet(VerbsCommon.Switch, "MediaAccount")]
    public class SwitchMediaAccountCommand : CmdletBase
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

    [Cmdlet(VerbsCommon.Get, "MediaAccount")]
    public class GetMediaAccountCommand : CmdletBase
    {
        public override void ExecuteCmdlet()
        {
            foreach (var accountData in CloudMediaAccountStore.Instance.Accounts)
            {
                WriteObject(accountData);
            }
        }
    }

    [DataContract]
    internal class MediaAccountStoreData
    {
        [DataMember]
        public MediaAccountData[] Accounts { get; set; }
    }

    [DataContract]
    internal class MediaAccountData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Key { get; set; }
    }

    internal class CloudMediaAccount
    {
        public static MediaAccountData Current { get; set; }
    }

    internal class CloudMediaAccountStore
    {
        private const string DefaulStoreName = "MediaAccountStore.xml";
        private const string DefaultAppDataFolder = "Windows Azure Powershell";

        private static readonly string DefaultPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DefaultAppDataFolder), DefaulStoreName);

        private static readonly Lazy<CloudMediaAccountStore> _instance = new Lazy<CloudMediaAccountStore>(() => new CloudMediaAccountStore());

        public static CloudMediaAccountStore Instance
        {
            get { return _instance.Value; }
        }

        private CloudMediaAccountStore()
        {

        }

        private List<MediaAccountData> _accounts;

        public void Add(MediaAccountData accountData)
        {
            EnsureLoaded();
            var accountIndex = _accounts.FindIndex(t => t.Name == accountData.Name);
            if (accountIndex >= 0)
            {
                _accounts[accountIndex] = accountData;
            }
            else
            {
                _accounts.Add(accountData);
            }
            Save();
        }

        public void Remove(string name)
        {
            _accounts.RemoveAll(t => t.Name == name);
            Save();
        }

        public MediaAccountData Find(string name)
        {
            return _accounts.Find(t => t.Name == name);
        }

        public IReadOnlyCollection<MediaAccountData> Accounts
        {
            get { return new ReadOnlyCollection<MediaAccountData>(_accounts); }
        }

        private void EnsureLoaded()
        {
            if (_accounts == null)
            {
                _accounts = (Load() ?? Enumerable.Empty<MediaAccountData>()).ToList();
            }
        }

        private void Save()
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                CloseOutput = true
            };
            string finalFileName;
            using (XmlWriter writer = XmlWriter.Create(CreateTempFile(out finalFileName), settings))
            {
                new DataContractSerializer(typeof(MediaAccountStoreData)).WriteObject(writer, new MediaAccountStoreData { Accounts = _accounts.ToArray() });
            }
            if (File.Exists(DefaultPath))
            {
                File.Replace(finalFileName, DefaultPath, null);
            }
            else
            {
                File.Move(finalFileName, DefaultPath);
            }
        }

        private MediaAccountData[] Load()
        {
            if (!File.Exists(DefaultPath))
                return null;
            try
            {
                using (var fileStream = new FileStream(DefaultPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return ((MediaAccountStoreData)new DataContractSerializer(typeof(MediaAccountStoreData)).ReadObject(fileStream)).Accounts;
                }
            }
            catch (XmlException)
            {
                return null;
            }
        }

        private FileStream CreateTempFile(out string finalFileName)
        {
            while (true)
            {
                try
                {
                    string path = DefaultPath + "." + Guid.NewGuid();
                    var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                    finalFileName = path;
                    return fileStream;
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
