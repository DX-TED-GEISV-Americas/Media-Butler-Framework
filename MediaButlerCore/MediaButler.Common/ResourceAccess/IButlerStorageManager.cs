using MediaButler.Common.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.ResourceAccess
{
    /// <summary>
    /// MBF storage manager
    /// </summary>
    public interface IButlerStorageManager
    {
        /// <summary>
        /// Delete Blob
        /// </summary>
        /// <param name="blobUrl">Blob URL</param>
        void DeleteBlobFile(string blobUrl);
        /// <summary>
        /// Read text from  blob 
        /// </summary>
        /// <param name="blobUrl">blob URL access</param>
        /// <returns></returns>
        string ReadTextBlob(Uri blobUrl);
        /// <summary>
        /// Read text from MBF storage blob 
        /// </summary>
        /// <param name="containerName">blob container</param>
        /// <param name="blobName">blob name</param>
        /// <returns></returns>
        string ReadTextBlob(string containerName, string blobName);
        /// <summary>
        /// Persis instance process status on Table
        /// </summary>
        /// <param name="request">current instance process request</param>
        void PersistProcessStatus(ChainRequest request);
        /// <summary>
        /// Persis instance process status on Table
        /// </summary>
        /// <param name="processSnapshot">Process status snapshot</param>
        void PersistProcessStatus(ProcessSnapShot processSnapshot);
        /// <summary>
        /// Read process instance Snapshot
        /// </summary>
        /// <param name="processName">process type ID</param>
        /// <param name="processId">Process instance ID</param>
        /// <returns></returns>
        ProcessSnapShot readProcessSanpShot(string processName, string processId);
        /// <summary>
        /// Generate blob URL SAS
        /// </summary>
        /// <param name="blobUri">Blob URI</param>
        /// <param name="hours">ttl hours</param>
        /// <returns></returns>
        string GetBlobSasUri(string blobUri, int hours);
        /// <summary>
        /// Download binary files for storage BIN container
        /// </summary>
        void parkingNewBinaries();
        /// <summary>
        /// Key value data from blob 
        /// </summary>
        /// <param name="URL">Blob URL</param>
        /// <returns>key value data</returns>
        IjsonKeyValue GetDotControlData(string URL);
        /// <summary>
        /// Read configuration from MBF configuration table
        /// </summary>
        /// <param name="partition">partition key</param>
        /// <param name="row">row Key</param>
        /// <returns>string value</returns>
        string GetButlerConfigurationValue(string partition, string row);
    }
}
