using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Kaishakanri.Commons
{
    public class MessageMan
    {
        private LogMan.LogLevel menmLogLevel = LogMan.LogLevel.Verbose;

        #region Singlton

        /// <summary>
        /// Get the Instance of the Mssage class
        /// </summary>
        public static readonly MessageMan Instance = new MessageMan();
        #endregion

        private MessageMan()
		{
			//
			// TODO: Add constructor logic here
			//
            string loglevel = Utils.GetAppConfig("LogLevel");
            if (!string.IsNullOrEmpty(loglevel))
            {
                menmLogLevel = (LogMan.LogLevel)(Convert.ToInt16(loglevel));
            }
		}

        public void MsgDialogShow(string pstrTitle, string pstrMessage, LogMan.LogLevel penmLevel)
        {
            if ((int)menmLogLevel >= (int)penmLevel)
            {
                switch (penmLevel)
                {
                    case LogMan.LogLevel.Error:
                        MessageBox.Show(pstrMessage, pstrTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogMan.Instance.WriteToLog(pstrMessage, penmLevel);
                        break;
                    case LogMan.LogLevel.Warn:
                        MessageBox.Show(pstrMessage, pstrTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        LogMan.Instance.WriteToLog(pstrMessage, penmLevel);
                        break;
                    case LogMan.LogLevel.Info:
                        MessageBox.Show(pstrMessage, pstrTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogMan.Instance.WriteToLog(pstrMessage, penmLevel);
                        break;
                    case LogMan.LogLevel.Verbose:
                        MessageBox.Show(pstrMessage, pstrTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogMan.Instance.WriteToLog(pstrMessage, penmLevel);
                        break;
                }
            }
        }

        public void MsgDialogShow(string pstrTitle, Exception ex)
        {
            MessageBox.Show(ex.Message, pstrTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            LogMan.Instance.WriteErrorToLog(ex);
        }

        public void MsgShow(string pstrTitle, string pstrMessage, MessageBoxIcon iIcon)
        {
            MessageBox.Show(pstrMessage, pstrTitle, MessageBoxButtons.OK, iIcon);
        }
    }
}
