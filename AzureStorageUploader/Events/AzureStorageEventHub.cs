using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageUploader.Events {
    public class AzureStorageEventHub {
        public static event FilesGetedEventHandler FilesGeted;
        public static event FileGetedEventHandler FileGeted;
        public static event UploadedFileEventHandler UploadedFile;

        public static void OnFilesGeted(object sender, FilesGetedEventArgs e) {
            FilesGeted?.Invoke(sender, e);
        }

        public static void OnFileGeted(object sender, FileGetedEventArgs e) {
            FileGeted?.Invoke(sender, e);
        }

        public static void OnUploadedFile(object sender, UploadedFileEventArgs e) {
            UploadedFile?.Invoke(sender, e);
        }
    }
}
