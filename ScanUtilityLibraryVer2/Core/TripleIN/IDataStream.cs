using System;

namespace ScanUtilityLibrary.Core.TripleIN
{
    /// <summary>
    /// Interface for data stream classes.
    /// </summary>
    public interface IDataStream
    {
        /// <summary>
        /// Shall close the Device and releas any system resources associated.
        /// Returns ERR_SUCCESS if the data stream was successfully closed,
        /// negative error code if failed.
        /// </summary>
        /// <returns></returns>
        ErrorID Close();

        /// <summary>
        /// Returns true if the socket is ready to be used.
        /// </summary>
        /// <returns></returns>
        bool IsOpen();

        /// <summary>
        /// Opens and resets a data stream.
        /// Returns ERR_SUCCESS if the data stream could be opened successfully,
        /// negative error code if open() failed.
        /// </summary>
        /// <returns></returns>
        ErrorID Open();

        /// <summary>
        /// POSIX similar read() method.
        /// Reads up to len bytes of data from the input buffer device
        /// into an array of bytes.
        /// If no data are available because the end of the data stream has been
        /// reached, the value -1 is returned.
        /// Returns the number bytes received or a negative error code if failed.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        int Read(IntPtr buffer, int size);

        /// <summary>
        /// POSIX similar write() method.
        /// Writes len bytes from the specified byte array to the outgoing
        /// data stream.
        /// Returns the number bytes written or a negative error code if failed.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        int Write(IntPtr buffer, int size);
    }

}