using Xunit;

namespace Netnr.Test
{
    public class TestDriveInfo
    {
        [Fact]
        public void GetDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo di in allDrives)
            {
                var dic = new Dictionary<string, object>
                {
                    { "Name", di.Name },
                    { "DriveType", di.DriveType },
                    { "DriveFormat", di.DriveFormat },
                    { "IsReady", di.IsReady },
                    { "RootDirectory", di.RootDirectory.FullName },
                    { "TotalSize", di.TotalSize },
                    { "TotalFreeSpace", di.TotalFreeSpace },
                    { "AvailableFreeSpace", di.AvailableFreeSpace },
                    { "VolumeLabel", di.VolumeLabel }
                };

                Debug.WriteLine(dic.ToJson(true));
            }
        }
    }
}
