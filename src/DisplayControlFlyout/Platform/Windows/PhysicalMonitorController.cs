using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// ReSharper disable UnusedMember.Local

namespace DisplayControlFlyout.Platform.Windows
{
    public class PhysicalMonitorController : IDisposable
    {
        #region DllImport
        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PhysicalMonitor[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitor")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PhysicalMonitor[] pPhysicalMonitorArray);

        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
        delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);
        private static int SC_MONITORPOWER = 0xF170;
        private static uint WM_SYSCOMMAND = 0x0112;


        enum MonitorState
        {
            On = -1,
            Off = 2,
            Standby = 1
        }

        public static void TurnDisplayOff()
        {
            Form frm = new Form();
            SendMessage(frm.Handle, WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MonitorState.Off);
        }

        public IReadOnlyCollection<MonitorInfo>? Monitors { get; set; }

        public PhysicalMonitorController()
        {
            UpdateMonitors();
        }

        #region Get & Set
        public void Set(uint brightness, MonitorInfo monitor)
        {
            Set(brightness, monitor, true);
        }

        public void SetAll(uint brightness)
        {
            if (Monitors == null) return;
            foreach (var m in Monitors)
            {
                Set(brightness, m, true);
            }
        }

        private void Set(uint brightness, MonitorInfo monitor, bool refreshMonitorsIfNeeded)
        {
            bool isSomeFail = false;

            uint realNewValue = (monitor.MaxValue - monitor.MinValue) * brightness / 100 + monitor.MinValue;
            if (SetMonitorBrightness(monitor.Handle, realNewValue))
            {
                monitor.CurrentValue = realNewValue;
            }
            else if (refreshMonitorsIfNeeded)
            {
                isSomeFail = true;
            }

            if (Monitors == null) return;
            if (!refreshMonitorsIfNeeded || (!isSomeFail && Monitors.Any())) return;
            UpdateMonitors();
            Set(brightness, monitor, false);
        }

        public uint GetAverage()
        {
            if (Monitors == null) return 0;
            if (!Monitors.Any())
            {
                return 0;
            }
            return (uint)Monitors.Average(d => d.CurrentValue);
        }

        public uint Get(MonitorInfo monitor)
        {
            if (Monitors == null) return 0;
            return !Monitors.Any() ? 0 : Monitors.FirstOrDefault(m => m.Handle == monitor.Handle)!.CurrentValue;
        }
        #endregion

        public void UpdateMonitors()
        {
            DisposeMonitors(this.Monitors);

            var monitors = new List<MonitorInfo>();
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr _, ref Rect _, IntPtr _) =>
            {
                uint physicalMonitorsCount = 0;
                if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref physicalMonitorsCount))
                {
                    // Cannot get monitor count
                    return true;
                }

                var physicalMonitors = new PhysicalMonitor[physicalMonitorsCount];
                if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, physicalMonitorsCount, physicalMonitors))
                {
                    // Cannot get physical monitor handle
                    return true;
                }

                foreach (PhysicalMonitor physicalMonitor in physicalMonitors)
                {
                    uint minValue = 0, currentValue = 0, maxValue = 0;
                    if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, ref minValue, ref currentValue, ref maxValue))
                    {
                        DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
                        continue;
                    }

                    var info = new MonitorInfo
                    {
                        Handle = physicalMonitor.hPhysicalMonitor,
                        MinValue = minValue,
                        CurrentValue = currentValue,
                        MaxValue = maxValue,
                    };
                    monitors.Add(info);
                }

                return true;
            }, IntPtr.Zero);

            this.Monitors = monitors;
        }

        public void Dispose()
        {
            DisposeMonitors(Monitors);
            GC.SuppressFinalize(this);
        }

        private static void DisposeMonitors(IEnumerable<MonitorInfo>? monitors)
        {
            if (monitors == null) return;

            var monitorInfos = monitors as MonitorInfo[] ?? monitors.ToArray();
            if (monitorInfos.Any() != true) return;
            PhysicalMonitor[] monitorArray = monitorInfos.Select(m => new PhysicalMonitor { hPhysicalMonitor = m.Handle }).ToArray();
            DestroyPhysicalMonitors((uint)monitorArray.Length, monitorArray);
        }

        #region Classes
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PhysicalMonitor
        {
            public IntPtr hPhysicalMonitor;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public class MonitorInfo
        {
            public uint MinValue { get; set; }
            public uint MaxValue { get; set; }
            public IntPtr Handle { get; set; }
            public uint CurrentValue { get; set; }
        }
        #endregion
    }
}