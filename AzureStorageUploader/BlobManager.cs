using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzureStorageUploader {
    public class BlobManager {
        private const Task<Uri> Task = default;
        private string _conectionString;

        public BlobManager(string conectionString) {
            _conectionString = conectionString;
        }


        public List<string> GetAllBlobContainesr() {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_conectionString);
            var containers = blobServiceClient.GetBlobContainers();
            var names = new List<string>();
            foreach (var c in containers) {
                names.Add(c.Name);
            }
            return names;
        }

        public async Task<BlobContainerClient> GetBlobContainerAsync(string containerName) {
            BlobContainerClient containerCliente = new BlobContainerClient(_conectionString, containerName);
            await containerCliente.CreateIfNotExistsAsync();
            return containerCliente;
        }

        public async Task<Uri> UploadFileAsync(FileInfo file, string blobContainerName, bool randomizeBlobName = false) {
            BlobContainerClient containerCliente = await GetBlobContainerAsync(blobContainerName);
            var fileName = file.Name;

            if (randomizeBlobName) {
                fileName = GetRandomBlobName(file.Name);
            }

            BlobClient blobClient = containerCliente.GetBlobClient(fileName);

            using FileStream uploadFileStream = File.OpenRead(file.FullName);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            return blobClient.Uri;
        }

        public async Task<Uri> UploadFileAsync(string file, string blobContainerName, bool randomizeBlobName = false) =>
                                                             await UploadFileAsync(new FileInfo(file), blobContainerName, randomizeBlobName);

        public async Task<IEnumerable<Uri>> GetAllFilesUriAsync(string blobContainerName) {
            List<Uri> filesURI = new List<Uri>();

            BlobContainerClient containerCliente = await GetBlobContainerAsync(blobContainerName);
            await foreach (BlobItem blobItem in containerCliente.GetBlobsAsync()) {
                BlobClient blobClient = containerCliente.GetBlobClient(blobItem.Name);
                filesURI.Add(blobClient.Uri);
            }

            return filesURI;
        }

        public async Task<Uri> GetFileUriAsync(string filename, string blobContainerName) {
            BlobContainerClient containerCliente = await GetBlobContainerAsync(blobContainerName);
            BlobClient blobClient = containerCliente.GetBlobClient(filename);
            return blobClient.Uri;
        }

        public async Task<bool> DeleteFileAsync(Uri uri, string blobContainerName) {
            BlobContainerClient containerCliente = await GetBlobContainerAsync(blobContainerName);
            string filename = Path.GetFileName(uri.LocalPath);
            BlobClient blobClient = containerCliente.GetBlobClient(filename);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<bool> DeleteFileAsync(string fileUri, string blobContainerName) {
            Uri uri = new Uri(fileUri);
            return await DeleteFileAsync(uri, blobContainerName);
        }

        public async Task<bool> DeleteAllAsync(String blobContainerName) {
            BlobContainerClient containerCliente = await GetBlobContainerAsync(blobContainerName);
            return await containerCliente.DeleteIfExistsAsync();
        }

        private string GetRandomBlobName(string filename) {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
    }
}
