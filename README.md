MediaServicesCommandLineTools
=============================

Tools and sample code for Windows Azure Media Services.

This is experimental code that exercises the features of the Windows
Azure Media Services Preview release.

Not for production use. Some bits still problematic. Use with care!

Comments, feedback and patches welcome!

Getting Started
---------------

Build the solutions (currently V2012 but should be easily converted
back to to VS2010).

edit the ..\etc\setup.bat to include you account details.

Bring up a cmd.exe prompt and change into the ..\bin directory.

Run a command

Jobs - Lists jobs
Tasks <jobId> - list all the tasks taht are part of the specified job
Assets - Lists assets
AssetFiles <assetId> - Lists the files for an Asset
Download <fileId> - downloads a file
DownloadAsset <assetId> - downloads all the files for an asset
ToMP4 <assetId> - creates a job to convert the given asset to MP4
....



Rob Blackwell

October 2012