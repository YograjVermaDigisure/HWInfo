namespace HwdInfo
{
    using System;
    using System.Runtime.InteropServices;
    internal class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetVolumeInformation(
        string lpRootPathName,
        System.Text.StringBuilder lpVolumeNameBuffer,
        uint nVolumeNameSize,
        out uint lpVolumeSerialNumber,
        out uint lpMaximumComponentLength,
        out uint lpFileSystemFlags,
        System.Text.StringBuilder lpFileSystemNameBuffer,
        uint nFileSystemNameSize);
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int GetAdaptersInfo(IntPtr pAdapterInfo, ref uint pOutBufferLength);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct IP_ADAPTER_INFO
        {
            public IntPtr Next;
            public int ComboIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string AdapterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 132)]
            public string Description;
            public uint AddressLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Address;
            public int Index;
            public uint Type;
            public uint DhcpEnabled;
            public IntPtr CurrentIpAddress;
            public IP_ADDR_STRING IpAddressList;
            public IP_ADDR_STRING GatewayList;
            public IP_ADDR_STRING DhcpServer;
            [MarshalAs(UnmanagedType.Bool)]
            public bool HaveWins;
            public IP_ADDR_STRING PrimaryWinsServer;
            public IP_ADDR_STRING SecondaryWinsServer;
            public uint LeaseObtained;
            public uint LeaseExpires;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct IP_ADDR_STRING
        {
            public IntPtr Next;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string IpAddress;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string IpMask;
            public uint Context;
        }
        static void Main(string[] args)
        {
            uint serialNumber;
            Console.WriteLine("Release Date : 21/Feb/2026");
            GetVolumeSerial("C:\\", out serialNumber);
            Console.WriteLine($"Volume Serial Number: {serialNumber:X}");
            string macAddress = GetMacAddress();
            Console.WriteLine("MAC Address: " + macAddress);
            Console.ReadLine();
        }

        static void GetVolumeSerial(string driveLetter, out uint serialNumber)
        {
            serialNumber = 0;
            System.Text.StringBuilder volumeName = new System.Text.StringBuilder(261);
            System.Text.StringBuilder fileSystemName = new System.Text.StringBuilder(261);
            uint maxCompLength, fileSystemFlags;

            if (GetVolumeInformation(driveLetter, volumeName, (uint)volumeName.Capacity, out serialNumber,
                out maxCompLength, out fileSystemFlags, fileSystemName, (uint)fileSystemName.Capacity))
            {
                // Successfully retrieved volume serial number
            }
        }

        public static string GetMacAddress()
        {
            uint bufferSize = 0;
            GetAdaptersInfo(IntPtr.Zero, ref bufferSize); // Get buffer size
            IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);

            try
            {
                if (GetAdaptersInfo(buffer, ref bufferSize) == 0)
                {
                    var adapterInfo = Marshal.PtrToStructure<IP_ADAPTER_INFO>(buffer);
                    return BitConverter.ToString(adapterInfo.Address, 0, (int)adapterInfo.AddressLength);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

            return "Not Available";
        }

    }
}
