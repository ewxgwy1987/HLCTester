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
        #endregion

        public FieldFormat
            (string fieldname, string offset, string fieldlength, string defaultvalue)
        {
            this.m_fieldname = fieldname;
            this.m_offset = offset;
            this.m_fieldlength = fieldlength;
            this.m_defaultvalue = defaultvalue;
        }


    }
}
