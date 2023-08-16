namespace ScanUtilityLibrary.Core.TripleIN
{
    /*
     * Enumeration of errors for input/output devices and for internal errors.
     * 0 and positive numbers means always "no error", negative indicates problems.
     * Supported ARM and GCC compilers defines different numbers in <errno.h>, so we decided to define some own, project depending definitions.
     * Note:
     * errno.h defines __ELASTERROR, which is used as a base for KEM-defined error values.
     * ANSI C requires a function strerror, but does not specify the strings used for each error number. The KEM application supplies a routine named _user_strerror. _user_strerror takes one argument of type int, and returns a character pointer with a human readable error message.
     */

    /// <summary>
    /// 错误识别码
    /// Error identification numbers.
    /// </summary>
    public enum ErrorID
    {
        /// <summary>
        /// 错误码的基础值
        /// </summary>
        __ELASTERROR = 2000,

        /// <summary>
        /// Serial version ID of this table
        /// </summary>
        ERRORID_SERIAL_VERSION = 20120601,

        /// <summary>
        /// Function was successful.
        /// </summary>
        ERR_SUCCESS = 0,

        /// <summary>
        /// no error
        /// </summary>
        ERR_NONE = 0,

        /// <summary>
        /// Number negative error codes from tail to top.
        /// Last error code is defined in stdlib's errno.h.
        /// 22 is the number of codes defined here.
        /// </summary>
        __ERR_LASTERROR = -__ELASTERROR - 22,

        /// <summary>
        /// system not ready
        /// </summary>
        ERR_SYSTEM_NOT_READY,

        /// <summary>
        /// front screen dirty or wet
        /// </summary>
        ERR_FRONT_SCREEN_NOT_CLEAR,

        /// <summary>
        /// temperature out of operating range
        /// </summary>
        ERR_TEMPERATURE_OUT_OF_RANGE,

        /// <summary>
        /// Angle encoder or motor unit failure.
        /// </summary>
        ERR_KEM_SCAN_UNIT_FAILURE,

        /// <summary>
        /// KEM unit / KEM measurement ufailure
        /// </summary>
        ERR_KEM_UNIT,

        /// <summary>
        /// Error in serialization: version does not match.
        /// </summary>
        ERR_SERIAL_VERSION,

        /// <summary>
        /// invalid or defect system configuration
        /// </summary>
        ERR_CONFIGURATION_ERROR,

        /// <summary>
        /// Fatal system error
        /// </summary>
        ERR_FATAL_SYSTEM_ERROR,

        /// <summary>
        /// internal Buffer overflow
        /// </summary>
        ERR_BUFFER_OVERFLOW,

        /// <summary>
        /// array index is out of range
        /// </summary>
        ERR_INDEX_OUT_OF_RANGE,

        /// <summary>
        /// Division by zero
        /// </summary>
        ERR_DIV_BY_ZERO,

        /// <summary>
        /// Invalid handle / Bad address
        /// </summary>
        ERR_INVALID_HANDLE,

        /// <summary>
        /// Function not supported
        /// </summary>
        ERR_UNSUPPORTED_FUNCTION,

        /// <summary>
        /// Access denied / Permission denied
        /// </summary>
        ERR_ACCESS_DENIED,

        /// <summary>
        /// Parameter is out of range
        /// </summary>
        ERR_INVALID_PARAMETER,

        /// <summary>
        /// Unknown command
        /// </summary>
        ERR_UNKNOWN_COMMAND,

        /// <summary>
        /// CRC checksum error
        /// </summary>
        ERR_CRC,

        /// <summary>
        /// user break
        /// </summary>
        ERR_USER_BREAK,

        /// <summary>
        /// Timeout
        /// </summary>
        ERR_TIMEOUT,

        /// <summary>
        /// cannot write
        /// </summary>
        ERR_WRITE,

        /// <summary>
        /// cannot read
        /// </summary>
        ERR_READ,

        /// <summary>
        /// physical I/O error
        /// </summary>
        ERR_IO
    }

    /// <summary>
    /// ErrorID转换器
    /// </summary>
    public static class ErrorIDConverter
    {
        /// <summary>
        /// Convert an integer into ErrorID
        /// </summary>
        /// <param name="theErrorID">The corresponding ErrorID_t value, or ERR_SUCCESS if (theErrorID >= 0) or ERR_INVALID_HANDLE if the negative code is not in range of ErrorID</param>
        public static ErrorID IntToError(long theErrorID)
        {
            if (theErrorID >= 0)
            {
                return ErrorID.ERR_SUCCESS;
            }
            else if (theErrorID >= (int)ErrorID.__ERR_LASTERROR && theErrorID < 0)
            {
                return (ErrorID)theErrorID;
            }
            else
            {
                return ErrorID.ERR_INVALID_HANDLE;
            }
        }
    }

}