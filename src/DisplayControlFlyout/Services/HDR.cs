using System;
using System.Runtime.InteropServices;

namespace DisplayControlFlyout.Services
{
    public static class HDR
    {

        readonly static object _dllLock = new object();

        [DllImport("HDRController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SetGlobalHDRState(bool enabled);

        [DllImport("HDRController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetGlobalHDRState();

        [DllImport("HDRController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr SetHDRState(UInt32 uid, bool enabled);

        [DllImport("HDRController.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetHDRState(UInt32 uid);
    }
}