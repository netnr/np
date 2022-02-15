using System;

using Netease.Cloud.NOS.Util;

namespace Netease.Cloud.NOS.Authentication
{
    public class DefaultCredentialsFactory : ICredentialsFactory
    {
        private volatile ICredentials _credentials;

        /// <summary>
        /// 构造一个<see cref="DefaultCredentialsFactory"/>的实例
        /// </summary>
        /// <param name="credentials"><see cref="ICredentials"/>实例</param>
        public DefaultCredentialsFactory(ICredentials credentials)
        {
            SetCredentials(credentials);
        }

        //设置一个Credentials
        public void SetCredentials(ICredentials credentials)
        {
            NosUtils.CheckCredentials(credentials.AccessKeyId, credentials.AccessKeySecret);
            this._credentials = credentials;
        }

        //获取一个Credentials
        public ICredentials GetCredentials()
        {
            return _credentials;
        }
    }
}
