using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ICSI_UDIN.Models;


namespace ICSI_UDIN.Repository
{
    public class UserRepository :IUserRepository

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
      public  IEnumerable<tblUser> GetUsers()
        {
            return DBcontext.tblUsers.ToList();
        }
      public  tblUser GetUserByID(int UserId)
        {
            return DBcontext.tblUsers.Find(UserId);
        }
        public bool CheckLogin(string username, string password) //This method check the user existence

        {
            bool flag = false;
            var loginresult = DBcontext.tblUsers.Where(s => s.UserName == username && s.Password == password).FirstOrDefault();
            if (loginresult == null)
            {
                return flag;
            }
            else
            {
                flag = true;
            }
            return flag ;

        }

        public void Save()
        {
            DBcontext.SaveChanges();
        }
        public tblUDIN GetUDINVerification(tblUDIN obj)
        {
            var Name = new SqlParameter
            {
                ParameterName = "Name",
                Value = obj.Fname
            };

            var Mobileno = new SqlParameter
            {
                ParameterName = "MobileNumber",
                Value = obj.MobileNumber
            };
            var email = new SqlParameter
            {
                ParameterName = "EmailID",
                Value = obj.EmailId
            };
            var udin = new SqlParameter
            {
                ParameterName = "MembershipNumber",
                Value = obj.MembershipNumber
            };
            
            var uddin = DBcontext.tblUDINs.SqlQuery(
            "exec RP_UDINVerification @Name, @MobileNumber, @EmailID, @MembershipNumber ", Name, Mobileno,email,udin).ToList<tblUDIN>();


       


            return uddin.SingleOrDefault(x=>x.ID==1) ;
        }

    }
}