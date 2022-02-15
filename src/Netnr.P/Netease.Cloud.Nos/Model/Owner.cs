using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;

namespace Netease.Cloud.NOS
{
    /// <summary>
    /// 表示对象的所有者
    /// </summary>
    [XmlRoot("Owner")]
    public class Owner : ICloneable
    {
        /// <summary>
        /// 获取或设置所有者的Id
        /// </summary>
        [XmlElement("ID")]
        public string Id { get; set; }

        /// <summary>
        /// 获取或设置所有者的显示名称
        /// </summary>
        [XmlElement("DispalyName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 构造一个新的<see cref="Owner"/>实例
        /// </summary>
        internal Owner()
        { }

        /// <summary>
        /// 构造一个新的给定所有者ID和名称的<see cref="Owner"/>实例
        /// </summary>
        /// <param name="id">所有者ID</param>
        /// <param name="displayName">所有者显示的名称</param>
        internal Owner(string id, string displayName)
        {
            this.Id = id;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// 克隆一个<see cref="Owner"/>
        /// </summary>
        /// <returns><see cref="Owner"/>对象</returns>
        public object Clone()
        {
            return new Owner(Id, DisplayName);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InstalledUICulture, "Owner Id={0}, DisplayName={1}", Id ?? string.Empty, DisplayName ?? string.Empty);
        }
    }
}
