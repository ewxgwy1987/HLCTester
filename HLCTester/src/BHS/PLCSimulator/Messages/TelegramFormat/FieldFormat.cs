using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BHS.PLCSimulator
{
    class FieldFormat
    {
        #region Class Attribute
        private string m_fieldname;
        private string m_offset;
        private string m_fieldlength;
        private string m_defaultvalue;

        // Add additional attributes to each field of telegram
        // So that each telegram can be defined and implemented just from xml file.
        // If there is any modification of telegram format in the future projects, 
        // then just modify the CFG_Telegrams.xml to redefine the format of telegram

        // The new attributes are "datatype" and "showlength"
        // datatype: the data type of variable used to store the value of field
        // showlength: the length of string to display the value of field in the client with pad rules
        private string m_datatype;
        private string m_showlength;

        public string FieldName
        {
            get
            {
                return this.m_fieldname;
            }
            set
            {
                this.m_fieldname = value;
            }
        }

        public string Offset
        {
            get
            {
                return this.m_offset;
            }
            set
            {
                this.m_offset = value;
            }
        }

        public string FieldLength
        {
            get
            {
                return this.m_fieldlength;
            }
            set
            {
                this.m_fieldlength = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return this.m_defaultvalue;
            }
            set
            {
                this.m_defaultvalue = value;
            }
        }

        public string DataType
        {
            get
            {
                return this.m_datatype;
            }
            set
            {
                this.m_datatype = value;
            }
        }

        public string ShowLength
        {
            get
            {
                return this.m_showlength;
            }
            set
            {
                this.m_showlength = value;
            }
        }
        #endregion

        public FieldFormat
            (string fieldname, string offset, string fieldlength, string defaultvalue, string datatype, string showlength)
        {
            this.m_fieldname = fieldname;
            this.m_offset = offset;
            this.m_fieldlength = fieldlength;
            this.m_defaultvalue = defaultvalue;

            this.m_datatype = datatype;
            this.m_showlength = showlength;
        }


    }
}
