using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NETPath.Helpers{

	// GAC Interfaces - IAssemblyCache. As a sample, non used vtable entries declared as dummy.
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
	internal interface IAssemblyCache
	{
		int Dummy1();
		[PreserveSig()]
		IntPtr QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] String assemblyName, ref ASSEMBLY_INFO assemblyInfo);
		int Dummy2();
		int Dummy3();
		int Dummy4();
	}
	[StructLayout(LayoutKind.Sequential)]
	internal struct ASSEMBLY_INFO
	{
		public int cbAssemblyInfo;
		public int assemblyFlags;
		public long assemblySizeInKB;
		[MarshalAs(UnmanagedType.LPWStr)]
		public String currentAssemblyPath;
		public int cchBuf;
	}

	internal class GACUtil
	{
		[DllImport("fusion.dll")] 
		private static extern IntPtr CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

		public static bool IsAssemblyInGAC(String assemblyName)
		{
			try
			{
				ASSEMBLY_INFO assembyInfo = new ASSEMBLY_INFO();
				assembyInfo.cchBuf = 512;
				assembyInfo.currentAssemblyPath = new String('\0', assembyInfo.cchBuf);
				IAssemblyCache assemblyCache = null;
				// Get IAssemblyCache pointer
				IntPtr hr = CreateAssemblyCache(out assemblyCache, 0);
				if (hr == IntPtr.Zero)
				{
					hr = assemblyCache.QueryAssemblyInfo(1, assemblyName, ref assembyInfo);
					if (hr != IntPtr.Zero)
						Marshal.ThrowExceptionForHR(hr.ToInt32());
				}
				else
					Marshal.ThrowExceptionForHR(hr.ToInt32());
			}
			catch
			{
				return false;
			}
			return true;
		}
	}
}