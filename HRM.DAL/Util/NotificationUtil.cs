using HRM.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Util
{
    public class NotificationUtil
    {
        readonly static oneportalEntities _context = new oneportalEntities();

        public static void SendEmail(Email email)
        {
            email.from = _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value;//"workspace@naiis.ng";
            
            string SmtpServerUrl = _context.ws_app_settings.FirstOrDefault(e => e.key == "SmtpServerUrl").value; 
            int port = Convert.ToInt32(_context.ws_app_settings.FirstOrDefault(e => e.key == "port").value);

            MailMessage mail = new MailMessage();
            var SmtpServer = new SmtpClient(SmtpServerUrl)
            {
                Port = port
            };
            mail.From = new MailAddress(email.from);
            //if (System.Configuration.ConfigurationManager.AppSettings["sendmail"] == "false")
            //{
            //    mail.To.Add("emadubuko@mgic.umaryland.edu");
            //}
            //else
            {
                foreach (var email_address in email.to)
                {
                    mail.To.Add(email_address);
                }
            }
            
            
            mail.Subject = email.subject;
            mail.Body = email.body;
            if (email.attachments != null && email.attachments.Any())
            {
                foreach(var atchmt in email.attachments)
                {
                    Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(atchmt);
                    mail.Attachments.Add(attachment);
                }
               
            }
            mail.IsBodyHtml = email.IsHtml;
           

            SmtpServer.Credentials = new System.Net.NetworkCredential(_context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value, _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpPassword").value);
            string sslEnabled = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_ssl_enabled").value;
            SmtpServer.EnableSsl = Convert.ToBoolean(sslEnabled);

            string smtpTimeOut = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_timeout").value;
            SmtpServer.Timeout = Convert.ToInt32(smtpTimeOut);
           
            try
            {
                SmtpServer.Send(mail);
                Utils.LogInfo("Email Sent", "Email has been sent at " + DateTime.Now.ToString("F") + " To: " + mail.To);
            }
            catch (SmtpException ex)
            {
                Utils.LogError(ex);
                Utils.Log(ex.Message);
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                Utils.Log(e.Message);
            }

        }

        public static void SendNotifications(int[] employees,string message,string link=null)
        {
            try
            {
                foreach (var emp in employees)
                {
                    _context.ws_notification.Add(new ws_notification { id = Guid.NewGuid(), emp_number = emp, message = message, viewed = 0, created_at = DateTime.Now, link = link });
                }
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }
           
        }

        public static IQueryable<ws_notification> GetEmployeeNotifications(int emp_number)
        {
            try
            {
                return _context.ws_notification.Where(e => e.emp_number == emp_number);
            }
            catch (Exception e)
            {
                Utils.LogError(e);
                return new List<ws_notification>().AsQueryable();
            }
           // return _context.ws_notification.Where(e => e.emp_number == emp_number);
        }

        public static void SendEmailTest(Email email)
        {
            email.from = _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value;
            //;"workspace@mgic-nigeria.org";

            string SmtpServerUrl = _context.ws_app_settings.FirstOrDefault(e => e.key == "SmtpServerUrl").value;
            int port = Convert.ToInt32(_context.ws_app_settings.FirstOrDefault(e => e.key == "port").value);

            using (var mail = new MailMessage())
            {
                using (var smtpServer = new SmtpClient(SmtpServerUrl))
                {
                    smtpServer.Port = port;
                    mail.From = new MailAddress(email.from);

                    {
                        foreach (var email_address in email.to)
                        {
                            mail.To.Add(email_address);
                        }
                    }

                    mail.Subject = email.subject;
                    mail.Body = email.body;
                    if (email.attachments != null && email.attachments.Any())
                    {
                        foreach (var atchmt in email.attachments)
                        {
                            Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(atchmt);
                            mail.Attachments.Add(attachment);
                        }

                    }
                    mail.IsBodyHtml = email.IsHtml;


                    smtpServer.Credentials = new System.Net.NetworkCredential(_context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value, _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpPassword").value);
                    string sslEnabled = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_ssl_enabled").value;
                    smtpServer.EnableSsl = true;// Convert.ToBoolean(sslEnabled);

                    string smtpTimeOut = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_timeout").value;
                    smtpServer.Timeout = Convert.ToInt32(smtpTimeOut);

                    try
                    {
                        smtpServer.Send(mail);
                        Utils.LogInfo("Email Sent", "Email has been sent at " + DateTime.Now.ToString("F") + " To: " + mail.To);
                    }
                    catch (SmtpException ex)
                    {
                        Utils.LogError(ex);
                        Utils.Log(ex.Message);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError(e);
                        Utils.Log(e.Message);
                    }
                }
            }
        }

        public static void SendEmailComplain(Email email, string complianantEmail)
        {
            email.from = _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value; //"workspace@mgic-nigeria.org";

            string SmtpServerUrl = _context.ws_app_settings.FirstOrDefault(e => e.key == "SmtpServerUrl").value;
            int port = Convert.ToInt32(_context.ws_app_settings.FirstOrDefault(e => e.key == "port").value);

            using (var mail = new MailMessage())
            {
                using (var smtpServer = new SmtpClient(SmtpServerUrl))
                {
                    smtpServer.Port = port;
                    mail.From = new MailAddress(email.from);

                    {
                        foreach (var email_address in email.to)
                        {
                            mail.To.Add(email_address);
                        }
                    }
                    mail.CC.Add(complianantEmail);

                    mail.Subject = email.subject;
                    mail.Body = email.body;
                    if (email.attachments != null && email.attachments.Any())
                    {
                        foreach (var atchmt in email.attachments)
                        {
                            Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(atchmt);
                            mail.Attachments.Add(attachment);
                        }

                    }
                    mail.IsBodyHtml = email.IsHtml;


                    smtpServer.Credentials = new System.Net.NetworkCredential(_context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value, _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpPassword").value);
                    string sslEnabled = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_ssl_enabled").value;
                    smtpServer.EnableSsl = Convert.ToBoolean(sslEnabled);

                    string smtpTimeOut = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_timeout").value;
                    smtpServer.Timeout = Convert.ToInt32(smtpTimeOut);

                    try
                    {
                        smtpServer.Send(mail);
                        Utils.LogInfo("Email Sent", "Email has been sent at " + DateTime.Now.ToString("F"));
                    }
                    catch (SmtpException ex)
                    {
                        Utils.LogError(ex);
                        Utils.Log(ex.Message);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError(e);
                        Utils.Log(e.Message);
                    }
                }
            }
        }

        public static void NewSendEmail(Email email)
        {
            email.from = _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value;
            
            //;"workspace@mgic-nigeria.org";

            string SmtpServerUrl = _context.ws_app_settings.FirstOrDefault(e => e.key == "SmtpServerUrl").value;
            int port = Convert.ToInt32(_context.ws_app_settings.FirstOrDefault(e => e.key == "port").value);

            using (var mail = new MailMessage())
            {
                using (var smtpServer = new SmtpClient(SmtpServerUrl))
                {
                    smtpServer.Port = port;
                    mail.From = new MailAddress(email.from);

                    {
                        foreach (var email_address in email.to)
                        {
                            mail.To.Add(email_address);
                        }
                    }

                    mail.Subject = email.subject;
                    mail.Body = email.body;
                    if (email.attachments != null && email.attachments.Any())
                    {
                        foreach (var atchmt in email.attachments)
                        {
                            Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(atchmt);
                            mail.Attachments.Add(attachment);
                        }

                    }
                    mail.IsBodyHtml = email.IsHtml;


                    smtpServer.Credentials = new System.Net.NetworkCredential(_context.ws_app_settings.FirstOrDefault(e => e.key == "smtpLogin").value, _context.ws_app_settings.FirstOrDefault(e => e.key == "smtpPassword").value);
                    string sslEnabled = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_ssl_enabled").value;
                    smtpServer.EnableSsl = true;//Convert.ToBoolean(sslEnabled);

                    string smtpTimeOut = _context.ws_app_settings.FirstOrDefault(e => e.key == "email_timeout").value;
                    smtpServer.Timeout = Convert.ToInt32(smtpTimeOut);

                    try
                    {
                        smtpServer.Send(mail);
                        Utils.LogInfo("Email Sent", "Email has been sent at " + DateTime.Now.ToString("F") + " To: " + mail.To);
                    }
                    catch (SmtpException ex)
                    {
                        Utils.LogError(ex);
                        Utils.Log(ex.Message);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError(e);
                        Utils.Log(e.Message);
                    }
                }
            }
        }
    }


    public class Email
    {
        public List<string> to { set; get; }
        public string from { set; get; }
        public string body { set; get; }
        public string subject { set; get; }
        public List<string> attachments { set; get; }
        public Boolean IsHtml { set; get; }
    }
}
