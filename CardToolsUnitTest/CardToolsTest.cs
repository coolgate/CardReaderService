using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using CardTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardToolsUnitTest
{
    [TestClass]
    public class CardToolsTest
    {
        [TestMethod]
        public void TestClassCardDllImport()
        {
            var handle = CardDllImport.UsbHidOpen(0x6298, 0x3212);
            Assert.IsTrue(handle > 0);

            CardDllImport.Beep(handle, 200);

//            CardDllImport.AutoBeep(handle, 1);

            int ret;
//            ret = CardDllImport.VerifyPassword(handle, new IntPtr(0x62983212));

            var info = new StringBuilder();
            ret = CardDllImport.GetDeviceInfo(handle, info);

            var uid = new StringBuilder();
            ret = CardDllImport.IsoGetUid14443a(handle, uid);

            ret = CardDllImport.UsbHidClose(ref handle);
            Assert.IsTrue(ret == 0);
        }

        [TestMethod]
        public void TestCardReader()
        {
            var reader = new CardReader();
            reader.OpenDevice();
            var count = 10;
            var sleepTime = 1000;
            while (count > 0)
            {
                var uid = reader.ReadUid();
                if (uid != "")
                {
                    Console.WriteLine(uid);
                    reader.Beep(100);
                    count--;
                    sleepTime = 3000;
                }
                else
                {
                    sleepTime = 1000;
                }
                Thread.Sleep(sleepTime);
            }
            reader.CloseDevice();
        }

        [TestMethod]
        public void TestReadForever()
        {
            var reader = new CardReader();

            reader.OnReadCardUid += new CardReader.ReadCardUidHandler((sender, e) =>
            {
                Console.WriteLine("Card ID: " + e.Uid);
            });

            reader.OpenDevice();
            reader.ReadForever();
            Thread.Sleep(10000);
            reader.StopReadForever();
            reader.CloseDevice();
        }
    }
}
