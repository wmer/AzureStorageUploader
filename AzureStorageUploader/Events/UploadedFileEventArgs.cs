using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageUploader.Events {
    public class UploadedFileEventArgs : EventArgs {
        public Uri UriFile { get; set; }
        public DateTime DateTime { get; set; }

        public UploadedFileEventArgs(Uri uri, DateTime dateTime) {
            UriFile = uri;
            DateTime = dateTime;
        }
    }

    public delegate void UploadedFileEventHandler(object sender, UploadedFileEventArgs ev);
}
