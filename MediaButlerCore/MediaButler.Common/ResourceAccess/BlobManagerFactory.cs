namespace MediaButler.Common.ResourceAccess
{
    /// <summary>
    /// Blob manager factory
    /// </summary>
    public class BlobManagerFactory
    {
        /// <summary>
        /// Instance new blob manager
        /// </summary>
        /// <param name="strConn"></param>
        /// <returns></returns>
        public static IButlerStorageManager CreateBlobManager(string strConn)
        {
            return new BlobStorageManager(strConn);
        }
    }
}
