using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_DoListDataAccessLayer;

namespace To_DoListAPIBusinessLayer
{
    public class GroupBusiness
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public GroupDTO GDTO { get { return (new GroupDTO(this.Group_ID, this.Groupname, this.Color, this.User_ID)); } }

        public int Group_ID { get; set; }
        public string Groupname { get; set; }
        public string Color { get; set; }
        public int User_ID { get; set; }

        public GroupBusiness(GroupDTO GDTO, enMode Cmode = enMode.AddNew)
        {
            this.Group_ID = GDTO.Group_ID;
            this.Groupname = GDTO.Groupname;
            this.Color = GDTO.Color;
            this.User_ID = GDTO.User_ID;

            Mode = Cmode;
        }
        public static bool IsGroupExist(int ID)
        {
            return GroupData.IsGroupExist(ID);
        }
        private bool _AddNewGroup()
        {
            this.Group_ID = GroupData.AddNewGroub(GDTO);
            return (this.Group_ID != -1);
        }
        private bool _UpdateGroup()
        {
            return GroupData.UpdateGroup(GDTO);
        }
        public static List<GroupDTO> GetAllGroups()
        {
            return GroupData.GetAllGroubs();
        }

        public static GroupBusiness Find(int groupID)
        {
            GroupDTO GDTO = GroupData.GetGroupById(groupID);

            if (GDTO != null)
            {
                return new GroupBusiness(GDTO, enMode.Update);
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
                    if (_AddNewGroup())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateGroup();
            }
            return false;
        }

        public static bool DeleteGroup(int ID)
        {
            return GroupData.DeleteGroup(ID);
        }

        





    }
}
