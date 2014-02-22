using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Collections;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using PALS.Utilities;

namespace BHS
{
    public class Util
    {
        private static uint m_gid_seed;
        private static Object gidLock = new Object();
        public static Hashtable HT_LocToLocid;
        public static Hashtable HT_LocidToLoc;

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static Util()
        {
            DateTime dt = DateTime.Now;
            uint year = Convert.ToUInt32(dt.Year % 100);
            uint month = 1;
            //uint month = Convert.ToUInt32(dt.Month);
            uint day = Convert.ToUInt32(dt.Day);
            uint hour = Convert.ToUInt32(dt.Hour);
            uint min = Convert.ToUInt32(dt.Minute);
            uint sec = Convert.ToUInt32(dt.Second);
            uint tms = Convert.ToUInt32(dt.Millisecond >> 4); // The minimum time interval is 16ms

            uint temp_gid = 0;
            temp_gid |= tms << 0; // 6 bits
            temp_gid |= sec << 6; // 6 bits
            temp_gid |= min << 12;// 6 bits
            temp_gid |= hour << 18; // 5 bits
            temp_gid |= day << 23; // 5 bits
            temp_gid |= month << 28; // 4 bits

            m_gid_seed = temp_gid;

        }

        /// <summary>
        /// Reverse the order of byte array
        /// </summary>
        /// <param name="mybyte"></param>
        /// <returns></returns>
        public static byte[] Reverse(byte[] mybyte)
        {
            int i;
            byte b_temp;

            for (i = 0; i < mybyte.Length / 2; i++)
            {
                b_temp = mybyte[i];
                mybyte[i] = mybyte[mybyte.Length - 1 - i];
                mybyte[mybyte.Length - 1 - i] = b_temp;
            }

            return mybyte;
        }

        /// <summary>
        /// conver "48,48,48,49" to "0001"
        /// </summary>
        /// <param name="default_type"></param>
        /// <returns></returns>
        public static string DecByteStrToStr(string default_type)
        {
            string type = "";
            char[] ch_separator = new char[] { ',' };
            char[] ch_type = new char[4];
            int i;

            if (default_type != "")
            {
                string[] substr_type = default_type.Split(ch_separator, 4);
                for (i = 0; i < ch_type.Length; i++)
                {
                    ch_type[i] = Convert.ToChar(Convert.ToInt32(substr_type[i]));
                }
                type = new string(ch_type);
            }
            return type;
        }

        /// <summary>
        /// conver "48,48,48,49" to byte[]{48,48,48,49}
        /// </summary>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static byte[] DecByteStrToArray(string default_value)
        {
            char[] ch_separator = new char[] { ',' };
            string[] substr_value = default_value.Split(ch_separator);
            byte[] default_bytes = new byte[substr_value.Length];

            int i;
            for (i = 0; i < default_bytes.Length; i++)
            {
                default_bytes[i] = Convert.ToByte(substr_value[i]);
            }

            return default_bytes;
        }

        /// <summary>
        ///  conver <F1><C0> to byte[]{0xF1,0xC0}
        /// </summary>
        /// <param name="default_value"></param>
        /// <returns></returns>
        public static byte[] HexByteStrToArray(string default_value)
        {
            char[] ch_separator = new char[] { '<','>' };
            string[] substr_value = default_value.Split(ch_separator,StringSplitOptions.RemoveEmptyEntries);
            byte[] default_bytes = new byte[substr_value.Length];

            int i;
            for (i = 0; i < default_bytes.Length; i++)
            {
                default_bytes[i] = Convert.ToByte(substr_value[i], 16);
            }

            return default_bytes;
        }

        /// <summary>
        /// convert byte[]{48,48,48,49} to char[]{'0','0','0','1'}
        /// </summary>
        /// <param name="bytearray"></param>
        /// <returns></returns>
        public static char[] ByteArrayToCharArray(byte[] bytearray)
        {
            char[] chararray = new char[bytearray.Length];

            if (bytearray == null)
            {
                return null;
            }

            int i;
            for (i = 0; i < bytearray.Length; i++)
                chararray[i] = Convert.ToChar(bytearray[i]);

            return chararray;
        }

        /// <summary>
        /// Alphanumeric Padding Rule: Left justified, space filled
        /// </summary>
        /// <para>hahhahahahahha</para>
        /// <param name="org_chstr">the original char array</param>
        /// <param name="target_length">the target length</param>
        /// <returns></returns>
        public static char[] CharPad(char[] org_chstr, int target_length)
        {
            char[] new_chstr = new char[target_length];
            if (org_chstr.Length > target_length)
            {
                Array.Copy(org_chstr, 0, new_chstr, 0, target_length);
            }
            else
            {
                Array.Copy(org_chstr, 0, new_chstr, 0, org_chstr.Length);
                int i;
                for (i = org_chstr.Length; i < target_length; i++)
                    new_chstr[i] = ' ';
            }

            return new_chstr;
        }

