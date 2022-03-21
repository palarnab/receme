using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;


namespace DeltaSyncProject
{
    class WLiveMail
    {

        CookieCollection _passportCookies = new CookieCollection();
        CookieCollection DSCookies = new CookieCollection();
        enum XMLDeltaType { Login, Settings, MailSettings, Download }
        string _urlWLmail = "";
        string _pptokenimage = "";

        public string GetTicket(string UserName, string Password)
        {
            string xmlRequest = GetXMLLogin(UserName, Password);
            string url = "https://login.live.com/RST2.srf";
            string loginticket = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            byte[] requestBytes = System.Text.Encoding.ASCII.GetBytes(xmlRequest);
            req.Method = "POST";
            req.CookieContainer = new CookieContainer();
            req.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; MyIE2; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 1.1.4322; IDCRL 4.200.513.1; IDCRL-cfg 6.0.11409.0; App wlmail.exe, 12.0.1606.1023, {47A6D4CF-5EB0-4B0E-9138-1B3F2DD40981})");
            req.Headers.Add("ContentLength", requestBytes.Length.ToString());
            req.Headers.Add("Cache-Control", "no-cache");
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(requestBytes, 0, requestBytes.Length);
            requestStream.Close();
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default);
            string result = sr.ReadToEnd();
            sr.Close();
            res.Close();
            res = null;
            sr = null;
            req = null;

            if (result != null && result.IndexOf(@"Id=""Compact1"">") > -1)
            {
                loginticket = result.Substring(result.IndexOf(@"Id=""Compact1"">") + 14);
                loginticket = loginticket.Substring(0, loginticket.IndexOf("</wsse"));
                loginticket = loginticket.Replace("+", "%2b");
                loginticket = loginticket.Replace("&amp;", "%26");
            }

            if (result != null && result.IndexOf(@"Id=""Compact6"">") > -1)
            {
                _pptokenimage = result.Substring(result.IndexOf(@"Id=""Compact6"">") + 14);
                _pptokenimage = _pptokenimage.Substring(0, _pptokenimage.IndexOf("</wsse"));
                _pptokenimage = _pptokenimage.Replace("+", "%2b");
                _pptokenimage = _pptokenimage.Replace("&amp;", "%26");
            }

