using System;
using System.IO;
using System.Collections.Generic;

using Netease.Cloud.NOS.Model;
using Netease.Cloud.NOS.Model.XmlModel;

namespace Netease.Cloud.NOS.Transform
{
    internal class DeleteObjectsRequestSerializer : RequestSerializer<DeleteObjectsRequest, DeleteObjectsRequestModel>
    {
        public DeleteObjectsRequestSerializer(ISerializer<DeleteObjectsRequestModel, Stream> contentSerializer)
            : base(contentSerializer)
        { }

        public override Stream Serialize(DeleteObjectsRequest request)
        {
            var newKeys = new List<DeleteObjectsRequestModel.ObjectToDel>();
            foreach (var key in request.Keys)
            {
                newKeys.Add(new DeleteObjectsRequestModel.ObjectToDel { Key = key });
            }

            var model = new DeleteObjectsRequestModel
            {
                Quiet = request.Quiet,
                Keys = newKeys.ToArray()
            };
            return ContentSerializer.Serialize(model);
        }
    }
}
