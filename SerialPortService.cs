using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LedOperasi
{
    class SerialPortService
    {
        //const int SLAVE_ADDRESS = 1;
        //const int COIL_ADDRESS = 32;

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private string SLAVE_ADDRESS;
        private string start_Address;
        private string register_Qty;
        private string commport;
        private string baud_rate;
        private string parity;
        private string stopBits;
        private string dataBits;

        modbus mb = new modbus();
        SerialPort sp = new SerialPort();

        public SerialPortService()
        {
            try
            {
                _log.Debug("Serial Port Service started...");
                commport = ConfigurationManager.AppSettings.Get("comm_port");
                _log.Debug(String.Format("Communication Port Installed:{0}...", commport));
                baud_rate = ConfigurationManager.AppSettings.Get("baud_rate");
                _log.Debug(String.Format("Baud Rate Installed:{0}...", baud_rate));
                parity= ConfigurationManager.AppSettings.Get("parity");
                _log.Debug(String.Format("Parity Installed:{0}...", parity));
                stopBits= ConfigurationManager.AppSettings.Get("stopbits");
                _log.Debug(String.Format("Stopbits Installed:{0}...", stopBits));
                dataBits = ConfigurationManager.AppSettings.Get("databits");
                _log.Debug(String.Format("Databits Installed:{0}...", dataBits));
                SLAVE_ADDRESS = ConfigurationManager.AppSettings.Get("slave_address");
                _log.Debug(String.Format("SLAVE_ADDRESS Installed:{0}...", SLAVE_ADDRESS));
                start_Address = ConfigurationManager.AppSettings.Get("start_address");
                _log.Debug(String.Format("start_Address Installed:{0}...", start_Address));
                register_Qty= ConfigurationManager.AppSettings.Get("register_qty");
                _log.Debug(String.Format("register_Qty Installed:{0}...", register_Qty));
            }
            catch (Exception ex)
            {
                _log.Debug(String.Format("Check app.config !!\nShould be available configuration with comm_port and baud_rate\n{0}",ex.Message));
            }
            
        }
        //call this procedure first after instantiate this object;
        public Boolean setSerialPort()
        {
            var portnames = SerialPort.GetPortNames();
            if (portnames.Length == 0)
            {
                _log.Debug(String.Format("System Communication Port Is not Installed..."));
                return false;
            }

            Boolean isAvailable = false;
            foreach (String port in portnames)
            {
                if (port.ToUpper().Equals(commport.ToUpper()))
                {
                    isAvailable = true;
                    break;
                }
            }
            if (!isAvailable)
                _log.Debug(String.Format("Communication Port Is not Available..."));
            else
                _log.Debug(String.Format("Communication Port Is Available:{0}",commport));

            return isAvailable;
        }
        public bool openPort()
        {
            int _baudrate = Convert.ToInt32(this.baud_rate);

            if (mb.Open(commport, _baudrate, SetPortDataBits(), SetPortParity(), SetPortStopBits()))
            {
                return true;
            }
            return false;
        }

        public float readTemperature()
        {
            ushort pollStart = Convert.ToUInt16(start_Address);
            ushort pollLength = Convert.ToUInt16(register_Qty);
            short[] values = new short[Convert.ToInt32(register_Qty)];
            _log.Debug(String.Format("Start:{0},lengh:{1},register_qty:{2}", start_Address, register_Qty, register_Qty));
            try
            {
                while (!mb.getTemperature(Convert.ToByte(SLAVE_ADDRESS), pollStart, pollLength, ref values)) ;
                string hex;
                hex = values[3].ToString("X") + values[4].ToString("X");
                int intValue = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return intValue / 10.0f;
            }
            catch (Exception err)
            {
                _log.Debug(String.Format("Error in readTemperature:{0}", err.Message));
            }
            return 0.0f;
        }

        public void stopSerialService()
        {
            mb.Close();
            mb = null;
        }

        public float readHumidity()
        {
            ushort pollStart = Convert.ToUInt16(start_Address);
            ushort pollLength = Convert.ToUInt16(register_Qty);
            short[] values = new short[Convert.ToInt32(register_Qty)];
            _log.Debug(String.Format("Start:{0},lengh:{1},register_qty:{2}", start_Address,register_Qty,register_Qty));
            try
            {
                while (!mb.getHumidity(Convert.ToByte(SLAVE_ADDRESS), pollStart, pollLength, ref values)) ;
                string hex;
                hex = values[3].ToString("X") + values[4].ToString("X");
                int intValue = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                return intValue / 10.0f;
            }
            catch (Exception err)
            {
                _log.Debug(String.Format("Error in readHumidity:{0}", err.Message));
            }
            return 0.0f;
        }

        private Parity SetPortParity()
        {
            return (Parity)Enum.Parse(typeof(Parity), parity);
        }

        private int SetPortDataBits()
        {
            return int.Parse(dataBits);
        }

        private StopBits SetPortStopBits()
        {
            return (StopBits)Enum.Parse(typeof(StopBits),stopBits);
        }
    }
}
