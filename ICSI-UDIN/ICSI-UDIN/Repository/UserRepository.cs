using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ICSI_UDIN.Models;


namespace ICSI_UDIN.Repository
{
    public class UserRepository :IUserRepository

    {

        private UserDBEntities DBcontext;

        public UserRepository(UserDBEntities objusercontext)
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

    }
}