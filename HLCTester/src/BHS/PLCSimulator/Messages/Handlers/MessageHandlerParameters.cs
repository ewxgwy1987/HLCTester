using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

namespace BHS.PLCSimulator
{
    public class MessageHandlerParameters : PALS.Common.IParameters, IDisposable
    {
        #region Class Fields Declaration and Properties

        // The name of current class 
        private static readonly string _className =
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString();
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger =
                    log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Class Constructor & Destructor
        public MessageHandlerParameters(XmlNode configSet, XmlNode telegramSet)
        {
            if (telegramSet == null)
                throw new Exception("Constractor parameter can not be null! Creating class " + _className +
                    " object fail! <BHS.Gateway.TCPClientTCPClientChains.Messages.Handlers.MessageHandlerParameters.Constructor()>");

            if (configSet == null)
                throw new Exception("Constractor parameter can not be null! Creating class " + _className +
                    " object fail! <BHS.Gateway.TCPClientTCPClientChains.Messages.Handlers.MessageHandlerParameters.Constructor()>");

            if (Init(ref configSet, ref telegramSet) == false)
                throw new Exception("Instantiate class object failure! " +
                    "<BHS.Gateway.TCPClientTCPClientChains.Messages.Handlers.MessageHandlerParameters.Constructor()>");
        }
        // <summary>
        /// Class destructer.
        /// </summary>
        ~MessageHandlerParameters()
        {
            Dispose(false);
        }

        /// <summary>
        /// Class method to be called by class wrapper for release resources explicitly.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. If disposing equals true, 
        // the method has been called directly or indirectly by a user's code. Managed and 
        // unmanaged resources can be disposed.
        // If disposing equals false, the method has been called by the runtime from inside the 
        // finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
        }

        #endregion

        #region Class Methods

        /// <summary>
        /// Class Initialization.
        /// </summary>
        /// <param name="configSet"></param>
        /// <param name="telegramSet"></param>
        /// <returns></returns>
        public bool Init(ref XmlNode configSet, ref XmlNode telegramSet)
        {
            string thisMethod = _className + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "()";
            return true;
        }

        #endregion
    }
}
