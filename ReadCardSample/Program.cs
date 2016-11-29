using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CardTools;

namespace ReadCardSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new CardReader();
            reader.HoldTime = 2000;
            reader.ReadInterval = 1000;

            reader.OnReadCardUid += (sender, e) =>
            {
                Console.WriteLine("Card UID: " + e.Uid);
            };

            Console.WriteLine("Press ENTER to stop ...");

            reader.OpenDevice();
            reader.ReadForever();

            Console.Read();

            reader.StopReadForever();
            reader.CloseDevice();
        }
    }
}
