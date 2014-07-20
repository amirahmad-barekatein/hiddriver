using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO ;

namespace ubsHIDApp
{
    public class Log
    {
        string productId;
        string vendorId;
        StreamWriter logger = null; 

        public Log(string productId, string vendorId)
        {
            if (  productId == ""  || productId == null 
               || vendorId == "" || vendorId == null )
                return ;

            this.productId = productId;
            this.vendorId = vendorId;
            string filePath = string.Format("USB_{0}_{1}" , productId , vendorId);
            try
            {
                logger = new StreamWriter(filePath, File.Exists(filePath));
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void connectLog(){
            string lineToBeWritten = string.Format ("Connecting at : {0}" , DateTime.Now.ToString()) ;
            writeLog(lineToBeWritten);
        }

        public void IntractionLog(byte[] transactiveBytes , USBTransactionTypes trType ){
            string transactionLogString = string.Format("{0} at: {1}\n[" 
                                                        , trType.ToString() 
                                                        , DateTime.Now.ToString());
            foreach (byte transactiveByte in transactiveBytes)
                transactionLogString += " " + transactiveByte.ToString() + ",";
            transactionLogString.Remove(transactionLogString.Length - 1);
            transactionLogString += "]";
            writeLog(transactionLogString);
        }

        public void ErrorLog( USBErrorTypes usbError )
        {
            string logErrorString = "" ; 
            switch (usbError)
            {
                case USBErrorTypes.START_CMD_SEND:
                    logErrorString = "Error : Send Start CMD ";
                    break;
                case USBErrorTypes.SEND:
                    logErrorString = "Error : Send Bytes";
                    break;
                case USBErrorTypes.RECIEVE:
                    logErrorString = "Error : Recieve Data";
                    break;
                case USBErrorTypes.STOP_CMD_SEND :
                    logErrorString = "Error : Send Stop Command";
                    break;
            }
            writeLog(logErrorString);
        }

        private void writeLog(string logLine)
        {
            if (logger == null){
                throw new IOException();
                return ;
            }
            try
            {
                logger.WriteLine(logLine);
            }
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
