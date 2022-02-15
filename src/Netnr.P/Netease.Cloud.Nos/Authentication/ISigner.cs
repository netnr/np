using Netease.Cloud.NOS.Service;

namespace Netease.Cloud.NOS.Authentication
{
    internal interface ISigner
    {
        void Sign(ServiceRequest request, ICredentials credentials);
    }
}
