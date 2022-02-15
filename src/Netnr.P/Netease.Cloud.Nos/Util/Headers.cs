using System;

namespace Netease.Cloud.NOS.Util
{
    internal static class Headers
    {
        /*
         * Standard HTTP Headers
         */
        public const string CONTENT_LENGTH = "Content-Length";
        public const string CONTENT_MD5 = "Content-MD5";
        public const string CONTENT_TYPE = "Content-Type";
        public const string CONTENT_ENCODING = "Content-Encoding";
        public const string CONTENT_DISPOSITION = "Content-Disposition";
        public const string CONTENT_LANGUAGE = "Content-Language";
        public const string CACHE_CONTROL = "Cache-Control";
        public const string DATE = "Date";
        public const string ETAG = "ETag";
        public const string LAST_MODIFIED = "Last-Modified";

        /*
         * HTTP Headers
         */

        /** Prefix for general headers: x-nos- */
        public const string NOS_PREFIX = "x-nos-";

        /** NOS's canned ACL header: x-nos-acl */
        public const string NOS_CANNED_ACL = "x-nos-acl";

        /** NOS's alternative date header: x-nos-date */
        public const string NOS_ALTERNATE_DATE = "x-nos-date";

        /** Prefix for NOS user metadata: x-nos-meta- */
        public const string NOS_USER_METADATA_PREFIX = "x-nos-meta-";

        /** NOS's version ID header */
        public const string NOS_VERSION_ID = "x-nos-version-id";

        /** NOS response header for a request's request ID */
        public const string REQUEST_ID = "x-nos-request-id";

        /** DevPay token header */
        public const string SECURITY_TOKEN = "x-nos-security-token";

        /** Header describing what class of storage a user wants */
        public const string STORAGE_CLASS = "x-nos-storage-class";

        /** Header for optional object expiration */
        public const string EXPIRATION = "x-nos-expiration";

        /** Range header for the get object request */
        public const string RANGE = "Range";

        /** Modified since constraint header for the get object request */
        public const string GET_OBJECT_IF_MODIFIED_SINCE = "If-Modified-Since";

        public const string X_NOS_OBJECT_MD5 = "x-nos-Object-md5";

        public const string X_NOS_COPY_SOURCE = "x-nos-copy-source";

        public const string X_NOS_MOVE_SOURCE = "x-nos-move-source";
    }
}
