//*Author: Trieu Nguyen
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlateReader
{
    public class EmailMessage
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            if (File.Exists(fileName))
            {

                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                }
            }
            return buffer;
        }
        ///// <summary>
        ///// assuming the attachment is a pdf file
        ///// </summary>
        ///// <param name="toEmail"></param>
        ///// <param name="fromEmail"></param>
        ///// <param name="subject"></param>
        ///// <param name="content"></param>
        ///// <param name="fileAttachmentPath"></param>
        ///// <param name="attachment"></param>
        ///// <param name="attachmentName">name with extensions</param>
        ///// <param name="fromEmailFullName"></param>
        ///// <param name="bccEmail"></param>
        ///// <returns></returns>
        //public static bool SendMsg(string toEmail, string fromEmail, string subject, string content, string fileAttachmentPath, string attachmentName = "", string fromEmailFullName = "", string bccEmail = "", string ccEmail = "", MemoryStream fileStream = null)
        //{
        //    try
        //    {
        //        byte[] attachment = null;
        //        if (File.Exists(fileAttachmentPath))
        //        {

        //            attachment = ReadAllBytes(fileAttachmentPath);
        //        }
        //        if (string.IsNullOrEmpty(attachmentName))
        //            attachmentName = "fileattachment.pdf";

        //        SmtpClient client = new SmtpClient();

        //        //GetConfigurations();
        //        ConfigSettings config = new ConfigSettings();

        //        string serveradr = config.Settings.SmtpServerAddress;
        //        client = new SmtpClient(serveradr);
        //        //If you need to authenticate
        //        //client.Credentials = new NetworkCredential("username", "password");
        //        MailMessage mailMessage = new MailMessage();
        //        mailMessage.IsBodyHtml = true;

        //        mailMessage.From = new MailAddress(fromEmail, fromEmailFullName);

        //        foreach (var to in toEmail.Split(','))
        //            mailMessage.To.Add(new MailAddress(to));
        //        if (!string.IsNullOrEmpty(ccEmail))
        //            foreach (var cc in ccEmail.Split(','))
        //                mailMessage.CC.Add(new MailAddress(cc));
        //        if (!string.IsNullOrEmpty(bccEmail))
        //            foreach (var bcc in bccEmail.Split(','))
        //                mailMessage.Bcc.Add(new MailAddress(bcc));

        //        //mailMessage.To.Add(toEmail);
        //        //if(!string.IsNullOrEmpty(bccEmail))
        //        //    mailMessage.Bcc.Add(bccEmail);
        //        //if (!string.IsNullOrEmpty(ccEmail))
        //        //    mailMessage.CC.Add(ccEmail);
        //        mailMessage.Subject = subject;
        //        mailMessage.Body = content;

        //        if (attachment != null)
        //            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), attachmentName, "application/pdf"));

        //        if (fileStream != null)
        //            mailMessage.Attachments.Add(new Attachment(fileStream, attachmentName, "image/tiff"));

        //        client.Send(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //    return true;
        //}


        ///// <summary>
        ///// assuming the attachment is multiple and is sending multiple pics as file attachment
        ///// </summary>
        ///// <param name="toEmail"></param>
        ///// <param name="fromEmail"></param>
        ///// <param name="subject"></param>
        ///// <param name="content"></param>
        ///// <param name="fileAttachmentPath"></param>
        ///// <param name="attachment"></param>
        ///// <param name="attachmentName"></param>
        ///// <param name="fromEmailFullName"></param>
        ///// <param name="bccEmail"></param>
        ///// <returns></returns>
        //public static bool SendMsgAttachedPictures(string toEmail, string fromEmail, string subject, string content, string fileAttachmentPath, string attachmentName = "", string fromEmailFullName = "", string bccEmail = "", string ccEmail = "", List<byte[]> fileStreamList = null)
        //{


        //    try
        //    {
        //        byte[] attachment = null;
        //        if (File.Exists(fileAttachmentPath))
        //        {

        //            attachment = ReadAllBytes(fileAttachmentPath);
        //        }
        //        if (string.IsNullOrEmpty(attachmentName))
        //            attachmentName = "fileattachment.jpg";

        //        SmtpClient client = new SmtpClient();

        //        //GetConfigurations();
        //        ConfigSettings config = new ConfigSettings();

        //        string serveradr = config.Settings.SmtpServerAddress;
        //        client = new SmtpClient(serveradr);
        //        //If you need to authenticate
        //        //client.Credentials = new NetworkCredential("username", "password");
        //        MailMessage mailMessage = new MailMessage();
        //        mailMessage.IsBodyHtml = true;
        //        mailMessage.From = new MailAddress(fromEmail, fromEmailFullName);

        //        foreach (var to in toEmail.Split(','))
        //            mailMessage.To.Add(new MailAddress(to));
        //        if (!string.IsNullOrEmpty(ccEmail))
        //            foreach (var cc in ccEmail.Split(','))
        //                mailMessage.CC.Add(new MailAddress(cc));
        //        if (!string.IsNullOrEmpty(bccEmail))
        //            foreach (var bcc in bccEmail.Split(','))
        //                mailMessage.Bcc.Add(new MailAddress(bcc));

        //        mailMessage.Subject = subject;
        //        mailMessage.Body = content;

        //        if (attachment != null)
        //            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), attachmentName, "image/jpeg"));

        //        if (fileStreamList.Count > 0)
        //        {
        //            for (int i = 0; i < fileStreamList.Count; i++)
        //            {
        //                mailMessage.Attachments.Add(new Attachment(new MemoryStream(fileStreamList[i]), attachmentName + i.ToString() + ".jpg", "image/jpeg"));
        //            }
        //        }
        //        client.Send(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
        public static async Task<bool> SendMsgWithInlinePicturesAsync(string toEmail, string fromEmail, string subject, string content, string attachmentName = "", string fromEmailFullName = "", string bccEmail = "", string ccEmail = "", List<byte[]> fileStreamList = null)
        {

            var success = await Task.Run(()=>SendMsgWithInlinePictures(toEmail, fromEmail, subject, content, attachmentName, fromEmailFullName, bccEmail, ccEmail, fileStreamList));

            return success;
        }
            /// <summary>
            /// assuming the attachment is multiple and is sending multiple pics
            /// </summary>
            /// <param name="toEmail"></param>
            /// <param name="fromEmail"></param>
            /// <param name="subject"></param>
            /// <param name="content"></param>
            /// <param name="fileAttachmentPath"></param>
            /// <param name="attachment"></param>
            /// <param name="attachmentName"></param>
            /// <param name="fromEmailFullName"></param>
            /// <param name="bccEmail"></param>
            /// <returns></returns>
            public static bool SendMsgWithInlinePictures(string toEmail, string fromEmail, string subject, string content, string attachmentName = "", string fromEmailFullName = "", string bccEmail = "", string ccEmail = "", List<byte[]> fileStreamList = null)
        {

            logger.Trace($"{toEmail},{fromEmail},{subject},{content}");

            string imgTag = "<img src =\"cid:{0}\"/>";
            //To embed image in email

            string imagesTag = string.Empty;
            try
            {
                MailMessage mailMessage = new MailMessage();
                if (string.IsNullOrEmpty(attachmentName))
                    attachmentName = "fileattachment.jpg";
                if (fileStreamList.Count > 0)
                {
                    for (int i = 0; i < fileStreamList.Count; i++)
                    {
                        Attachment img = new Attachment(new MemoryStream(fileStreamList[i]), attachmentName + i.ToString(), "image/jpeg");

                        img.ContentId = Guid.NewGuid().ToString();

                        //To make the image display as inline and not as attachment

                        img.ContentDisposition.Inline = true;
                        img.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                        mailMessage.Attachments.Add(img);

                        imagesTag += string.Format(imgTag, img.ContentId);

                    }
                }
                //assuming body content has "embeddedimage" key word
                if (!string.IsNullOrEmpty(imagesTag))
                    content = content.Replace("embeddedimage", imagesTag);

                var client = GetSmptClient();

                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(fromEmail, fromEmailFullName);

                foreach (var to in toEmail.Split(','))
                    mailMessage.To.Add(new MailAddress(to));
                if (!string.IsNullOrEmpty(ccEmail))
                    foreach (var cc in ccEmail.Split(','))
                        mailMessage.CC.Add(new MailAddress(cc));
                if (!string.IsNullOrEmpty(bccEmail))
                    foreach (var bcc in bccEmail.Split(','))
                        mailMessage.Bcc.Add(new MailAddress(bcc));

                mailMessage.Subject = subject;
                mailMessage.Body = content;


                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "SendMail Error");
                return false;
            }

            return true;
        }
        private static SmtpClient GetSmptClient()
        {
            var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["gmailUserName"], ConfigurationManager.AppSettings["gmailPwd"])
            };

            return client;
        }
        //public static bool SendMsgWithInlinePicturesWithAuthentication(string toEmail, string fromEmail, string subject, string content, string attachmentName = "", string fromEmailFullName = "", string bccEmail = "", string ccEmail = "", List<byte[]> fileStreamList = null)
        //{



        //    string imgTag = "<img src =\"cid:{0}\"/>";
        //    //To embed image in email

        //    string imagesTag = string.Empty;
        //    try
        //    {
        //        MailMessage mailMessage = new MailMessage();
        //        if (string.IsNullOrEmpty(attachmentName))
        //            attachmentName = "fileattachment.jpg";
        //        if (fileStreamList != null && fileStreamList.Count > 0)
        //        {
        //            for (int i = 0; i < fileStreamList.Count; i++)
        //            {
        //                Attachment img = new Attachment(new MemoryStream(fileStreamList[i]), attachmentName + i.ToString(), "image/jpeg");

        //                img.ContentId = Guid.NewGuid().ToString();

        //                //To make the image display as inline and not as attachment

        //                img.ContentDisposition.Inline = true;
        //                img.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
        //                mailMessage.Attachments.Add(img);

        //                imagesTag += string.Format(imgTag, img.ContentId);

        //            }
        //        }
        //        //assuming body content has "embeddedimage" key word
        //        if (!string.IsNullOrEmpty(imagesTag))
        //            content = content.Replace("embeddedimage", imagesTag);



        //        //SmtpClient client = new SmtpClient();


        //        //GetConfigurations();
        //        ConfigSettings config = new ConfigSettings();

        //        string serveradr = config.Settings.SmtpServerAddress;
        //        string user = config.Settings.SmtpUser;
        //        string pwd = config.Settings.SmtpPwd;
        //        SmtpClient client = new SmtpClient(serveradr);


        //        client.UseDefaultCredentials = false;



        //        //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
        //        //If you need to authenticate
        //        client.Credentials = new NetworkCredential(user, pwd);

        //        mailMessage.IsBodyHtml = true;
        //        mailMessage.From = new MailAddress(fromEmail, fromEmailFullName);

        //        foreach (var to in toEmail.Split(','))
        //            mailMessage.To.Add(new MailAddress(to));
        //        if (!string.IsNullOrEmpty(ccEmail))
        //            foreach (var cc in ccEmail.Split(','))
        //                mailMessage.CC.Add(new MailAddress(cc));
        //        if (!string.IsNullOrEmpty(bccEmail))
        //            foreach (var bcc in bccEmail.Split(','))
        //                mailMessage.Bcc.Add(new MailAddress(bcc));

        //        mailMessage.Subject = subject;
        //        mailMessage.Body = content;


        //        client.Send(mailMessage);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

    }
}
