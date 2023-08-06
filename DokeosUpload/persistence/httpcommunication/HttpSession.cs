/*
*  
*  Copyright (C) 2010-2011 University College Ghent.
*  
*  For a full list of contributors, see "credits.txt".
*  
*  This file is part of LMS Desktop Assistant.
*  
*  LMS Desktop Assistant is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  LMS Desktop Assistant is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with LMS Desktop Assistant. If not, see <http://www.gnu.org/licenses/>.
*  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using lmsda.domain;

namespace lmsda.persistence.httpcommunication
{
    /// <summary>
    /// This class was originally written in Visual Basic by Patrick Lauwaerts, to
    /// emulate web browser POST requests for sending form data, to allow automating
    /// the process of logging in and performing operations on an online platform.
    /// It was converted to C# and refactored by Maarten Meuris.
    ///     
    ///     General usage method:
    ///      1) create a new HttpSession object
    ///      2) use setRequestUrl to set the URL
    ///      3) use addNameValuePair to add data
    ///      4) for an upload: set the file with setUploadFile
    ///      5) perform a SendPOSTRequest of a certain type
    /// </summary>
    class HttpSession
    {
        private Encoding            encoding;
        private HttpWebRequest      httpWebRequest;
        private HttpWebResponse     httpWebResponse;
        private CookieContainer     cookieJar;
        private String              requestUrl          = String.Empty;
        private String              responseFromServer  = String.Empty;
        private String              responseUrl         = String.Empty;
        private String              boundary            = "bh19ZwG9887GM0fAyLji";
        private byte[]              postData;

        private String              inputName;
        private String              uploadFilename;
        private String              uploadFilePath;
        private String              uploadMIMEtype;
        private String              userAgent;

        private String              downloadFilename;
        private String              downloadFilePath;
        
        private List<String[]>      nameValuePairs;
        private Boolean             error;

        #region Constructor


        /// <summary>
        ///     Creates a new HTTPSession, with the text encoding used on the target server.
        /// </summary>
        /// <param name="encoding">The text encoding used on the target server</param>
        public HttpSession(Encoding encoding): this(encoding, null)
        { }


        /// <summary>
        ///     Creates a new HTTPSession, with the text encoding used on the target server.
        /// </summary>
        /// <param name="encoding">The text encoding used on the target server</param>
        /// <param name="userAgent">The name of the user agent making the session</param>
        public HttpSession(Encoding encoding, String userAgent)
        {
            error=false;
            // fix for the error "The remote server returned an error: (417) Expectation Failed."
            System.Net.ServicePointManager.Expect100Continue = false;
            nameValuePairs = new List<String[]>();
            this.encoding = encoding;
            this.cookieJar = new CookieContainer();
            this.userAgent = userAgent;
        }

        #endregion

        #region Adding parameters

        /// <summary>
        ///     Sets the URL to which the next request will be sent,
        ///     and clears the response information of the previous requests.
        /// </summary>
        /// <param name="url"></param>
        public void setRequestUrl(String url)
        { 
            this.requestUrl = url;
            this.error = false;
            this.responseFromServer  = String.Empty;
            this.responseUrl = String.Empty;
        }

        
        public String getRequestUrl()
        {
            return this.requestUrl;
        }

        /// <summary>
        ///     Adds a parameter of the type name=value.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        public void addNameValuePair(String name, String value)
        {
            addNameValuePair(name, value, true);
        }

        /// <summary>
        ///     Adds a parameter of the type name=value.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        /// <param name="allowDuplicates">True to keep duplicates of the same name</param>
        public void addNameValuePair(String name, String value, Boolean allowDuplicates)
        {
            if (!allowDuplicates)
            {
                List<String[]> nameValuePairs2 = new List<String[]>(nameValuePairs);
                foreach (String[] nameValuePair in nameValuePairs2)
                    if (nameValuePair[0].Equals(name))
                        nameValuePairs.Remove(nameValuePair);
            }
            if(value == null) value = String.Empty;
            if(name != null)
                nameValuePairs.Add(new String[]{name,value});
        }

        /// <summary>
        ///     Removes all currently stored name=value pairs
        /// </summary>
        public void clearNameValuePairs()
        { 
            nameValuePairs.Clear();
        }

        /// <summary>
        ///     sets the name-value pairs to an external list of pairs
        /// </summary>
        public Boolean setNameValuePairs(List<String[]> nameValuePairs)
        {
            foreach (String[] str in nameValuePairs)
            {
                // need to contain 2 strings
                if (str.Length< 2)
                    return false;
            }

            this.nameValuePairs = new List<String[]>(nameValuePairs);
            return true;
        }

        /// <summary>
        ///     Returns a copy of the internally stored name-value pairs
        /// </summary>
        public List<String[]> getNameValuePairs()
        {
            return new List<String[]>(this.nameValuePairs);
        }

        /// <summary>
        ///     Sets the user agent string. Leave NULL to default to the inbuilt LMSDA identification.
        /// </summary>
        /// <param name="userAgent">the user agent string</param>
        public void setUserAgent(String userAgent)
        {
            this.userAgent = userAgent;
        }

        /// <summary>
        ///     Gets the user agent string.
        /// </summary>
        /// <returns>The user agent string.</returns>
        public String getUserAgent()
        {
            return this.userAgent;
        }

        /// <summary>
        ///     Sets the file that needs to be uploaded
        /// </summary>
        /// <param name="inputName">The input name of the file browser.</param>
        /// <param name="filename">filenale on the server</param>
        /// <param name="fullFilePath">Full path of the file to upload</param>
        /// <param name="mimetype">MIME Type of the file. Can be null if the type is unknown.</param>
        public void setUploadFile(String inputName, String filename, String fullFilePath, String mimetype)
        {
            this.inputName      = inputName;
            this.uploadFilename = filename;
            this.uploadFilePath = fullFilePath;
            this.uploadMIMEtype = mimetype;
        }

        #endregion

        #region Getting the response information saved from the server

        /// <summary>
        ///     Returns the answer that was returned by the server. This value is not used for download requests.
        /// </summary>
        /// <returns>The HTML page that was returned by the server.</returns>
        public String getResponseFromServer()
        { 
            return this.responseFromServer;
        }

        /// <summary>
        ///     Returns the URL that was returned by the server.
        /// </summary>
        /// <returns>The URL that was returned by the server.</returns>
        public String getResponseUrl()
        { 
            return this.responseUrl;
        }

        /// <summary>
        ///     Returns the filename of a downloaded file.
        /// </summary>
        /// <returns>The filename of a downloaded file.</returns>
        public String getDownloadFilename()
        { 
            return this.downloadFilename;
        }

        /// <summary>
        ///     Returns the full path of a downloaded file.
        /// </summary>
        /// <returns>The full path of a downloaded file.</returns>
        public String getDownloadFilePath()
        { 
            return this.downloadFilePath;
        }

        public Boolean getErrorStatus()
        {
            return this.error;
        }

        #endregion

        #region Internal functions for fetching the parameters in a certain way

        /// <summary>
        ///     Creates a multipart HTTP header
        /// </summary>
        /// <returns>a StringBuilder object with the header data</returns>
        private StringBuilder getNameValuePairsForMultipart()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String[] pair in this.nameValuePairs)
            {
                sb.Append("--" + boundary);
                sb.Append(Environment.NewLine);
                sb.Append("Content-Disposition: form-data; name=\"" + pair[0] + "\"");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(pair[1]);
                sb.Append(Environment.NewLine);
            }
            return sb;
        }

        /// <summary>
        ///     Creates a HTTP post-header of which the values are encoded with URL encoding
        /// </summary>
        /// <returns>a StringBuilder object with the header data</returns>
        private StringBuilder getNameValuePairsForUrlEncoded()
        {
            StringBuilder sb = new StringBuilder();
            foreach (String[] pair in this.nameValuePairs)
            {
                if (sb.Length > 0) sb.Append("&");
                sb.Append(pair[0] + "=");
                sb.Append(System.Web.HttpUtility.UrlEncode(this.encoding.GetBytes(pair[1])));
            }
            return sb;
        }
        
        #endregion

        #region Public methodes for sending requests

        /// <summary>
        ///     This is a normal URL request to fetch a web page.
        /// </summary>
        public void sendPOSTrequestSimple()
        {
            try
            {
                this.error=false;
                this.createPostRequest();
                this.getResponse();
                this.saveNewCookie();
                this.saveResponseFromServer();
                this.clearNameValuePairs();
            }
            catch(Exception e)
            {
                this.error = true;
                DomainController.Instance().processError(e,false);
            }
        }

        /// <summary>
        ///     This request gets data from a form. This is normally used for uploading text data from a form.
        /// </summary>
        public void sendPOSTrequestFromForm()
        {
            try
            {
                this.error=false;
                this.createPostRequest();
                this.setPostDataForURLEncoded();
                this.sendDataStream();
                this.getResponse();
                this.saveNewCookie();
                this.saveResponseFromServer();
                this.clearNameValuePairs();
            }
            catch(Exception e)
            {
                this.error = true;
                DomainController.Instance().processError(e,false);
            }
        }

        /// <summary>
        ///     This request uploads a file. The file has to be set in advance with setUploadFile.
        /// </summary>
        public void sendPOSTrequestFromFormForUpload()
        {
            try
            {
                this.error=false;
                if (this.uploadFilename.Equals(String.Empty) || this.uploadFilePath.Equals(String.Empty))
                    throw new Exception("Programmer logic error - file not set before upload!");

                byte[] endBoundary = this.encoding.GetBytes(Environment.NewLine + "--" + this.boundary + "--" + Environment.NewLine);
                this.createPostRequest();
                FileStream uploadDocument = new FileStream(this.uploadFilePath, FileMode.Open, FileAccess.Read);
                this.setPostDataForFileUpload(uploadDocument, endBoundary);
                this.sendDataStream(uploadDocument, endBoundary);
                uploadDocument.Close();
                this.getResponse();
                this.saveNewCookie();
                this.saveResponseFromServer();
                this.clearNameValuePairs();
            }
            catch(Exception e)
            {
                this.error = true;
                DomainController.Instance().processError(e,false);
            }
        }

        /// <summary>
        ///     This request uploads a file, and gets the filename, the local path and the MIME type as parameters.
        /// </summary>
        /// <param name="inputName">The input name of the file browser.</param>
        /// <param name="filename">Filename the uploaded file will get on the server</param>
        /// <param name="fullFilePath">Path and filename of the local file</param>
        /// <param name="mimetype">MIME Type</param>
        public void sendPOSTrequestFromFormForUpload(String inputName, String filename, String fullFilePath, String mimetype)
        {
            this.error=false;
            this.setUploadFile(inputName, filename, fullFilePath, mimetype);
            this.sendPOSTrequestFromFormForUpload();
        }
        
        /// <summary>
        ///     This request uploads a file, and gets the filename and the local path as parameters.
        /// </summary>
        /// <param name="inputName">The input name of the file browser.</param>
        /// <param name="filename">Naam van het bestand voor de server</param>
        /// <param name="fullFilePath">Pad en naam van het lokale bestand</param>
        public void sendPOSTrequestFromFormForUpload(String inputName, String filename, String fullFilePath)
        {
            this.error=false;
            this.setUploadFile(inputName, filename, fullFilePath, String.Empty);
            this.sendPOSTrequestFromFormForUpload();
        }

        /// <summary>
        ///     This request is meant for downloading a file.
        ///     If the download fails, downloadFilePath and downloadFilename will both be null.
        /// </summary>
        /// <param name="savePath">Local directory in which the file should be saved.</param>
        public void sendPOSTrequestFromFormForDownload(String savePath)
        {
            try
            {
                this.error=false;
                this.createPostRequest();
                this.setPostDataForURLEncoded();
                this.sendDataStream();
                this.getResponse();
                this.saveNewCookie();
                this.saveFileResponseFromServer(savePath);
                this.clearNameValuePairs();
            }
            catch(Exception e)
            {
                this.error = true;
                DomainController.Instance().processError(e,false);
            }
        }

        #endregion 

        #region Internal methods used by the public methods

        /// <summary>
        ///     Creates the httpWebRequest object, and sets the method to POST
        /// </summary>
        /// <remarks>
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        ///      -> Added a custom user-agent header.
        /// </remarks>
        public void createPostRequest()
        {
            this.httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(this.requestUrl);
            this.httpWebRequest.CookieContainer = this.cookieJar;
            // fix for the error "The server committed a protocol violation. Section=ResponseStatusLine"
            this.httpWebRequest.KeepAlive = false;
            // Set the Method property of the request to POST.
            this.httpWebRequest.Method = "POST";
            //As of 1.09
            if (userAgent!=null)
                this.httpWebRequest.UserAgent = userAgent;
        }

        /// <summary>
        ///     Creates the postData, and sets the ContentType and ContentLength of the web request stream.
        ///     This method specifically sets the data for a request with URL encoding.
        /// </summary>
        private void setPostDataForURLEncoded()
        {
            this.postData = this.encoding.GetBytes(this.getNameValuePairsForUrlEncoded().ToString());
            this.httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            this.httpWebRequest.ContentLength = this.postData.Length;
        }

        /// <summary>
        ///     Creates the postData, and sets the ContentType and ContentLength of the web request stream.
        ///     This method specifically sets the data for an upload
        /// </summary>
        /// <param name="uploadDocument">The filestream of the document to upload, to know the length</param>
        /// <param name="endBoundary">The boundary to put behind the uploaded document, to know the length</param>
        private void setPostDataForFileUpload(FileStream uploadDocument, byte[] endBoundary)
        {
            this.httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            StringBuilder mpPostData = getNameValuePairsForMultipart();
            mpPostData.Append("--" + boundary);
            mpPostData.Append(Environment.NewLine);
            mpPostData.Append("Content-Disposition: form-data; name=\"" + this.inputName + "\"; filename=\"" + uploadFilename + "\"");
            if (!uploadMIMEtype.Equals(String.Empty))
            {
                mpPostData.Append(Environment.NewLine);
                mpPostData.Append("Content-Type: " + uploadMIMEtype);
            }
            mpPostData.Append(Environment.NewLine);
            mpPostData.Append("Content-Transfer-Encoding: binary");
            mpPostData.Append(Environment.NewLine);
            mpPostData.Append(Environment.NewLine);
            this.postData = this.encoding.GetBytes(mpPostData.ToString());
            this.httpWebRequest.ContentLength = this.postData.Length + uploadDocument.Length
                                         + endBoundary.Length;
        }


        /// <summary>
        ///     Overload for a normal sendDataStream without upload
        /// </summary>
        private void sendDataStream() 
        { 
            this.sendDataStream(null,null);
        }

        /// <summary>
        ///     Sends data through the data stream of the HttpRequest. If the uploadDocument is Null, it is a normal postdata send.
        /// </summary>
        /// <param name="uploadDocument">FileStream of an opened document.</param>
        /// <param name="endBoundary">The pre-made boundary die that needs to be put at the end of the file data .</param>
        private void sendDataStream(FileStream uploadDocument, byte[] endBoundary)
        {
            long bytesProcessed = 0;

            // Get the request stream.
            Stream dataStream = this.httpWebRequest.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(postData, 0, postData.Length);
            if (uploadDocument != null)
            {
                byte[] buffer = new byte[1024];
                int bytesread = 0;
                bytesread = uploadDocument.Read(buffer, 0, buffer.Length);
                bytesProcessed += bytesread;
                while (bytesread > 0)
                {
                    dataStream.Write(buffer, 0, bytesread);
                    bytesread = uploadDocument.Read(buffer, 0, buffer.Length);
                    bytesProcessed += bytesread;
                    DomainController.Instance().fireProgressBarUpdate(bytesProcessed);
                }
                dataStream.Write(endBoundary, 0, endBoundary.Length);
            }

            // Close the Stream object.
            dataStream.Close();
        }

        /// <summary>
        ///     Sends the request, saves the response in httpWebResponse and stores the response URL.
        /// </summary>
        private void getResponse()
        {
            try
            {
                // SEND the request en Get the response.
                this.httpWebResponse = (HttpWebResponse) this.httpWebRequest.GetResponse();
                this.responseUrl = this.httpWebResponse.ResponseUri.AbsoluteUri;
                // Eventueel nieuwe cookies bewaren.
            }
            catch (Exception e)
            {
                Debug.WriteLine("fout" + httpWebResponse.ResponseUri.AbsoluteUri);
            }
        }

        /// <summary>
        ///     Fetches the new cookies and saves them in the cookie jar.
        /// </summary>
        private void saveNewCookie()
        {
            try
            {
                //this.cookieJar = new CookieContainer();
                foreach(Cookie c in this.httpWebResponse.Cookies)
                    this.cookieJar.Add(c);
            }
            catch
            {
                // no new cookies
            }
        }
            
        /// <summary>
        ///     Saves the response from the server in a String. This method is only for web pages, and should not be used for download responses.
        /// </summary>
        private void saveResponseFromServer()
        {
            this.responseFromServer = new StreamReader(this.httpWebResponse.GetResponseStream(), this.encoding).ReadToEnd();
        }

        /// <summary>
        ///     Saves the server response of a file request to a file.
        /// </summary>
        /// <param name="savePath">Path in which the file from the response stream will be saved</param>
        private void saveFileResponseFromServer(String savePath)
        {
            Stream responseStream = this.httpWebResponse.GetResponseStream();
            this.downloadFilePath = null;
            this.downloadFilename = getFilenameFromHeader();
            if (this.downloadFilename != null)
            {
                this.downloadFilePath = savePath.Trim('\\') + "\\" + this.downloadFilename;
                FileStream fs = new FileStream(this.downloadFilePath, FileMode.CreateNew);
                byte[] buffer = new byte[8096];
                int count = -1;
                while (count != 0)
                {
                    count = responseStream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, count);
                }
                fs.Close();
            }
            responseStream.Close();
        }

        /// <summary>
        ///     Filters the file name of a downloaded file from the HTTP response header
        /// </summary>
        /// <returns>Name of the file that was sent back</returns>
        private String getFilenameFromHeader()
        {
            try
            {
                String header=httpWebResponse.Headers["Content-disposition"];
                Match matcher = Regex.Match(header, "filename=\"(.*)\"");
                if (matcher.Success)
                    return matcher.Groups[1].Value;
                else
                { 
                    // for header without quotes around the filename, and with possible leading spaces
                    matcher = Regex.Match(header, "filename=(.*)");
                    if (matcher.Success)
                        return matcher.Groups[1].Value.Trim();
                    else
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

    }
}
