using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml;

using Netease.Cloud.NOS.Service;
using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Transform;
using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Handlers
{
    internal class ErrorResponseHandler : ResponseHander
    {
        public override void Handle(ServiceReponse response)
        {
            base.Handle(response);

            if (response.IsSuccessful())
                return;

            if (response.HttpStatusCode == HttpStatusCode.NotModified)
                throw ExceptionFactory.CreateException((int)response.HttpStatusCode, HttpStatusCode.NotModified.ToString(), response.Failure.Message, null, null);

            ErrorResult errorResult = null;
            try
            {
                var deserializer = DeserializerFactory.GetFactory().CreateErrorResultDeserializer();
                if (deserializer == null)
                    response.EnsureSuccessful();
                else
                {
                    errorResult = deserializer.Deserialize(response);
                }

            }
            catch (XmlException)
            {
                response.EnsureSuccessful();
            }
            catch (InvalidOperationException)
            {
                response.EnsureSuccessful();
            }

            Debug.Assert(errorResult != null);
            throw ExceptionFactory.CreateException((int)response.HttpStatusCode,errorResult.Code, errorResult.Message, errorResult.Resource, errorResult.RequestId);
        }
    }
}
