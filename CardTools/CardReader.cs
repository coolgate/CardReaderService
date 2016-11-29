using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CardTools
{
    public class CardReader
    {
        private Int32 _handle;
        private bool _shouldRead;

        public int ReadInterval = 1000;
        public int HoldTime = 2000;

        public delegate void ReadCardUidHandler(object sender, ReadCardUidEvent e);

        public event ReadCardUidHandler OnReadCardUid;

        public bool IsDeviceOpen()
        {
            return _handle > 0;
        }

        public bool OpenDevice()
        {
            var handle = CardDllImport.UsbHidOpen(0x6298, 0x3212);
            if (handle > 0)
            {
                _handle = handle;
                return true;
            }
            return false;
        }

        public void CloseDevice()
        {
            CardDllImport.UsbHidClose(ref _handle);
        }

        public CardDeviceInfo GetDeviceInfo()
        {
            var info = new StringBuilder();

            CardDllImport.GetDeviceInfo(_handle, info);

            return new CardDeviceInfo(info.ToString());
        }

        public void Beep(byte time)
        {
            CardDllImport.Beep(_handle, time);
        }

        public string ReadUid()
        {
            var uid = new StringBuilder();
            CardDllImport.IsoGetUid14443a(_handle, uid);
            if (uid.Length > 0)
                uid.Remove(8, uid.Length - 8);
            return uid.ToString();
        }

        public void ReadForever()
        {
            _shouldRead = true;
            Task readTask = new Task(() =>
            {
                while (_shouldRead)
                {
                    var uid = this.ReadUid();
                    if (uid != "")
                    {
                        OnReadCardUid(this, new ReadCardUidEvent(uid));                        
                        this.Beep(100);
                        Thread.Sleep(HoldTime);
                    }
                    Thread.Sleep(ReadInterval);
                }
            });
            readTask.Start();
        }

        public void StopReadForever()
        {
            _shouldRead = false;
        }

    }

    public class CardDeviceInfo
    {
        private readonly string _info;

        public CardDeviceInfo(string info)
        {
            _info = info;
        }

        public string Code => _info.Substring(0, 4);

        public string Name => _info.Substring(4, 4);

        public string Version => _info.Substring(10, 4);
    }

    public class ReadCardUidEvent : EventArgs
    {
        public string Uid { get; }

        public ReadCardUidEvent(string uid)
        {
            Uid = uid;
        }
    }
}