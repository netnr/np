using System;

namespace Netease.Cloud.NOS.Authentication
{
    /// <summary>
    /// 鉴权工厂接口
    /// </summary>
    public interface ICredentialsFactory
    {
        /// <summary>
        /// 设置一个新的<see cref="ICredentials"/>
        /// </summary>
        /// <param name="creds"><see cref="ICredentials">实例</param>
        void SetCredentials(ICredentials credentials);

        /// <summary>
        /// 获取一个<see cref="ICredentials"/>
        /// </summary>
        /// <returns><see cref="ICredentials"/>实例</returns>
        ICredentials GetCredentials();
    }
}
