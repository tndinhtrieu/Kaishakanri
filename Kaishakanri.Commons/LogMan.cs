using System;
using System.IO;
using System.Text;
using System.Configuration;

namespace Kaishakanri.Commons
{
    /// <summary>
	/// write log to file.
	/// </summary>
	public sealed class LogMan
	{
        public enum LogLevel
        {
            Error = 1,
            Warn = 2,
            Info = 3,
            Verbose = 4
        }

        #region Singlton

        /// <summary>
        /// Get the Instance of the Log class
        /// </summary>
        public static readonly LogMan Instance = new LogMan();
        #endregion

        #region Variables
        private long _logSizeLimit = 512000;
        private string _logFileName;
        private string _logFilePath;
        private string _logFilePathDefault = System.Windows.Forms.Application.StartupPath + @"\Logs";
        private LogLevel _level = LogLevel.Verbose;
	    private string mstrChannel;

        #endregion

        #region Properties
        public string LogFileName
        {
            get { return _logFileName; }
            set { _logFileName = value; }
        }
        public string LogFilePath
        {
            get { return _logFilePath; }
            set { _logFilePath = value; }
        }
        public long LogSizeLimit
        {
            get { return _logSizeLimit; }
            set { _logSizeLimit = value; }
        }
        #endregion

        private LogMan()
		{
			//
			// TODO: Add constructor logic here
			//
            string loglevel = Utils.GetAppConfig("LogLevel");
            if (!string.IsNullOrEmpty(loglevel))
            {
                _level = (LogLevel)(Convert.ToInt16(loglevel));
            }
            SetLogFilePath();
            SetLogFileName();

            //Prepare for to push the Monitor app
            SetChannel();
		}
        /// <summary>
        /// Set the path to the log file
        /// </summary>
        private void SetLogFilePath()
        {
            if (string.IsNullOrEmpty(this._logFilePath))
            {
                _logFilePath = _logFilePathDefault + @"\" + Utils.GetAppConfig("AppName");
            }
            if (string.IsNullOrEmpty(this._logFilePath))
            {
                _logFilePath = _logFilePathDefault;
            }
            if (!Directory.Exists(this._logFilePath))
            {
                Directory.CreateDirectory(this._logFilePath);
            }
        }
        /// <summary>
        /// Set the name of the log file
        /// </summary>
        private void SetLogFileName()
        {
            if (string.IsNullOrEmpty(this._logFileName))
            {
                this._logFileName = Path.Combine(this.LogFilePath, GetLogFileName());
            }
        }
		/// <summary>
		/// Get the name for the log file by actual date
		/// </summary>
		/// <returns></returns>
		private string GetLogFileName()
		{
            return DateTime.Now.ToString("yyMMddHHmmss") + ".log";
		}

        private void SetChannel()
        {
            if (string.IsNullOrEmpty(this.mstrChannel))
            {
                mstrChannel = Utils.GetAppConfig("AppName");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void CheckLogSize()
        {
            FileInfo fInfo = new FileInfo(this.LogFileName);
            if (fInfo.Exists)
            {
                long len = fInfo.Length;
                if (len > this.LogSizeLimit)
                {
                    _logFileName = Path.Combine(this.LogFilePath, GetLogFileName());
                }
            }
        }

		#region Writer
        /// <summary>
		/// Write to log file
		/// </summary>
		/// <param name="message">log message</param>
		public void WriteToLog(string message)
		{
            WriteToLog(message, LogLevel.Verbose);
		}
        /// <summary>
        /// Write to log file
        /// </summary>
        /// <param name="message">log message</param>
        /// <param name="level">log level to writing</param>
        public void WriteToLog(string message, LogLevel level)
        {
            if ((int)_level >= (int)level)
            {
                if (message != null)
                {
                    try
                    {
                        CheckLogSize();
                        StreamWriter write = null;
                        if (File.Exists(_logFileName))
                        {
                            write = File.AppendText(_logFileName);
                        }
                        else
                        {
                            write = File.CreateText(_logFileName);
                        }
                        write.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "." + DateTime.Now.Millisecond.ToString() + ":" + message);
                        write.Flush();
                        write.Close();
                        write.Dispose();
                    }
                    catch (Exception ex )
                    {
                        string str = ex.Message;
                    }
                }
            }
        }

		/// <summary>
		/// Write error message to the log file
		/// </summary>
		/// <param name="ex"></param>
		public void WriteErrorToLog(Exception ex)
		{
            WriteErrorToLog(ex, null);
		}
        public void WriteErrorToLog(Exception ex, string title)
        {
            if (ex != null)
            {
                string message = GetErrorMessage(ex);
                if (!string.IsNullOrEmpty(title))
                    message = title + Environment.NewLine + message;
                WriteToLog(message, LogLevel.Error);
            }
        }
        #endregion

		#region Error Message
		/// <summary>
		/// Get string error from the exeption
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public string GetErrorMessage(Exception ex)
		{
			if (ex==null)
				return "";
			string msg = "Error Source: " + ex.Source + Environment.NewLine;
			msg = msg + "Error StackTrace: " + ex.StackTrace + Environment.NewLine;
			msg = msg + "Error TargetSite: " + ex.TargetSite + Environment.NewLine;
			msg = msg + "Error Message: " + ex.Message + Environment.NewLine;
            if (ex.InnerException != null)
			    msg = msg + "Inner Exception: " + ex.InnerException + Environment.NewLine;
			return msg;
		}
		#endregion
   }
}
