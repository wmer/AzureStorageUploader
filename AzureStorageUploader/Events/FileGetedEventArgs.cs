using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageUploader.Events {
    public class FileGetedEventArgs : EventArgs {
        public IListBlobItem FileBlob { get; set; }
        public DateTime DateTime { get; set; }

        public FileGetedEventArgs(IListBlobItem fileBlob, DateTime dateTime) {
            FileBlob = fileBlob;
            DateTime = dateTime;
        }
    }

    public delegate void FileGetedEventHandler(object sender, FileGetedEventArgs ev);
}
