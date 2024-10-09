using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_DoListDataAccessLayer
{
    public class StickyNoteDTO
    {
        public int SN_ID { get; set; }
        public string Titel { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int User_ID { get; set; }
        public int Group_ID { get; set; }

        public StickyNoteDTO(int sN_ID, string titel, string description, string color, int user_ID, int group_ID)
        {
            this.SN_ID = sN_ID;
            this.Titel = titel;
            this.Description = description;
            this.Color = color;
            this.User_ID = user_ID;
            this.Group_ID = group_ID;

        }
    }
    public class StickyNoteData
    {
        public static List<StickyNoteDTO> GetAllStickyNote()
        {
            var StickyNoteList = new List<StickyNoteDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllStickyNotes", con))
                {

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StickyNoteList.Add(new StickyNoteDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("SN_ID")),
                                    reader.GetString(reader.GetOrdinal("Titel")),
                                    reader.GetString(reader.GetOrdinal("Description")),
                                    reader.GetString(reader.GetOrdinal("Color")),
                                    reader.GetInt32(reader.GetOrdinal("User_ID")),
                                    reader.GetInt32(reader.GetOrdinal("Group_ID"))

                                ));

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return StickyNoteList;
            }
        }


        public static StickyNoteDTO GetStickyNoteById(int sn_Id)
        {
            using (var con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetStickyNoteByID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SN_ID", sn_Id);
                try
                {
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new StickyNoteDTO
                            (
                                 reader.GetInt32(reader.GetOrdinal("SN_ID")),
                                 reader.GetString(reader.GetOrdinal("Titel")),
                                 reader.GetString(reader.GetOrdinal("Description")),
                                 reader.GetString(reader.GetOrdinal("Color")),
                                 reader.GetInt32(reader.GetOrdinal("User_ID")),
                                 reader.GetInt32(reader.GetOrdinal("Group_ID"))
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


        public static int AddNewStickyNote(StickyNoteDTO stickyNoteDTODTO)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_AddNewStickyNotes", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Titel", stickyNoteDTODTO.Titel);
                cmd.Parameters.AddWithValue("@Description", stickyNoteDTODTO.Description);
                cmd.Parameters.AddWithValue("@Color", stickyNoteDTODTO.Color);
                cmd.Parameters.AddWithValue("@User_ID", stickyNoteDTODTO.User_ID);
                cmd.Parameters.AddWithValue("@Group_ID", stickyNoteDTODTO.Group_ID );


                var outputIdParam = new SqlParameter("@SN_ID", SqlDbType.Int)
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


        public static bool UpdateStickyNote(StickyNoteDTO stickyNoteDTO)
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_UpdateStickyNote", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SN_ID", stickyNoteDTO.SN_ID);
                cmd.Parameters.AddWithValue("@Titel", stickyNoteDTO.Titel);
                cmd.Parameters.AddWithValue("@Description", stickyNoteDTO.Description);
                cmd.Parameters.AddWithValue("@Color", stickyNoteDTO.Color);
                cmd.Parameters.AddWithValue("@User_ID", stickyNoteDTO.User_ID);
                cmd.Parameters.AddWithValue("@Group_ID", stickyNoteDTO.Group_ID);

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



        public static bool DeleteStickyNote(int sn_Id)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_DeleteStickyNotes", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SN_ID", sn_Id);

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


        public static bool IsStickyNoteExist(int sn_Id)
        {
            bool isFound = false;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_CheckStickyNote", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SN_ID", sn_Id);
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
