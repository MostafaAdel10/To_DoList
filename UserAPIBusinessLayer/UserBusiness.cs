using System.Numerics;
using To_DoListDataAccessLayer;
using static System.Net.Mime.MediaTypeNames;

namespace To_DoListAPIBusinessLayer
{
    public class UserBusiness
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public UserDTO UDTO { get { return (new UserDTO(this.User_ID, this.Username, this.Password, this.Email,this.Emage,this.Phone)); } }

        public int User_ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Emage { get; set; }
        public string Phone { get; set; }

        public UserBusiness(UserDTO UDTO, enMode Cmode = enMode.AddNew)
        {
            this.User_ID = UDTO.User_ID;
            this.Username = UDTO.Username;
            this.Password = UDTO.Password;
            this.Email = UDTO.Email;
            this.Emage = UDTO.Emage;
            this.Phone = UDTO.Phone;
            

            Mode = Cmode;
        }
        
        private bool _AddNewUser()
        {
            this.User_ID = UserData.AddNewUser(UDTO);
            return (this.User_ID != -1);
        }

        private bool _UpdateUser()
        {
            return UserData.UpdateUser(UDTO);
        }

        public static List<UserDTO> GetAllUsers()
        {
            return UserData.GetAllUsers();
        }

        public static UserBusiness Find(int userID)
        {
            UserDTO UDTO = UserData.GetUserById(userID);
            if (UDTO != null)
            {
                return new UserBusiness(UDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }
        

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }
        public static bool DeleteUser(int ID)
        {
            return UserData.DeleteUser(ID);
        }

        public static bool IsUserExist(int ID)
        {
            return UserData.IsUserExist(ID);
        }


    }
}
