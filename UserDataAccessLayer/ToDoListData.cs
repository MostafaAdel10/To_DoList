using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_DoListDataAccessLayer
{
    public class ToDoListDTO
    {
        public int ToDo_ID { get; set; }
        public string Titel { get; set; }
        public bool Status { get; set; }
        public int Group_ID { get; set; }
        public int User_ID { get; set; }

        public ToDoListDTO(int toDo_ID, string titel, bool status, int group_ID, int user_ID)
        {
            this.ToDo_ID = toDo_ID;
            this.Titel = titel;
            this.Status = status;
            this.Group_ID = group_ID;
            this.User_ID = user_ID;

        }
    }
    public class ToDoListData
    {
        public static List<ToDoListDTO> GetAllToDoList()
        {
            var ToDoListList = new List<ToDoListDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllToDoList", con))
                {

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ToDoListList.Add(new ToDoListDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("ToDo_ID")),
                                    reader.GetString(reader.GetOrdinal("Titel")),
                                    reader.GetBoolean(reader.GetOrdinal("Status")),
                                    reader.GetInt32(reader.GetOrdinal("Group_ID")),
                                    reader.GetInt32(reader.GetOrdinal("User_ID"))

                                ));

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return ToDoListList;
            }
        }

        public static ToDoListDTO GetToDoListById(int todo_Id)
        {
            using (var con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetToDoListByID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ToDo_ID", todo_Id);
                try
                {
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ToDoListDTO
                            (
                                 reader.GetInt32(reader.GetOrdinal("ToDo_ID")),
                                 reader.GetString(reader.GetOrdinal("Titel")),
                                 reader.GetBoolean(reader.GetOrdinal("Status")),
                                 reader.GetInt32(reader.GetOrdinal("Group_ID")),
                                 reader.GetInt32(reader.GetOrdinal("User_ID"))
                            );
                        }
                        else
                        {
                            return null;
                        }

                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public static int AddNewToDoList(ToDoListDTO toDoListDTO)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_AddNewToDoList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Titel", toDoListDTO.Titel);
                cmd.Parameters.AddWithValue("@Status", toDoListDTO.Status);
                cmd.Parameters.AddWithValue("@Group_ID", toDoListDTO.Group_ID);
                cmd.Parameters.AddWithValue("@User_ID", toDoListDTO.User_ID);


                var outputIdParam = new SqlParameter("@ToDo_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    return (int)outputIdParam.Value;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public static bool UpdateToDoList(ToDoListDTO toDoListDTO)
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_UpdateToDoList", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ToDo_ID", toDoListDTO.ToDo_ID);
                cmd.Parameters.AddWithValue("@Titel", toDoListDTO.Titel);
                cmd.Parameters.AddWithValue("@Status", toDoListDTO.Status);
                cmd.Parameters.AddWithValue("@Group_ID", toDoListDTO.Group_ID);
                cmd.Parameters.AddWithValue("@User_ID", toDoListDTO.User_ID);

                try
                {
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();

                    //return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                return (rowsAffected > 0);
            }
        }

        public static bool DeleteToDoList(int todo_Id)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_DeleteToDoList", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ToDo_ID", todo_Id);

                try
                {
                    connection.Open();

                    rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return (rowsAffected > 0);
        }


        public static bool IsToDoListExist(int todo_Id)
        {
            bool isFound = false;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_CheckToDoList", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ToDo_ID", todo_Id);
                try
                {
                    connection.Open();

                    // استخدام ExecuteScalar للحصول على نتيجة بسيطة (0 أو 1)
                    var result = command.ExecuteScalar();
                    if (result != null && Convert.ToInt32(result) == 1)
                    {
                        isFound = true;
                    }
                }
                catch (Exception ex)
                {
                    // يمكنك تسجيل الخطأ هنا إذا كنت تستخدم نظام تسجيل
                    // isFound يبقى false
                    isFound = false;
                }
            }

            return isFound;
        }


    }
}
