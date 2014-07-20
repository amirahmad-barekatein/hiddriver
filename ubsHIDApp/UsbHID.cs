using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USBHIDDRIVER;

namespace ubsHIDApp
{
    public class UsbHID
    {
        USBHIDDRIVER.USBInterface usb = null;
        Log usbLog;

        public UsbHID(string vendorId, string productId)
        {
            usb = new USBInterface(vendorId, productId);
            if (!usb.Connect())
            {
                throw new InvalidOperationException("Usb can't connect to devices");
                return;
            }
            usbLog = new Log(productId, vendorId);
            usbLog.connectLog();
        }

        public static String[] getALLUsbDevices()
        {
            USBHIDDRIVER.USBInterface usbTemp = new USBInterface("0");
            if (usbTemp.getDeviceList().Length > 1)
                Console.WriteLine("dfsdfsdF");
            return usbTemp.getDeviceList();
        }

        public bool sendBytes(byte[] sendingBytes)
        {
            if (!usb.write(sendingBytes))
            {
                usbLog.ErrorLog(USBErrorTypes.SEND);
                return false;
            }
            usbLog.IntractionLog(sendingBytes, USBTransactionTypes.Send);
            return true;
        }

        public void Disconnect()
        {
            usb.Disconnect();
        }

        private bool sendStartCMD()
        {
            if (usb == null)
                return false;
            byte[] startCMD = new byte[8];
            //Start
            startCMD[0] = 255;
            //Mode
            startCMD[1] = 0;
            //USync
            startCMD[2] = 28;
            //ULine
            startCMD[3] = 20;
            //tSync
            startCMD[4] = 20;
            //tRepeat - High
            startCMD[5] = 0;
            //tRepeat - Low
            startCMD[6] = 0x01;
            //BusMode
            startCMD[7] = 0xF4;

            if (usb.write(startCMD))
            {
                usbLog.IntractionLog(startCMD, USBTransactionTypes.Send);
                return true;
            }
            usbLog.ErrorLog(USBErrorTypes.START_CMD_SEND);
            return false;
        }

        private bool sendStopCMD()
        {
            byte[] stopCMD = new byte[75];
            //Stop
            stopCMD[0] = 128;
            stopCMD[64] = 8;
            if (!usb.write(stopCMD))
            {
                usbLog.ErrorLog(USBErrorTypes.STOP_CMD_SEND);
                return false;
            }
            usbLog.IntractionLog(stopCMD, USBTransactionTypes.Send);
            return true;
        }

        public void startRead()
        {
            sendStartCMD();
            usb.enableUsbBufferEvent(new System.EventHandler(usbReadacher));
            Thread.Sleep(5);
            usb.startRead();
            Thread.Sleep(5);
            for (int i = 0; i < 200; i++)
            {
                //Assert.IsNotNull(USBHIDDRIVER.USBInterface.usbBuffer);
                Thread.Sleep(2);
            }
            usb.stopRead();
            sendStopCMD();
        }

        private void usbReadacher(object sender, EventArgs e)
        {
            Console.WriteLine("Event caught");
            if (USBHIDDRIVER.USBInterface.usbBuffer.Count > 0)
            {
                byte[] currentRecord = null;
                int counter = 0;
                while ((byte[])USBHIDDRIVER.USBInterface.usbBuffer[counter] == null)
                {
                    //Remove this report from list
                    lock (USBHIDDRIVER.USBInterface.usbBuffer.SyncRoot)
                    {
                        USBHIDDRIVER.USBInterface.usbBuffer.RemoveAt(0);
                    }
                }
                //since the remove statement at the end of the loop take the first element
                currentRecord = (byte[])USBHIDDRIVER.USBInterface.usbBuffer[0];
                lock (USBHIDDRIVER.USBInterface.usbBuffer.SyncRoot)
                {
                    USBHIDDRIVER.USBInterface.usbBuffer.RemoveAt(0);
                }
                //DO SOMETHING WITH THE RECORD HERE
            }
        }

    }
}