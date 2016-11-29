using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CardTools
{
    public class CardDllImport
    {
        [DllImport("fb50.dll", EntryPoint = "usbhidOpen_CPP")]
        public static extern Int32 UsbHidOpen(UInt16 vender, UInt16 productId);

        [DllImport("fb50.dll", EntryPoint = "usbhidClose_CPP")]
        public static extern Int32 UsbHidClose(ref Int32 handle);

        [DllImport("fb50.dll", EntryPoint = "VerifyPSW_CPP")]
        public static extern int VerifyPassword(Int32 handle, IntPtr password);

        [DllImport("fb50.dll", EntryPoint = "GetDevInfo_CPP")]
        public static extern int GetDeviceInfo(Int32 handle, StringBuilder info);

        [DllImport("fb50.dll", EntryPoint = "Beep_CPP")]
        public static extern Int32 Beep(Int32 handle, Byte time);

        [DllImport("fb50.dll", EntryPoint = "AutoBeep_CPP")]
        public static extern int AutoBeep(Int32 handle, byte enable);

        [DllImport("fb50.dll", EntryPoint = "isoGetUID_14443a_CPP")]
        public static extern int IsoGetUid14443a(Int32 handle, StringBuilder uid);

    }
}
