using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageUploader.Events {
    public class FilesGetedEventArgs : EventArgs {
        public List<IListBlobItem> FilesBlob { get; set; }
        public DateTime DateTime { get; set; }

        public FilesGetedEventArgs(List<IListBlobItem> filesBlob, DateTime dateTime) {
            FilesBlob = filesBlob;
            DateTime = dateTime;
        }
    }

    public delegate void FilesGetedEventHandler(object sender, FilesGetedEventArgs ev);
}
