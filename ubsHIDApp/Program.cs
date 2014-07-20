using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ubsHIDApp
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] list = UsbHID.getALLUsbDevices();
            foreach (String item in list)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }
    }
}
