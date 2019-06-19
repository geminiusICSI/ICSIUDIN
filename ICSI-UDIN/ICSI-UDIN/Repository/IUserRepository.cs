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

        bool CheckLogin(string UserName, string Password);

        void UpdateUser(tblUser User); //U

        void DeleteUser(int UserId); //D

        void Save();
        tblUDIN GetUDINVerification(tblUDIN obj);
    }
}