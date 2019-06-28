using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ICSI_UDIN.Models;

namespace ICSI_UDIN.Repository
{
    public interface IUserRepository
    {
        void InsertUser(tblUser User); // C

        IEnumerable<tblUser> GetUsers(); // R

        tblUser GetUserByID(int UserId); // R

        bool CheckLogin(tblUser objuser);

        void UpdateUser(tblUser User); //U

        void DeleteUser(int UserId); //D

        void Save();

        RP_UDINVerification_Result GetUDINVerification(UDINVerification obj);

        int InserttblUDINUser(tblUDIN User);

        string UDINGeneration(string MembershipNo);

        int updatetblUserById(tblUser objtblUser);

        void sendMail(string MailTo, string Subject, string Body);

        int InsertTblUser(tblUser User);

        List<RP_GetUDINList_Result> GetUDINList(UDINSearch obj);

        int RevokeUDIN(RP_GetUDINList_Result obj);

        List<Certificate> CertificateList();

        void InsertGenerateUDIN();

        //
        bool CheckUdn(tblUser objuser);

        tblUser GetUserByUserName(string UserName);
    }
}