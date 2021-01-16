using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureStorageUploader {
    public class BlobManager {
        private const Task<Uri> Task = default;
        private CloudBlobClient blobClient;

        public BlobManager(string conectionString) {
            Init(conectionString);
        }

        public void Init(string conectionString) {
            try {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conectionString);
                blobClient = storageAccount.CreateCloudBlobClient();
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task<IEnumerable<Uri>> GetAllFilesUriAsync(string blobContainerName) {
            try {
                List<IListBlobItem> listBlob = await GetAllFilesAsync(blobContainerName);
                List<Uri> FilesUri = new List<Uri>();

                foreach (IListBlobItem blob in listBlob) {
                    FilesUri.Add(blob.Uri);
                }

                return FilesUri;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Uri> UploadFilesAsync(FileInfo file, string blobContainerName) {
            CloudBlobContainer blobContainer = await GetBlobContainer(blobContainerName);
            var fileName = GetRandomBlobName(file.Name);
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName);
            await blob.UploadFromFileAsync(file.FullName);
            return await GetFileUriAsync(fileName, blobContainerName);
        }

        public async Task<Uri> UploadFilesAsync(string file, string blobContainerName) =>
                                                             await UploadFilesAsync(new FileInfo(file), blobContainerName);

        public async Task<Uri> GetFileUriAsync(string filename, string blobContainerName) {
            try {
                var list = await GetAllFilesUriAsync(blobContainerName);
                Uri uri = null;
                foreach (var fileUri in list) {
                    string name = Path.GetFileName(fileUri.ToString());
                    if (name == filename) {
                        uri = fileUri;
                        break;
                    }
                }
                return uri;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteFileAsync(string fileUri, string blobContainerName) {
            try {
                CloudBlobContainer blobContainer = await GetBlobContainer(blobContainerName);
                Uri uri = new Uri(fileUri);
                string filename = Path.GetFileName(uri.LocalPath);
                var blob = blobContainer.GetBlockBlobReference(filename);
                await blob.DeleteIfExistsAsync();
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteAllAsync(String blobContainerName) {
            try {
                List<IListBlobItem> listBlob = await GetAllFilesAsync(blobContainerName);

                foreach (IListBlobItem blob in listBlob) {
                    if (blob.GetType() == typeof(CloudBlockBlob)) {
                        await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                    }
                }
                
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        private async Task<CloudBlobContainer> GetBlobContainer(String blobContainerName) {
            try {
                CloudBlobContainer blobContainer;
                blobContainer = blobClient.GetContainerReference($"{blobContainerName}");
                await blobContainer.CreateIfNotExistsAsync();
                await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

                return blobContainer;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        private async Task<List<IListBlobItem>> GetAllFilesAsync(string blobContainerName) {
            try {
                CloudBlobContainer blobContainer = await GetBlobContainer(blobContainerName);
                BlobContinuationToken blobContinuationToken = null;
                List<IListBlobItem> FilesUri = new List<IListBlobItem>();

                do {
                    var results = await blobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem blob in results.Results) {
                        FilesUri.Add(blob);
                    }
                } while (blobContinuationToken != null);
                return FilesUri;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        private string GetRandomBlobName(string filename) {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
    }
}