            return loginticket;
        }

        private string GetXMLLogin(string username, string password)
        {
            string result = "";
            string xmlname = "DeltaSyncProject.MailsXML.XMLLogin.xml";
            Stream streamXml = base.GetType().Assembly.GetManifestResourceStream(xmlname);
            System.Xml.XmlDataDocument _xmldoc = new System.Xml.XmlDataDocument();
            _xmldoc.Load(streamXml);
            _xmldoc["s:Envelope"]["s:Header"]["wsse:Security"]["wsse:UsernameToken"]["wsse:Username"].InnerText = username;
            _xmldoc["s:Envelope"]["s:Header"]["wsse:Security"]["wsse:UsernameToken"]["wsse:Password"].InnerText = password;
            DateTime _c_and_e = DateTime.Now.ToUniversalTime();
            string _created = _c_and_e.ToString("yyyy-MM-dd") + "T" + _c_and_e.ToString("HH:mm:ss") + "Z";
            string _expired = _c_and_e.ToString("yyyy-MM-dd") + "T" + _c_and_e.ToString("HH:") + (_c_and_e.Minute + 5).ToString().PadLeft(2, Convert.ToChar("0")) + _c_and_e.ToString(":ss") + "Z";
            _xmldoc["s:Envelope"]["s:Header"]["wsse:Security"]["wsu:Timestamp"]["wsu:Created"].InnerText = _created;
            _xmldoc["s:Envelope"]["s:Header"]["wsse:Security"]["wsu:Timestamp"]["wsu:Expires"].InnerText = _expired;
            result = _xmldoc.InnerXml;
            return result;
        }

        public string GetSettings(string ticket)
        {
            string string1;
            string string2;
            string string3;
            byte[] byteArray1;
            HttpWebRequest httpWebRequest1;
            HttpWebResponse httpWebResponse1;
            StreamReader streamReader1;
            Stream stream1;
            Exception exception1;
            string1 = string.Empty;
            string2 = GetXmlSettings();
            string3 = string.Concat("http://mail.services.live.com/DeltaSync_v2.0.0/Settings.aspx?", ticket);
            byteArray1 = Encoding.UTF8.GetBytes(string2);
            httpWebRequest1 = ((HttpWebRequest)null);
            httpWebResponse1 = ((HttpWebResponse)null);
            streamReader1 = ((StreamReader)null);
        DoAgain:
            httpWebRequest1 = ((HttpWebRequest)WebRequest.Create(string3));
            httpWebRequest1.Method = "POST";
            httpWebRequest1.Accept = "text/*";
            httpWebRequest1.ContentType = "text/xml";
            httpWebRequest1.UserAgent = "WindowsLiveMail/1.0";
            httpWebRequest1.CookieContainer = new CookieContainer();
            httpWebRequest1.AllowAutoRedirect = true;
            httpWebRequest1.KeepAlive = true;
            httpWebRequest1.Headers.Add("Cache-Control", "no-cache");
            httpWebRequest1.ContentLength = ((long)byteArray1.Length);
            stream1 = httpWebRequest1.GetRequestStream();
            stream1.Write(byteArray1, 0, byteArray1.Length);
            stream1.Close();
            try
            {
                httpWebResponse1 = ((HttpWebResponse)httpWebRequest1.GetResponse());
                streamReader1 = new StreamReader(httpWebResponse1.GetResponseStream(), Encoding.Default);
                string1 = streamReader1.ReadToEnd();
                this.DSCookies = httpWebResponse1.Cookies;
                string3 = httpWebRequest1.Address.AbsoluteUri + "?" + ticket;
                _urlWLmail = httpWebRequest1.Address.AbsoluteUri.Replace("Settings", "Sync") + "?" + ticket;
            }
            catch (Exception exception2)
            {
                exception1 = exception2;
                if (exception1.ToString().IndexOf("(405)") == -1)
                    string1 = string.Empty;
                else
                {
                    string3 = string.Concat(httpWebRequest1.Address.AbsoluteUri, "?", ticket);
                    _urlWLmail = httpWebRequest1.Address.AbsoluteUri.Replace("Settings", "Sync") + "?" + ticket;
                    goto DoAgain;
                }
            }
            if (streamReader1 != null)
                streamReader1.Close();
            if (httpWebResponse1 != null)
                httpWebResponse1.Close();
            httpWebResponse1 = ((HttpWebResponse)null);
            streamReader1 = ((StreamReader)null);
            httpWebRequest1 = ((HttpWebRequest)null);
            return string1;
        }

        private string GetXmlSettings()
        {
            string _xmldeltaName = "DeltaSyncProject.MailsXML.XMLLiveSettings.xml";
            System.IO.Stream stream = null;
            stream = this.GetType().Assembly.GetManifestResourceStream(_xmldeltaName);
            System.Xml.XmlDocument _xmldoc = new System.Xml.XmlDocument();
            _xmldoc.Load(stream);

            return _xmldoc.InnerXml;

        }

        public string GetMail()
        {
            string _xmlMailSettings = "";
            string xmlRequest = GetXmlMail();
            byte[] requestBytes = null;
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            StreamReader sr = null;

            do
            {
                req = (HttpWebRequest)WebRequest.Create(_urlWLmail);
                requestBytes = System.Text.Encoding.UTF8.GetBytes(xmlRequest);
                req.Method = "POST";
                req.Accept = "text/*";
                req.ContentType = "text/xml";
                req.UserAgent = "WindowsLiveMail/1.0";
                req.CookieContainer = new CookieContainer();
                req.CookieContainer.Add(_passportCookies);
                req.AllowAutoRedirect = true;
                req.KeepAlive = true;
                req.Headers.Add("Cache-Control", "no-cache");
                req.ContentLength = requestBytes.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
                try
                {
                    res = (HttpWebResponse)req.GetResponse();
                    sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.Default);
                    if (_xmlMailSettings == "")
                    {
                        xmlRequest = sr.ReadToEnd();
                        xmlRequest = XMLRequestMailFormat(xmlRequest);
                        _xmlMailSettings = xmlRequest;
                        _passportCookies.Add(res.Cookies);
                    }
                    else
                    {
                        _passportCookies.Add(res.Cookies);
                        _xmlMailSettings = sr.ReadToEnd();
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            } while (true);

            sr.Close();
            res.Close();
            res = null;
            sr = null;
            req = null;
            return _xmlMailSettings;
        }

        private string GetXmlMail()
        {
            string _xmldeltaName = "DeltaSyncProject.MailsXML.XMLLiveEmailReq1.xml";
            System.IO.Stream stream = null;
            stream = this.GetType().Assembly.GetManifestResourceStream(_xmldeltaName);
            System.Xml.XmlDocument _xmldoc = new System.Xml.XmlDocument();
            _xmldoc.Load(stream);

            return _xmldoc.InnerXml;
        }

        private string XMLRequestMailFormat(string xmlRequest)
        {
            string result = xmlRequest.Replace("Status", "GetChanges");
            XmlDocument _xmldoc = new XmlDocument();
            _xmldoc.LoadXml(result);
            _xmldoc.ChildNodes[1].RemoveChild(_xmldoc.ChildNodes[1].ChildNodes[0]);
            _xmldoc.ChildNodes[1].ChildNodes[0].ChildNodes[0].ChildNodes[2].InnerText = "";
            _xmldoc.ChildNodes[1].ChildNodes[0].ChildNodes[1].ChildNodes[2].InnerText = "";
            result = _xmldoc.InnerXml;
            _xmldoc = null;
            return result;
        }

    }
}

