using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Util
{
    public class RestUtils
    {
        private static IList<String> SIGNED_PARAMETERS = new List<string>
        {
            "acl", "location", "versioning", "versions", "versionId", "notification", "uploadId", "uploads", "partNumber", "delete", "deduplication", "crop", "resize"
        };
        /**
        * Calculate the canonical string for a REST/HTTP request to Nos.
        * 
        * When expires is non-null, it will be used instead of the Date header.
        */
        internal static string makeNosCanonicalString(string method, string resource, ServiceRequest request, string expires)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(method + "\n");

            // Add all interesting headers to a list, then sort them. "Interesting"
            // is defined as Content-MD5, Content-Type, Date, and x-nos-
            IDictionary<string, string> headers = request.Headers;
            IDictionary<string, string> interestingHeaders = new SortedDictionary<string, string>();
            if (headers != null && headers.Count > 0)
            {
                foreach (string name in headers.Keys)
                {
                    string value = headers[name];
                    string key = name.ToLower();

                    // Ignore any headers that are not particularly interesting.
                    if (key.Equals(Headers.CONTENT_TYPE.ToLower()) || key.Equals(Headers.CONTENT_MD5.ToLower()) || key.Equals(Headers.DATE.ToLower())
                        || key.StartsWith(Headers.NOS_PREFIX))
                    {
                        interestingHeaders.Add(key, value);
                    }
                }
            }

            // Remove default date timestamp if "x-nos-date" is set.
            if (interestingHeaders.ContainsKey(Headers.NOS_ALTERNATE_DATE.ToLower()))
            {
                interestingHeaders.Add(Headers.DATE.ToLower(), "");
            }

            // Use the expires value as the timestamp if it is available. This
            // trumps both the default
            // "date" timestamp, and the "x-nos-date" header.
            if (expires != null)
            {
                interestingHeaders.Add(Headers.DATE.ToLower(), expires);
            }

            // These headers require that we still put a new line in after them,
            // even if they don't exist.
            if (!interestingHeaders.ContainsKey(Headers.CONTENT_TYPE.ToLower()))
            {
                interestingHeaders.Add(Headers.CONTENT_TYPE.ToLower(), "");
            }
            if (!interestingHeaders.ContainsKey(Headers.CONTENT_MD5.ToLower()))
            {
                interestingHeaders.Add(Headers.CONTENT_MD5.ToLower(), "");
            }

            // Any parameters that are prefixed with "x-nos-" need to be included
            // in the headers section of the canonical string to sign
            foreach (string name in request.Parameters.Keys)
            {
                if (name.StartsWith(Headers.NOS_PREFIX))
                {
                    string value = request.Parameters[name];
                    interestingHeaders[name] = value;
                }
            }

            //// Add all the interesting headers (i.e.: all that startwith x-nos- ;-))
            foreach (string name in interestingHeaders.Keys)
            {
                if (name.StartsWith(Headers.NOS_PREFIX))
                    buf.Append(name + ":" + interestingHeaders[name]);
                else buf.Append(interestingHeaders[name]);
                buf.Append("\n");
            }

            //// Add all the interesting parameters  
            buf.Append(resource);
            string[] parameterNames = request.Parameters.Keys.ToArray();
            Array.Sort(parameterNames);
            char separator = '?';
            foreach (string parameterName in parameterNames)
            {
                // Skip any parameters that aren't part of the canonical signed string
                if (SIGNED_PARAMETERS.Contains(parameterName) == false) continue;

                buf.Append(separator);
                buf.Append(parameterName);
                string parameterValue = request.Parameters[parameterName];
                if (parameterValue != null)
                    buf.Append("=" + parameterValue);

                separator = '&';
            }

            return buf.ToString();
        }
    }
}
