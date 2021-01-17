using AzureStorageUploader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageUploaderUnitTest {
    [TestClass]
    public class BlobManagerUnitTest {
        [TestMethod]
        public void GetAllContainers() {
            BlobManager blobManager = GetBlobManager();
            var containers = blobManager.GetAllBlobContainesr();
            Assert.IsTrue(containers.Count() > 0);
        }


        [TestMethod]
        public async Task GetContainerAsync() {
            BlobManager blobManager = GetBlobManager();
            var containers = blobManager.GetAllBlobContainesr();
            var container = await blobManager.GetBlobContainerAsync(containers.FirstOrDefault());
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public async Task GetAllBlobLinksAsync() {
            BlobManager blobManager = GetBlobManager();
            var containers = blobManager.GetAllBlobContainesr();
            var container = await blobManager.GetAllFilesUriAsync(containers[1]);
            Assert.IsTrue(container.Count() > 0);
        }


        private static BlobManager GetBlobManager() {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            var blobManager = new BlobManager(connectionString);
            return blobManager;
        }
    }
}
