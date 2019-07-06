﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using ICSI_UDIN.Models;
using System.Text;


namespace ICSI_UDIN.Repository
{
    public class UserRepository : IUserRepository
    {
        private ICSI_DBModleEntities DBcontext;
        public UserRepository(ICSI_DBModleEntities objusercontext)
        {
            this.DBcontext = objusercontext;
        }
        public void UpdateUser(tblUser User)
        {
            DBcontext.Entry(User).State = EntityState.Modified;
            DBcontext.SaveChanges();
        }
        public void DeleteUser(int UserId)
        {
            tblUser user = DBcontext.tblUsers.Find(UserId);

            DBcontext.tblUsers.Remove(user);

            DBcontext.SaveChanges();
        }
        public void InsertUser(tblUser User)
        {
            DBcontext.tblUsers.Add(User);

            DBcontext.SaveChanges();
        }
        public IEnumerable<tblUser> GetUsers()
        {
            return DBcontext.tblUsers.ToList();
        }
        public tblUser GetUserByID(int UserId)
        {
            return DBcontext.tblUsers.Find(UserId);
        }
        public tblUser GetUserByUserName(string UserName)
        {   
            return DBcontext.tblUsers.Where(x => x.UserName == UserName).FirstOrDefault();
        }

        public bool CheckLogin(tblUser objuser) //This method check the user existence
        {
            bool flag = false;
            tblUser loginresult = DBcontext.tblUsers.Where(s => s.UserName == objuser.UserName && s.Password == objuser.Password).FirstOrDefault();
            if (loginresult == null)
            {
                return flag;
            }
            else
            {
                flag = true;
            }
            return flag;

        }
        public void Save()
        {
            DBcontext.SaveChanges();
        }
        public RP_UDINVerification_Result GetUDINVerification(UDINVerification obj)
        {
            RP_UDINVerification_Result lst = DBcontext.RP_UDINVerification(obj.FName, obj.EmailId, obj.MobileNumber, obj.MembershipNumber.ToString()).ToList<RP_UDINVerification_Result>().FirstOrDefault();

            return lst;
        }

        public int InserttblUDINUser(tblUDIN User)
        {
            int status = 0;
            DBcontext.tblUDINs.Add(User);
            status = DBcontext.SaveChanges();
            return status;
        }

        public string UDINGeneration(string MembershipNo)
        {
            char FirstChar = Convert.ToChar(MembershipNo.Substring(0, 1));
            int TotalLength = MembershipNo.Length;
            string First7Digit = string.Empty;
            string UDINNumber = string.Empty;
            for (int i = 0; i < 7 - TotalLength; i++)
            {
                First7Digit += "0";
            }
            First7Digit = FirstChar + First7Digit + MembershipNo.Substring(1, TotalLength - 1);
            foreach (FinancialYear year in Enum.GetValues(typeof(FinancialYear)))
            {
                if (Convert.ToInt32(year) == DateTime.Now.Year)
                    First7Digit = First7Digit + year;
            }


            string Last8Digit = string.Empty;
            int lastValue = 0;
            string FinancialYear = DateTime.Now.Year + "-" + DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2);
            var resGenerateUDIN = DBcontext.tblGenerateUDINs.Where(x => x.FinancialYear == FinancialYear).SingleOrDefault();
            if (resGenerateUDIN == null)
            {
                Last8Digit = "0000000" + 1;
                lastValue = 1;
            }
            else
            {
                lastValue = resGenerateUDIN.TotalCount + 1;
                for (int i = 0; i < 8 - lastValue.ToString().Length; i++)
                {
                    Last8Digit += "0";
                }
                Last8Digit = Last8Digit + lastValue;
            }
            UDINNumber = First7Digit + Last8Digit;

            string MembershipNowithoutChar = MembershipNo.Substring(1, MembershipNo.Length - 1);
            lastValue = Convert.ToInt32(MembershipNowithoutChar) + lastValue;
            int last17Value = Convert.ToInt32((lastValue / 11).ToString().Substring(0, 1));

            UDINNumber = UDINNumber + last17Value;
            return UDINNumber;
        }

        public enum FinancialYear
        {
            A = 2019, B, C, D, E, F, G, H
        }


