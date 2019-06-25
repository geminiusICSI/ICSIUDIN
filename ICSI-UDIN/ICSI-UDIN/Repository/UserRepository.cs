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
        public bool CheckLogin(tblUser objuser) //This method check the user existence
        {
            bool flag = false;
            var loginresult = DBcontext.tblUsers.Where(s => s.UserName == objuser.UserName && s.Password == objuser.Password).FirstOrDefault();
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
        public RP_UDINVerification_Result GetUDINVerification(RP_UDINVerification_Result obj)
        {
            //var Name = new SqlParameter
            //{
            //    ParameterName = "Name",
            //    Value = obj.Fname
            //};

            //var Mobileno = new SqlParameter
            //{
            //    ParameterName = "MobileNumber",
            //    Value = obj.MobileNumber
            //};
            //var email = new SqlParameter
            //{
            //    ParameterName = "EmailID",
            //    Value = obj.EmailId
            //};
            //var udin = new SqlParameter
            //{
            //    ParameterName = "MembershipNumber",
            //    Value = obj.MembershipNumber
            //};

            //var uddin = DBcontext.tblUDINs.SqlQuery(
            //"exec RP_UDINVerification @Name, @MobileNumber, @EmailID, @MembershipNumber ", Name, Mobileno, email, udin).ToList<tblUDIN>();





            //return uddin.SingleOrDefault(x => x.ID == 1);

            RP_UDINVerification_Result lst = DBcontext.RP_UDINVerification(obj.FName, obj.EmailId, obj.MobileNumber, obj.MembershipNumber.ToString()).ToList<RP_UDINVerification_Result>().FirstOrDefault();

            return lst;
        }
        public void InserttblUDINUser(tblUDIN User)
        {
            DBcontext.tblUDINs.Add(User);

            DBcontext.SaveChanges();
        }

        public void UDINGeneration(string MembershipNo)
        {
            char FirstChar = Convert.ToChar(MembershipNo.Substring(0, 1));
            int TotalLength = MembershipNo.Length;
            string First7Digit = string.Empty;
            for (int i = 0; i < 7 - TotalLength; i++)
            {
                First7Digit += "0";
            }
            First7Digit = FirstChar + First7Digit + MembershipNo.Substring(1, TotalLength-1);
            foreach (FinancialYear year in Enum.GetValues(typeof(FinancialYear)))
            {
                if (Convert.ToInt32(year) == DateTime.Now.Year)
                    First7Digit = First7Digit + year;
            }


        }

        public enum FinancialYear
        {
            A = 2019, B, C, D, E, F, G, H
        }

    }
}