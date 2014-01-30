using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Xml;
using PALS.Utilities;

namespace BHS.PLCSimulator
{
    // Item Screened (0004)
    public class ICR_Telegram : SAC2PLCTelegram
    {
        #region Class Field and Property
        // Class const variable
        private const string m_aliasname = "ICR";

        // fdn - field name
        private const string FDN_GID_MSB = "GID_MSB";
        private const string FDN_GID_LSB = "GID_LSB";
        private const string FDN_LOCATION = "LOCATION";
        private const string FDN_SCR_LVL = "SCR_LVL";
        private const string FDN_SCR_RES = "SCR_RES";
        private const string FDN_PLC_IDX = "PLC_IDX";

        // Real Value Length (For Display)
        private const int LEN_GID_MSB = 2;
        private const int LEN_GID_LSB = 8;
        private const int LEN_LOCATION = 5;
        private const int LEN_SCR_LVL = 1;
        private const int LEN_SCR_RES = 2;
        private const int LEN_PLC_IDX = 5;

        // private variable -- Field Value
        private byte m_GID_MSB = 0;
        private uint m_GID_LSB = 0;
        private ushort m_location = 0;
        private byte m_SCR_LVL = 0;
        private byte m_SCR_RES = 0;
        private ushort m_PLC_IDX = 0;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string GID_MSB
        {
            get
            {
                //return m_GID_MSB.ToString();
                return m_GID_MSB.ToString("D" + LEN_GID_MSB.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_GID_MSB = Convert.ToByte(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string GID_LSB
        {
            get
            {
                //return m_GID_LSB.ToString();
                return m_GID_LSB.ToString("D" + LEN_GID_LSB.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_GID_LSB = Convert.ToUInt32(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string LOCATION
        {
            get
            {
                //return m_location.ToString();
                return m_location.ToString("D" + LEN_LOCATION.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_location = Convert.ToUInt16(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string SCR_LVL
        {
            get
            {
                //return m_SCR_LVL.ToString();
                return m_SCR_LVL.ToString("D" + LEN_SCR_LVL.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_SCR_LVL = Convert.ToByte(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string SCR_RES
        {
            get
            {
                //return m_SCR_RES.ToString();
                return m_SCR_RES.ToString("D" + LEN_SCR_RES.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_SCR_RES = Convert.ToByte(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        public string PLC_IDX
        {
            get
            {
                //return m_PLC_IDX.ToString();
                return m_PLC_IDX.ToString("D" + LEN_PLC_IDX.ToString());
            }
            set
            {
                string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
                try
                {
                    this.m_PLC_IDX = Convert.ToUInt16(value);
                }
                catch (Exception exp)
                {
                    string errorstr = "Error in " + thisMethod + "  value=" + value + "\n" + exp.ToString();
                    Console.WriteLine(errorstr);
                    _logger.Error(errorstr);
                }
            }
        }

        #endregion 

        #region SAC2PLCTelegram Constructor, Dispose, Finalize and Destructor

        public ICR_Telegram(byte[] rawdata)
            : base(rawdata, m_aliasname)
        {
            this.TelegramDecoding();
        }

        public ICR_Telegram(byte[] rawdata, string channel)
            : base(rawdata, channel, m_aliasname)
        {
            //this.TelegramDecoding();
        }

        public ICR_Telegram()
            : base(m_aliasname)
        {
        }

        public ICR_Telegram
            (byte gid_msb, uint gid_lsb, ushort location, byte scr_lvl, byte scr_res, byte plc_idx)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, scr_lvl, scr_res, plc_idx);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public ICR_Telegram
            (string gid_msb, string gid_lsb, string location, string scr_lvl, string scr_res, string plc_idx)
            : base(m_aliasname)
        {
            SetICRData(gid_msb, gid_lsb, location, scr_lvl, scr_res, plc_idx);
            byte[] Telegram_datafield = this.TelegramEncoding();
        }

        public ICR_Telegram(XmlNode xml_TeleTestcase)
            : base(m_aliasname)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                string gid_msb = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_GID_MSB, "?");
                string gid_lsb = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_GID_LSB, "?");
                if (gid_msb == "?" || gid_lsb == "?")
                {
                    char[] gid = Util.GetGID();
                    gid_msb = new string(gid, 0, LEN_GID_MSB);
                    gid_lsb = new string(gid, LEN_GID_MSB, LEN_GID_LSB);
                }
                string location = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_LOCATION, "00000");
                string scr_lvl = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_SCR_LVL, "0");
                string scr_res = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_SCR_RES, "33");
                string plc_idx = XMLConfig.GetSettingFromInnerText(xml_TeleTestcase, FDN_PLC_IDX, "00");
                SetICRData(gid_msb, gid_lsb, location, scr_lvl, scr_res, plc_idx);
                byte[] Telegram_datafield = this.TelegramEncoding();
            }
            catch (Exception exp)
            {
                string errorstr = "Error in " + thisMethod + "\n" + exp.ToString();
                _logger.Error(errorstr);
            }
        }
        #endregion

        #region Member Function
        public bool SetICRData
            (string gid_msb, string gid_lsb, string location, string scr_lvl, string scr_res, string plc_idx)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            try
            {
                this.GID_MSB = gid_msb;
                this.GID_LSB = gid_lsb;
                this.LOCATION = location;
                this.SCR_LVL = scr_lvl;
                this.SCR_RES = scr_res;
                this.PLC_IDX = plc_idx;
                return true;
            }
            catch (Exception exp)
            {
                string errorstr = "Error in " + thisMethod + "\n" + exp.ToString();
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }
        }

        public bool SetICRData
            (byte gid_msb, uint gid_lsb, ushort location, byte scr_lvl, byte scr_res, byte plc_idx)
        {
            this.m_GID_MSB = gid_msb;
            this.m_GID_LSB = gid_lsb;
            this.m_location = location;
            this.m_SCR_LVL = scr_lvl;
            this.m_SCR_RES = scr_res;
            this.m_PLC_IDX = plc_idx;
            return true;
        }

        private bool IsValidByteData(byte[] bytedata, string fieldname, out string errorstr)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            errorstr = "";
            string datatype = "";

            switch (fieldname)
            {
                case FDN_GID_MSB:
                    if (bytedata.Length != Marshal.SizeOf(this.m_GID_MSB))
                        datatype = this.m_GID_MSB.GetType().ToString();
                    break;
                case FDN_GID_LSB:
                    if (bytedata.Length != Marshal.SizeOf(this.m_GID_LSB))
                        datatype = this.m_GID_LSB.GetType().ToString();
                    break;
                case FDN_LOCATION:
                    if (bytedata.Length != Marshal.SizeOf(this.m_location))
                        datatype = this.m_location.GetType().ToString();
                    break;
                case FDN_SCR_LVL:
                    if (bytedata.Length != Marshal.SizeOf(this.m_SCR_LVL))
                        datatype = this.m_SCR_LVL.GetType().ToString();
                    break;
                case FDN_SCR_RES:
                    if (bytedata.Length != Marshal.SizeOf(this.m_SCR_RES))
                        datatype = this.m_SCR_RES.GetType().ToString();
                    break;
                case FDN_PLC_IDX:
                    if (bytedata.Length != Marshal.SizeOf(this.m_PLC_IDX))
                        datatype = this.m_PLC_IDX.GetType().ToString();
                    break;
                default:
                    datatype = "Wrong Field Name";
                    errorstr += "The Field Name:" + fieldname + " is wrong.\n";
                    break;
            }

            if (datatype != "")
            {
                errorstr += "Position: " + thisMethod + '\n';
                errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                errorstr += "The length of Field:" + fieldname + " does not match the data type.\n";
                errorstr += "The Byte Array Length:" + bytedata.Length + " Data Type:" + datatype + '\n';
                return false;
            }
            else
                return true;
        }

        protected override bool AnalyseFieldData()
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errorstr = "";

            byte[] b_gidmsb = this.GetFieldValue(FDN_GID_MSB);
            byte[] b_gidlsb = this.GetFieldValue(FDN_GID_LSB);
            byte[] b_location = this.GetFieldValue(FDN_LOCATION);
            byte[] b_scrlvl = this.GetFieldValue(FDN_SCR_LVL);
            byte[] b_scrres = this.GetFieldValue(FDN_SCR_RES);
            byte[] b_plcidx = this.GetFieldValue(FDN_PLC_IDX);

            try
            {
                string temperror = "";
                // 1.GID_MSB
                if (IsValidByteData(b_gidmsb, FDN_GID_MSB, out temperror))
                    this.m_GID_MSB = b_gidmsb[0];
                errorstr += temperror;

                // 2.GID_LSB
                if (IsValidByteData(b_gidlsb, FDN_GID_LSB, out temperror))
                    this.m_GID_LSB = BitConverter.ToUInt32(Util.Reverse(b_gidlsb), 0);
                errorstr += temperror;

                // 3.LOCATION
                if (IsValidByteData(b_location, FDN_LOCATION, out temperror))
                    this.m_location = BitConverter.ToUInt16(Util.Reverse(b_location), 0);
                errorstr += temperror;

                // 4.SCR_LVL
                if (IsValidByteData(b_scrlvl, FDN_SCR_LVL, out temperror))
                    this.m_SCR_LVL = b_scrlvl[0];
                errorstr += temperror;

                // 5.SCR_RES
                if (IsValidByteData(b_scrres, FDN_SCR_RES, out temperror))
                    this.m_SCR_RES = b_scrres[0];
                errorstr += temperror;

                // 6.PLC_IDX
                if (IsValidByteData(b_plcidx, FDN_PLC_IDX, out temperror))
                    this.m_PLC_IDX = b_plcidx[0];
                errorstr += temperror;

            }
            catch (Exception exp)
            {
                errorstr += exp.ToString();
            }

            if (errorstr.Length != 0)
            {
                errorstr += "At Position: " + thisMethod + '\n';
                errorstr += "Telegram: " + this.m_TelegramAlias + '\n';
                Console.WriteLine(errorstr);
                _logger.Error(errorstr);
                return false;
            }

            return true;
        }

        public override string ShowAllData()
        {
            string showstr = "";
            showstr += "Telegram:" + this.TelegramAlias + "  Name:" + this.TelegramName + "\n";
            showstr += "TLG_TYPE:" + this.TLG_TYPE + " ";
            showstr += "TLG_LENGTH:" + this.TLG_LEN + " ";
            showstr += "TLG_SEQ:" + this.TLG_SEQ + " ";
            showstr += FDN_GID_MSB + ":" + this.GID_MSB + ' ';
            showstr += FDN_GID_LSB + ":" + this.GID_LSB + ' ';
            showstr += FDN_LOCATION + ":" + this.LOCATION + ' ';
            showstr += FDN_SCR_LVL + ":" + this.SCR_LVL + ' ';
            showstr += FDN_SCR_RES + ":" + this.SCR_RES + ' ';
            showstr += FDN_PLC_IDX + ":" + this.PLC_IDX + ' ';
            Console.WriteLine(showstr);
            return showstr;
            
        }

        protected override bool HasAllData()
        {
            return (this.m_GID_MSB > 0)
                && (this.m_GID_LSB > 0)
                && (this.m_location > 0)
                && (this.m_SCR_LVL > 0)
                && (this.m_SCR_RES > 0)
                && (this.m_PLC_IDX > 0);
        }

        protected override bool FillFieldData()
        {
            byte[] b_gidmsb = new byte[1] { this.m_GID_MSB };
            // BitConverter.GetBytes returns the byte array which is from low byte to hight byte
            byte[] b_gidlsb = Util.Reverse(BitConverter.GetBytes(this.m_GID_LSB));
            byte[] b_location = Util.Reverse(BitConverter.GetBytes(this.m_location));
            byte[] b_scrlvl = new byte[1] { this.m_SCR_LVL };
            byte[] b_scrres = new byte[1] { this.m_SCR_RES };
            byte[] b_plcidx = Util.Reverse(BitConverter.GetBytes(this.m_PLC_IDX));

            int datafield_length
                = b_gidmsb.Length
                + b_gidlsb.Length
                + b_location.Length
                + b_scrlvl.Length
                + b_scrres.Length
                + b_plcidx.Length;

            // create raw data template
            CreateRawdataTemplate(datafield_length);

            bool result = true;
            result &= SetFieldValue(FDN_GID_MSB, b_gidmsb);
            result &= SetFieldValue(FDN_GID_LSB, b_gidlsb);
            result &= SetFieldValue(FDN_LOCATION, b_location);
            result &= SetFieldValue(FDN_SCR_LVL, b_scrlvl);
            result &= SetFieldValue(FDN_SCR_RES, b_scrres);
            result &= SetFieldValue(FDN_PLC_IDX, b_plcidx);

            return result;
        }
        #endregion
    }
}