        public int updatetblUserById(tblUser objtblUser)
        {
            int status = 0;
            tblUser restblUser = DBcontext.tblUsers.Where(x => x.UserId == objtblUser.UserId).FirstOrDefault();
            restblUser.Password = objtblUser.Password;
            status = DBcontext.SaveChanges();
            //restblUser.EmailId = "bgupta@gemini-us.com";
            string body = "Dear " + restblUser.FirstName + ",<br/><br/> Your UDIN Application is Registered Successfully. Your Login Credentials are  as follows :-  LoginID :- " + restblUser.UserId + " and Password:- " + restblUser.Password + ".";

            if (status >= 0 && restblUser.EmailId != null)
                sendMail(restblUser.EmailId, "Create Password", body);

            return status;
        }

        public void sendMail(string MailTo, string Subject, string Body)
        {
            try
            {
                string host = ConfigurationManager.AppSettings["host"].ToString();
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                string username = ConfigurationManager.AppSettings["username"].ToString();
                string password = ConfigurationManager.AppSettings["password"].ToString();

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(host);

                mail.From = new MailAddress(username);
                mail.To.Add(MailTo);
                mail.Subject = Subject;
                mail.Body = Body;
                mail.IsBodyHtml = true;

                SmtpServer.Port = port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);

            }
            catch (Exception ex)
            {

            }
        }

        public int InsertTblUser(tblUser User)
        {
            int status = 0;
            try
            {
                var resTblUser = DBcontext.tblUsers.Where(x => x.DOB == User.DOB && x.UserName == User.UserName).FirstOrDefault();

                if (resTblUser == null)
                {
                    DBcontext.tblUsers.Add(User);
                    status = DBcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                status = 0;
            }


            return status;
        }

        //public List<RP_GetUDINList_Result> GetUDINList(UDINSearch obj)
        //{
        //    try
        //    {

        //        var result = DBcontext.RP_GetUDINList(obj.UserId, obj.UDIN, obj.FinancialYear, obj.FromDate, obj.ToDate).ToList<RP_GetUDINList_Result>();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }

        //}

        public List<RP_GetUDINList_Result> GetUDINList(UDINSearch obj)
        {
            try
            {

                if (obj.UDIN == null)
                {
                    obj.UDIN = string.Empty;
                }
                if (obj.FinancialYear == null)
                {
                    obj.FinancialYear = string.Empty;
                }
                if (obj.FromDate == null)
                {
                    obj.FromDate = string.Empty;

                }
                if (obj.ToDate == null)
                {
                    obj.ToDate = string.Empty;
                }
                var result = DBcontext.Database.SqlQuery<RP_GetUDINList_Result>(
                "exec [dbo].[RP_GetUDINList] @UserId,@UDINNumber,@FinancialYear,@FromDate,@ToDate",
                new Object[] { new SqlParameter("@UserId", obj.UserId),
                               new SqlParameter("@UDINNumber", obj.UDIN),new SqlParameter("@FinancialYear", obj.FinancialYear),new SqlParameter("@FromDate", obj.FromDate),
                new SqlParameter("@ToDate", obj.ToDate)}
                ).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        //public int RevokeUDIN(RP_GetUDINList_Result obj)
        //{
        //    try
        //    {
        //        return DBcontext.RevokeUDIN(obj.MembershipNumber);
        //    }




        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }
        //}

        public int RevokeUDIN(RP_GetUDINList_Result obj)
        {
            try
            {
                return DBcontext.RevokeUDIN(obj.ID, obj.UserId, obj.UDINRevokeReason, obj.MembershipNumber);
            }




            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<Certificate> CertificateList(int TypeOfDocument)
        {
            Certificate objCertificate = new Certificate();
            objCertificate.CertificateId = 0;
            objCertificate.CertificateName = "-- Select --";
            List<Certificate> lstCertificates = DBcontext.tblDocumentTypes.Where(x => x.IsValid == "Y" && x.TypesOfDocument== TypeOfDocument).Select(x => new Certificate { CertificateId = x.DocumentTypeID, CertificateName = x.DocumentType }).ToList();
            lstCertificates.Add(objCertificate);
            return lstCertificates.OrderBy(x => x.CertificateId).ToList();
        }

        public void InsertGenerateUDIN()
        {
            tblGenerateUDIN objGenerateUDIN = new tblGenerateUDIN();
            objGenerateUDIN.FinancialYear = DateTime.Now.Year + "-" + DateTime.Now.AddYears(1).Year.ToString().Substring(2, 2);
            objGenerateUDIN.TotalCount = 1;

            var resGenerateUDIN = DBcontext.tblGenerateUDINs.Where(x => x.FinancialYear == objGenerateUDIN.FinancialYear).FirstOrDefault();
            if (resGenerateUDIN == null)
                DBcontext.tblGenerateUDINs.Add(objGenerateUDIN);
            else
                resGenerateUDIN.TotalCount = resGenerateUDIN.TotalCount + 1;
            DBcontext.SaveChanges();
        }

        public bool CheckUdn(tblUser objuser) //This method check the Udin existence
        {
            bool flag = false;
            var udnId = (from cust in DBcontext.tblUsers
                         join ord in DBcontext.tblUDINs
                         on cust.UserId equals ord.UserId
                         where (cust.UserId == objuser.UserId)
                         select new
                         {
                             ord.UDINUniqueCode
                         }).ToList();

            if (udnId.Count == 0)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }

            return flag;
        }

        public string UDINGenerationEmailBody(string MembershipNo, string UDINNo, string CINNumber, string FinYear, int UDINId, string DateOfSignDoc)
        {
            string DocType = string.Empty;
            string DocDesc = string.Empty;
            string DocDescinDetail = string.Empty;
            var resDocumentType = (from objUDIN in DBcontext.tblUDINs
                                   where objUDIN.ID == UDINId
                                   select new
                                   {
                                       objUDIN.DocumentDescription,
                                       objUDIN.CertificateTypeId,
                                       objUDIN.DocumentTypeId
                                   }).SingleOrDefault();

            if (resDocumentType != null)
            {
                if (resDocumentType.CertificateTypeId == 1)
                    DocType = "Certificates";
                else if (resDocumentType.CertificateTypeId == 2)
                    DocType = "Reports";
                else if (resDocumentType.CertificateTypeId == 3)
                    DocType = "Other Attest Functions";

                if (!string.IsNullOrEmpty(resDocumentType.DocumentDescription))
                    DocDesc = resDocumentType.DocumentDescription;
                else
                {
                    var resDocTypes = DBcontext.tblDocumentTypes.Where(x => x.DocumentTypeID == resDocumentType.DocumentTypeId).Select(x => new { x.DocumentType, x.TypeDesc }).SingleOrDefault();
                    DocDesc = resDocTypes.DocumentType;
                    DocDescinDetail = resDocTypes.TypeDesc;
                }
            }

            StringBuilder sbString = new StringBuilder();
            sbString.Append("<!DOCTYPE html>"
                + "<html>"
                + "<head>"
                + "</head>"
                + "<body>"
                    + "<h2>UDIN GENERATED SUCCESSFULLY</h2>"
                    + "<table style=\"font-family:arial,sans-serif;border-collapse:collapse;width:100%;font-size:13px;\">"
                        + "<tr>"
                            + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">Membership Number</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + MembershipNo + "</td>"
                        + "</tr>"
                        + "<tr style=\"background-color:#dddddd\">"
                            + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">UDIN Number</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + UDINNo + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">CIN Number</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + CINNumber + "</td>"
                        + "</tr>"
                        + "<tr style=\"background-color:#dddddd\">"
                           + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">Financial Year</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + FinYear + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">Document Type(" + DocType + ")</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + DocDesc + "</td>"
                        + "</tr>"
                        + "<tr style=\"background-color:#dddddd\">"
                            + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">Document Description</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + DocDescinDetail + "</td>"
                        + "</tr>"
                        + "<tr>"
                            + "<th style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #2D4383\">Date of signing documents</th>"
                            + "<td style=\"border: 1px solid #dddddd;text-align: left;padding: 8px;color: #666\">" + DateOfSignDoc + "</td>"
                        + "</tr>"
                    + "</table>"
                + "</body>"
                + "</html>");

            return sbString.ToString();
        }

        public GetTotalUserUDIN_Result GetTotalUDINUser()
        {
            try
            {
                return DBcontext.GetTotalUserUDIN().SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }

        }


    }
}