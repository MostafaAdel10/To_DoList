using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_DoListDataAccessLayer
{
    public class GroupDTO
    {
        public int Group_ID { get; set; }
        public string Groupname { get; set; }
        public string Color { get; set; }
        public int User_ID { get; set; }
       

        public GroupDTO(int group_ID, string groupname, string color, int user_ID)
        {
            this.Group_ID = group_ID;
            this.Groupname = groupname;
            this.Color = color;
            this.User_ID = user_ID;
        }
    }
    public class GroupData
    {
        public static List<GroupDTO> GetAllGroubs()
        {
            var GroupList = new List<GroupDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllGroups", con))
                {

                        cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GroupList.Add(new GroupDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("Group_ID")),
                                    reader.GetString(reader.GetOrdinal("Groupname")),
                                    reader.GetString(reader.GetOrdinal("Color")),
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
                return GroupList;
            }
        }


        public static GroupDTO GetGroupById(int groupId)
        {
            using (var con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetGroupByID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GroupID", groupId);
                try
                {
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new GroupDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("Group_ID")),
                                reader.GetString(reader.GetOrdinal("Groupname")),
                                reader.GetString(reader.GetOrdinal("Color")),
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


        public static int AddNewGroub(GroupDTO groupDTO)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_AddNewGroup", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Groupname", groupDTO.Groupname);
                cmd.Parameters.AddWithValue("@Color", groupDTO.Color);
                cmd.Parameters.AddWithValue("@UserID", groupDTO.User_ID);


                var outputIdParam = new SqlParameter("@GroupID", SqlDbType.Int)
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


        public static bool UpdateGroup(GroupDTO groupDTO)
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_UpdateGroup", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@GroupID", groupDTO.Group_ID);
                cmd.Parameters.AddWithValue("@Groupname", groupDTO.Groupname);
                cmd.Parameters.AddWithValue("@Color", groupDTO.Color);
                cmd.Parameters.AddWithValue("@UserID", groupDTO.User_ID);

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


        public static bool DeleteGroup(int groupId)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_DeleteGroup", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@GroupID", groupId);

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




        public static bool IsGroupExist(int GroupID)
        {
            bool isFound = false;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_CheckGroup", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@GroupID", GroupID);

                try
                {
                    connection.Open();

                    // استخدام ExecuteScalar للبساطة والسرعة
                    var result = command.ExecuteScalar();
                    if (result != null && Convert.ToInt32(result) ==1)
                    {
                        isFound = true;
                    }
                }
                catch (Exception ex)
                {
                    // يمكنك تسجيل الخطأ هنا إذا كنت تستخدم نظام تسجيل
                    // isFound يظل false هنا
                    isFound = false;
                }

            }

            return isFound;
        }



    }
}