        /// <summary>
        /// Numeric Padding Rule: Right justified, '0' filled
        /// </summary>
        /// <param name="org_numstr">The original number</param>
        /// <param name="target_length">The target length</param>
        /// <returns>The Padded char array with length indicated by target_length</returns>
        public static char[] NumericPad(char[] org_numstr, int target_length)
        {
            char[] new_numstr = new char[target_length];
            for (int i = 0; i < new_numstr.Length; i++)
                new_numstr[i] = '0';

            int offset = target_length - org_numstr.Length;
            if (offset >= 0)
                Array.Copy(org_numstr, 0, new_numstr, offset, org_numstr.Length);
            else
                Array.Copy(org_numstr, 0, new_numstr, 0, target_length);
            return new_numstr;
        }

        /// <summary>
        /// Return a 10 digits GID serial number
        /// </summary>
        /// <returns>10 digits GID serial number</returns>
        public static char[] GetGID()
        {
            uint gid;
            lock (gidLock)
            {
                gid = m_gid_seed;
                m_gid_seed++;
            }
            return NumericPad(gid.ToString().ToCharArray(), 10);
        }

        public static bool HTInit(XmlNode xnode_util)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string colname_loc = "LOCATION";
            string colname_locid = "LOCATION_ID";

            bool res = true;
            try
            {
                HT_LocToLocid = new Hashtable();
                HT_LocidToLoc = new Hashtable();
                string connstr = XMLConfig.GetSettingFromInnerText(xnode_util, "connectionString", "");
                string locsql = XMLConfig.GetSettingFromInnerText(xnode_util, "LocationSql", "");

                // connect to database to retrieve location data
                DataTable dt_loc = new DataTable();
                SqlConnection sqlconn = new SqlConnection(connstr);
                sqlconn.Open();
                SqlCommand sqlcmd = new SqlCommand(locsql);
                sqlcmd.Connection = sqlconn;
                SqlDataReader reader = sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader.HasRows)
                    dt_loc.Load(reader);
                reader.Close();
                sqlconn.Close();

                string loc, locid;
                if (dt_loc.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt_loc.Rows)
                    {
                        loc = string.Empty;
                        locid = string.Empty;
                        if (dr.Table.Columns[colname_loc] != null && dr.Table.Columns[colname_locid] != null)
                        {
                            if (dr[colname_loc] != DBNull.Value && dr[colname_locid] != DBNull.Value)
                            {
                                loc = dr[colname_loc].ToString();
                                locid = dr[colname_locid].ToString();
                            }
                            else if (dr[colname_loc] != DBNull.Value && dr[colname_locid] == DBNull.Value)
                            {
                                loc = dr[colname_loc].ToString();
                                locid = dr[colname_loc].ToString();
                            }
                        }
                        else if (dr.Table.Columns[colname_loc] != null && dr.Table.Columns[colname_locid] == null)
                        {
                            if (dr[colname_loc] != DBNull.Value)
                            {
                                loc = dr[colname_loc].ToString();
                                locid = dr[colname_loc].ToString();
                            }
                        }

                        if (loc != string.Empty && locid != string.Empty)
                        {
                            HT_LocidToLoc.Add(locid, loc);
                            HT_LocToLocid.Add(loc, locid);
                        }
                    }
                }
                else
                {
                    errstr += "Cannot Find location data from database.";
                    _logger.Error(errstr);
                    res = false;
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
                return false;
            }

            return res;
        }

        public static bool Init(XmlNode xnode_util)
        {
            bool res = true;
            res &= HTInit(xnode_util);
            return res;
        }


        public static string ConverLocNameToLocID(string location)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string conv_locid = location;
            try
            {
                if (HT_LocToLocid.ContainsKey(location))
                    conv_locid = HT_LocToLocid[location] as string;
                else
                {
                    errstr += "Unknown Location:" + location;
                    _logger.Warn(errstr);
                }
            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return conv_locid;
        }

        public static string CovertLocIDToLocName(string location_id)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            string errstr = "Class:[" + _className + "]" + "Method:<" + thisMethod + ">\n";

            string conv_loc = location_id;
            try
            {
                if (HT_LocidToLoc.ContainsKey(location_id))
                    conv_loc = HT_LocidToLoc[location_id] as string;
                else
                {
                    errstr += "Unknown LocationID:" + location_id;
                    _logger.Warn(errstr);
                }

            }
            catch (Exception exp)
            {
                errstr += exp.ToString();
                _logger.Error(errstr);
            }

            return conv_loc;
        }
    }
}


