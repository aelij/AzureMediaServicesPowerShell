MediaServicesCommandLineTools
=============================

Tools and sample code for Windows Azure Media Services.

This is experimental code that exercises the features of Windows
Azure Media Services.

Not for production use. Some bits still problematic. Use with care!

Comments, feedback and patches welcome!

Getting Started
---------------

Build the solutions.

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
    ContentKeys - list content keys
    CreateAsset <filename> - upload and create a new asset from local file
    TaskPresets - list the task presets
    ErrorDetails <taskId> - Show error details of a failed task
    StreamingUrl <assetId> - Create an Origin Locator for a SmoothStream asset



TODO
----

Test PlayReady

HLS Streaming

More example format conversions


Rob Blackwell

October 2012

Update
---------------

Changes made to reflect the new SDK
CreateAsset is now Upload Asset. Some formats are now more tightly constrained with Azure Media Services, so task preset strings have been utilised.