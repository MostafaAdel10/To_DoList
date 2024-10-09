using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_DoListDataAccessLayer;

namespace To_DoListAPIBusinessLayer
{
    public class ToDoListBusiness
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public ToDoListDTO TDLDTO { get { return (new ToDoListDTO(this.ToDo_ID, this.Titel, this.Status, this.Group_ID, this.User_ID)); } }

        public int ToDo_ID { get; set; }
        public string Titel { get; set; }
        public bool Status { get; set; }
        public int Group_ID { get; set; }
        public int User_ID { get; set; }

        public ToDoListBusiness(ToDoListDTO TDLDTO, enMode Cmode = enMode.AddNew)
        {
            this.ToDo_ID = TDLDTO.ToDo_ID;
            this.Titel = TDLDTO.Titel;
            this.Status = TDLDTO.Status;
            this.Group_ID = TDLDTO.Group_ID;
            this.User_ID = TDLDTO.User_ID;

            Mode = Cmode;
        }
        private bool _AddNewToDoList()
        {
            this.ToDo_ID = ToDoListData.AddNewToDoList(TDLDTO);
            return (this.ToDo_ID != -1);
        }
        private bool _UpdateToDoList()
        {
            return ToDoListData.UpdateToDoList(TDLDTO);
        }

        public static List<ToDoListDTO> GetAllToDoList()
        {
            return ToDoListData.GetAllToDoList();
        }


        public static ToDoListBusiness Find(int todo_ID)
        {
            ToDoListDTO TDLDTO = ToDoListData.GetToDoListById(todo_ID);
            if (TDLDTO != null)
            {
                return new ToDoListBusiness(TDLDTO, enMode.Update);
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
                    if (_AddNewToDoList())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateToDoList();
            }
            return false;
        }
        public static bool DeleteToDoList(int todo_ID)
        {
            return ToDoListData.DeleteToDoList(todo_ID);
        }
        public static bool IsToDoListExist(int todo_ID)
        {
            return ToDoListData.IsToDoListExist(todo_ID);
        }
    }
}
