using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Kaishakanri.Commons
{
    public class FtpHelper
    {
        private string _ftpAddress = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private NetworkCredential _credential = null;

        public FtpHelper(string ftpAddress, string username, string password)
        {
            _ftpAddress = ftpAddress;
            _username = username;
            _password = password;
            _credential = new NetworkCredential(_username, _password);
        }

        /// <summary>
        /// Return content file
        /// </summary>
        /// <param name="fileUrl">Link to file</param>
        /// <returns></returns>
        public byte[] DownloadFile(string fileUrl)
        {
            byte[] result = null;
            string fullUrl = string.Format(@"{0}/{1}", _ftpAddress, fileUrl);
            WebClient requestDownload = new WebClient();
            requestDownload.Credentials = _credential;
            try
            {
                result = requestDownload.DownloadData(fullUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (requestDownload != null)
                    requestDownload.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Get file list in specified directory input
        /// </summary>
        /// <param name="ftpUrlDir"></param>
        /// <returns></returns>
        private List<string> GetListFile(string ftpUrlDir)
        {
            List<string> fileList = new List<string>();
            string fullUrl = string.Format(@"{0}/{1}", _ftpAddress, ftpUrlDir);
            FtpWebRequest request = null;
            FtpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;
            try
            {
                request = (FtpWebRequest)WebRequest.Create(fullUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = _credential;

                response = (FtpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                while (true)
                {
                    string str = reader.ReadLine();
                    if (str == null)
                        break;
                    if(!string.IsNullOrEmpty(str.Trim()))
                        fileList.Add(str);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (responseStream != null)
                    responseStream.Close();
                if (response != null)
                    response.Close();
                if (request != null)
                    request.Abort();
            }
            return fileList;
        }

        public static void UploadToFTP(String inFTPServerAndPath, String inFullPathToLocalFile, String inUsername, String inPassword)
        {
            // Get the local file name: C:\Users\Rhyous\Desktop\File1.zip
            // and get just the filename: File1.zip. This is so we can add it
            // to the full URI.
            String filename = Path.GetFileName(inFullPathToLocalFile);

            // Open a request using the full URI, c/file.ext
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(inFTPServerAndPath + "/" + filename);

            // Configure the connection request
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(inUsername, inPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            // Create a stream from the file
            FileStream stream = File.OpenRead(inFullPathToLocalFile);
            byte[] buffer = new byte[stream.Length];

            // Read the file into the a local stream
            stream.Read(buffer, 0, buffer.Length);

            // Close the local stream
            stream.Close();

            // Create a stream to the FTP server
            Stream reqStream = request.GetRequestStream();

            // Write the local stream to the FTP stream
            // 2 bytes at a time
            int offset = 0;
            int chunk = (buffer.Length > 2048) ? 2048 : buffer.Length;
            while (offset < buffer.Length)
            {
                reqStream.Write(buffer, offset, chunk);
                offset += chunk;
                chunk = (buffer.Length - offset < chunk) ? (buffer.Length - offset) : chunk;
            }
            // Close the stream to the FTP server
            reqStream.Close();
        }
        public static void DownloadFTP(String inLocalpath, String inFullPathToNetworkFile, String inUsername, String inPassword)
        {
            FtpWebResponse response=null;
            Stream responseStream=null;
            StreamReader reader=null;
            FileStream file = null;
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(inFullPathToNetworkFile);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(inUsername, inPassword);

                response = (FtpWebResponse)request.GetResponse();

                responseStream = response.GetResponseStream();
                reader = new StreamReader(responseStream);
                file = File.Create(inLocalpath);
                byte[] buffer = new byte[1024];
                int read;
                //reader.Read(
                while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, read);
                }
               
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
               if(file!=null)  file.Close();
               if(reader!=null) reader.Close();
               if(response!=null) response.Close();
               if (responseStream != null) responseStream.Dispose();
               GC.Collect();
               GC.WaitForPendingFinalizers();
            }
        }

       
        public static long GetLength(String inFTPServerAndPath, String inUsername, String inPassword)
        {
            try
            {
                FtpWebRequest reqSize = (FtpWebRequest)FtpWebRequest.Create(new Uri(inFTPServerAndPath));
                reqSize.Credentials = new NetworkCredential(inUsername, inPassword);
                reqSize.Method = WebRequestMethods.Ftp.GetFileSize;
                reqSize.UseBinary = true;

                FtpWebResponse loginresponse = (FtpWebResponse)reqSize.GetResponse();
                FtpWebResponse respSize = (FtpWebResponse)reqSize.GetResponse();
                respSize = (FtpWebResponse)reqSize.GetResponse();
                long size = respSize.ContentLength;
                respSize.Close();
                return size;
            }
            catch
            {
                return 0;
            }
           
        }
        public static bool FtpDirectoryExists(string directory, string username, string password)
        {

            try
            {
                var request = (FtpWebRequest)WebRequest.Create(directory);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.GetDateTimestamp;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
                else
                    return true;
            }
            return true;

        }

        public static bool FtpMakeDirectory(string directory, string username, string password)
        {

            try
            {
                var request = (FtpWebRequest)WebRequest.Create(directory);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
                else
                    return true;
            }
            return true;

        }


    }
}
