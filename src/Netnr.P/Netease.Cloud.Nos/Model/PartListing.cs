using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 获取列出已上传的分块的结果
    /// </summary>
    public class PartListing
    {
        private readonly IList<Part> _parts = new List<Part>();

        /// <summary>
        /// 获取对象所在的桶名
        /// </summary>
        public string Bucket { get; internal set; }

        /// <summary>
        /// 获取对象名称
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// 获取分块上传操作的ID 类型
        /// </summary>
        public string UploadId { get; internal set; }

        /// <summary>
        /// 获取桶拥有者的信息 子节点
        /// </summary>
        public Owner Owner { get; internal set; }

        /// <summary>
        /// 获取存储级别
        /// </summary>
        public string StorageClass { get; internal set; }

        /// <summary>
        /// 获取上次List操作后的Part number 类型
        /// </summary>
        public int PartNumberMarker { get; internal set; }

        /// <summary>
        /// 作为后续List操作的part-number-marker 类型
        /// </summary>
        public int NextPartNumberMarker { get; internal set; }

        /// <summary>
        /// 响应允许返回的的最大part数目 类型
        /// </summary>
        public int MaxParts { get; internal set; }

        /// <summary>
        /// 获取是否截断，如果因为设置了limit导致不是所有的数据集都返回了，则该值设置为true 类型
        /// </summary>
        public bool IsTruncated { get; internal set; }

        /// <summary>
        /// 获取所有Part
        /// </summary>
        public IEnumerable<Part> Parts
        {
            get { return _parts; }
        }

        /// <summary>
        /// 增加<see cref="Part"/>分片信息
        /// </summary>
        /// <param name="part">分片信息</param>
        internal void AddPart(Part part)
        {
            _parts.Add(part);
        }

        internal PartListing()
        { }
    }
}
