using Microsoft.Data.SqlClient;
using System.Data;
using System.Numerics;

namespace To_DoListDataAccessLayer
{
    public class UserDTO
    {
        public int User_ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Emage { get; set; }
        public string Phone { get; set; }

        public UserDTO(int user_ID, string username, string password, string email, string emage,string phone)
        {
            this.User_ID = user_ID;
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.Emage = emage;
            this.Phone = phone;
        }
    }


    public class UserData
    {
        public static List<UserDTO> GetAllUsers()
        {
            var UsersList = new List<UserDTO>();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetAllUser", con))
                {

                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsersList.Add(new UserDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("User_ID")),
                                    reader.GetString(reader.GetOrdinal("Username")),
                                    reader.GetString(reader.GetOrdinal("Password")),
                                    reader.GetString(reader.GetOrdinal("Email")),
                                    reader.GetString(reader.GetOrdinal("Emage")),
                                    reader.GetString(reader.GetOrdinal("Phone"))

                                ));

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return UsersList;
            }
        }


        public static UserDTO GetUserById(int userId)
        {
            using (var con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_GetUserByID", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                try
                {
                    con.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("User_ID")),
                                reader.GetString(reader.GetOrdinal("Username")),
                                reader.GetString(reader.GetOrdinal("Password")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                reader.GetString(reader.GetOrdinal("Emage")),
                                reader.GetString(reader.GetOrdinal("Phone"))
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


        public static int AddNewUser(UserDTO userDTO)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_AddNewUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", userDTO.Username);
                cmd.Parameters.AddWithValue("@Password", userDTO.Password);
                cmd.Parameters.AddWithValue("@Email", userDTO.Email);
                cmd.Parameters.AddWithValue("@Emage", userDTO.Emage);
                cmd.Parameters.AddWithValue("@Phone", userDTO.Phone);


                var outputIdParam = new SqlParameter("@NewUserID", SqlDbType.Int)
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

        public static bool UpdateUser(UserDTO UserDTO)
        {
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_UpdateUser", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserID", UserDTO.User_ID);
                cmd.Parameters.AddWithValue("@Username", UserDTO.Username);
                cmd.Parameters.AddWithValue("@Password", UserDTO.Password);
                cmd.Parameters.AddWithValue("@Email", UserDTO.Email);
                cmd.Parameters.AddWithValue("@Emage", UserDTO.Emage);
                cmd.Parameters.AddWithValue("@Phone", UserDTO.Phone);

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

        public static bool DeleteUser(int userId)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_DeleteUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", userId);

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

        public static bool IsUserExist(int UserID)
        {
            bool isFound = false;

            using (var connection = new SqlConnection(clsDataAccessSettings.connectionString))
            using (SqlCommand command = new SqlCommand("SP_CheckUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", UserID);
                try
                {
                    connection.Open();

                    // استخدام ExecuteScalar للحصول على نتيجة بسيطة (0 أو 1)
                    var result = command.ExecuteScalar();
                    if (result != null && Convert.ToInt32(result) ==1)
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
