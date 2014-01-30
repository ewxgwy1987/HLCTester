using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BHS
{
    public class Util
    {
        private static uint m_gid_seed;
        private static Object gidLock = new Object();

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
    }
}


