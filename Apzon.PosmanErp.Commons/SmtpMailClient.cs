
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Apzon.PosmanErp.Commons
{
    public class SmtpMailClient
    {
        //private string _from;
        //private string _pass;
        private SmtpClient _oSmtp;
        private string _emailFrom, _error, _fromName;

        public SmtpMailClient()
        {
            //Task.Run(() => DownloadTokenFile()).Wait();
            //DownloadTokenFile();
        }

        private async Task<bool> DownloadTokenFile(DataTable dtConfig, string appFolder)
        {
            try
            {
                if (dtConfig.IsNotNull())
                {
                    var r = dtConfig.Rows[0];
                    var method = Function.ToString(r["SmtpMethod"]);
                    if (method == "T")
                    {
                        var googleTokenFolder = appFolder + @"\Analytics.Auth.Store\";
                        if (!Directory.Exists(googleTokenFolder))
                        {
                            Directory.CreateDirectory(googleTokenFolder);
                        }
                        if (!Directory.GetFiles(googleTokenFolder).Any())
                        {
                            Logging.Write(Logging.ERROR, "Không tồn tại dữ liệu Google token", null);
                            return false;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR, "", ex);
                return false;
            }
            return true;
        }

        private async Task<string> TryConnect(DataTable dtConfig, string appFolder)
        {
            try
            {
                string smtpAddress = "", emailFrom = "", fullName = "", method = "", clientId = "", clientSecret = "", userName = "", password = "", mailAddress = "";
                int portNumber = 0, timeout = 0;
                bool enableSsl = false, useAuthen = false;
                #region Get Mail SMTP Setting
                if (dtConfig.IsNotNull())
                {
                    var r = dtConfig.Rows[0];
                    method = Function.ToString(r["SmtpMethod"]);
                    clientId = Function.ToString(r["ClientId"]);
                    clientSecret = Function.ToString(r["ClientSecret"]);
                    userName = Function.ToString(r["UserName"]);
                    timeout = Function.ParseInt(r["Timeout"]);

                    smtpAddress = Function.ToString(r["ServerAddress"]);
                    portNumber = Function.ParseInt(r["SSLPort"]);
                    fullName = Function.ToString(r["FullName"]);
                    enableSsl = Function.ToString(r["EnableSSL"]) == "Y";
                    mailAddress = Function.ToString(r["MailAddress"]);
                    useAuthen = Function.ToString(r["UseAuthen"]) == "Y";
                    
                    switch (method)
                    {
                        case "N":
                            emailFrom = userName;
                            if (!string.IsNullOrEmpty(Function.ToString(r["Password"])))
                            {
                                password = Function.Decrypt(Function.ToString(r["Password"]));
                            }
                            if (useAuthen)
                            {
                                if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName))
                                {
                                    _error = "You must setup Smtp Credentials!";
                                }
                            }
                            break;
                        case "T":
                            //Kiểm tra trường Mail Address có bị rỗng hay ko
                            //Nếu rỗng thì sử dụng luôn mail của người đang đăng nhập
                            if (!string.IsNullOrEmpty(mailAddress))
                            {
                                emailFrom = mailAddress;
                            }
                            else
                            {
                                _error = "Sender's email is missing!";
                            }

                            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                            {
                                _error = "ClientId or ClientSecret is missing!";
                            }
                            break;
                        case "S":

                            if (!string.IsNullOrEmpty(mailAddress))
                            {
                                emailFrom = mailAddress;
                            }
                            else
                            {
                                _error = "Sender's email is missing!";
                            }

                            if (!string.IsNullOrEmpty(Function.ToString(r["Password"])))
                            {
                                password = Function.Decrypt(Function.ToString(r["Password"]));
                            }
                            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName))
                            {
                                _error = "You must setup Smtp Credentials!";
                            }

                            break;
                    }

                    if (!string.IsNullOrEmpty(fullName))
                    {
                        _fromName = fullName;
                    }
                    else
                    {
                        _error = "You must setup Mail From's Display Name!";
                    }

                    _emailFrom = emailFrom;
                }
                else
                {
                    //XtraMessageBoxEx.Show();
                    _error = "You must setup Mail Smtp Setting first!";
                }
                #endregion
                if (!string.IsNullOrWhiteSpace(_error))
                {
                    return _error;
                }
                _oSmtp = new SmtpClient()
                {
                    Timeout = timeout * 1000,
                    //SslProtocols = SslProtocols.Ssl2,
                    ServerCertificateValidationCallback = (s, c, h, e) => true,

                };

                await _oSmtp.ConnectAsync(smtpAddress, portNumber, enableSsl);

                SaslMechanism mechan = null;
                switch (method)
                {
                    case "N":
                        if (useAuthen)
                        {
                            mechan = new SaslMechanismPlain(new NetworkCredential(userName, password));
                            await _oSmtp.AuthenticateAsync(mechan);
                        }
                        break;
                    case "S":
                        mechan = new SaslMechanismPlain(new NetworkCredential(userName, password));
                        await _oSmtp.AuthenticateAsync(mechan);
                        break;
                    case "T":
                        //string _client_id = "821387592681-dgipou81btdqc0g90ec95580q8h29k6c.apps.googleusercontent.com", _client_secret = "4DR5h3SzrqllXfynDPDBXDK9";
                        var rs = await DownloadTokenFile(dtConfig, appFolder);
                        if (rs)
                        {
                            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                                new ClientSecrets
                                {
                                    ClientId = clientId,
                                    ClientSecret = clientSecret
                                },
                                new[]
                                {
                                    "https://mail.google.com/",
                                },
                                "Apzon",
                                CancellationToken.None,
                                new FileDataStore(appFolder + @"\Analytics.Auth.Store\"));

                            var refreshToken = await credential.RefreshTokenAsync(CancellationToken.None);
                            if (!refreshToken)
                            {
                                Logging.Write(Logging.TRACE, "Refresh Token FAILED!", null);
                            }
                            mechan = new SaslMechanismOAuth2(mailAddress, credential.Token.AccessToken);
                            await _oSmtp.AuthenticateAsync(mechan);
                        }
                        else
                        {
                            _error = "Can not download Token file! Please check log for more detail";
                        }

                        break;
                }
                // Note: only needed if the SMTP server requires authentication

                return _error;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listTo">List of To Address </param>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Body</param>
        /// <param name="listAttachFile">List of Email Attach file's path</param>
        /// <param name="fromName">Name of From Address</param>
        /// <param name="cc">List of Cc Address</param>
        /// <param name="bcc">List of Bcc Address</param>
        /// <returns></returns>
        public async Task<string> SendEmail(DataTable dtConfig, string appFolder, List<string> listTo, string subject, string body, string fromName = null, List<string> cc = null, List<string> bcc = null, List<string> listAttachFile = null)
        {
            try
            {
                var rs = await TryConnect(dtConfig, appFolder);
                if (!string.IsNullOrEmpty(rs))
                {
                    return rs;
                }
                if (listTo.Count <= 0)
                {
                    return "No received email found!";
                }

                MimeMessage oMail = new MimeMessage();

                // Your gmail email address
                if (fromName != null)
                    oMail.From.Add(new MailboxAddress(fromName, _emailFrom));
                else
                    oMail.From.Add(new MailboxAddress(_fromName, _emailFrom));


                foreach (var emailTo in listTo.Where(t => !string.IsNullOrEmpty(t)))
                {
                    oMail.To.Add(new MailboxAddress(emailTo));
                }
                if (cc != null)
                    foreach (var emailTo in cc.Where(t => !string.IsNullOrEmpty(t)))
                    {
                        oMail.Cc.Add(new MailboxAddress(emailTo));
                    }
                if (bcc != null)
                    foreach (var emailTo in bcc.Where(t => !string.IsNullOrEmpty(t)))
                    {
                        oMail.Bcc.Add(new MailboxAddress(emailTo));
                    }

                
                // Set email subject
                oMail.Subject = subject;

                var builder = new BodyBuilder();

                // Set the plain-text version of the message text
                builder.HtmlBody = body;

                if (listAttachFile != null && listAttachFile.Count > 0)
                {
                    foreach (string filePath in listAttachFile)
                    {
                        //var type = new MimeKit.ContentType(MediaTypeNames.Application.Octet,"");
                        builder.Attachments.Add(filePath);
                        // builder.Attachments.Add(filePath,Stream.Null,type);
                    }
                }
                oMail.Body = builder.ToMessageBody();
                #region Send mail

                try
                {
                    await _oSmtp.SendAsync(oMail);
                    await _oSmtp.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Logging.Write(Logging.ERROR,
                        new StackTrace(new StackFrame(0)).ToString()
                            .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                    return ex.Message;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logging.Write(Logging.ERROR,
                    new StackTrace(new StackFrame(0)).ToString()
                        .Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex);
                return ex.Message;
            }
            return
                string.Empty;
        }
    }
}
