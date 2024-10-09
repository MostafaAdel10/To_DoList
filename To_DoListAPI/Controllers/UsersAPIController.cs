using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using To_DoListAPIBusinessLayer;
using To_DoListDataAccessLayer;

namespace To_DoListAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Users")]
    [ApiController]
    public class UsersAPIController : ControllerBase
    {
        [HttpGet("AllUsers", Name = "GetAllUsers")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            List<UserDTO> UsersList = UserBusiness.GetAllUsers();

            if (UsersList.Count == 0)
            {
                return NotFound("No Students Found!");
            }
            return Ok(UsersList);
        }




        [HttpGet("GetUserByID/{UserID}", Name = "GetUserById")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<UserBusiness> GetUserById(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Not Accepted ID {UserID}");
            }

            UserBusiness user = UserBusiness.Find(UserID);

            if (user == null)
            {
                return NotFound($"User With ID {UserID} Not Found.");
            }

            try
            {
                UserDTO UDTO = user.UDTO;

                return Ok(UDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }




        [HttpPost(Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<UserDTO> AddUser(UserDTO NewUserDTO)
        {
            if (NewUserDTO == null || string.IsNullOrEmpty(NewUserDTO.Username) || string.IsNullOrEmpty(NewUserDTO.Password) || string.IsNullOrEmpty(NewUserDTO.Email) || string.IsNullOrEmpty(NewUserDTO.Phone))
            {
                return BadRequest("Invalid User data.");
            }

            UserBusiness user =
                new UserBusiness(new UserDTO(NewUserDTO.User_ID, NewUserDTO.Username, NewUserDTO.Password, NewUserDTO.Email, NewUserDTO.Emage, NewUserDTO.Phone));

            try
            {
                if (user.Save())
                {
                    NewUserDTO.User_ID = user.User_ID;
                }
                    return CreatedAtRoute("GetUserById", new { UserID = NewUserDTO.User_ID }, NewUserDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = "Error Adding User" });
            }

        }



        [HttpPut("UpdateUser/{UserID}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<UserDTO> UpdateUser(int UserID, UserDTO updatedUserDTO)
        {
            if (UserID < 1 || string.IsNullOrEmpty(updatedUserDTO.Username) || string.IsNullOrEmpty(updatedUserDTO.Password) || string.IsNullOrEmpty(updatedUserDTO.Email) || string.IsNullOrEmpty(updatedUserDTO.Phone))
            {
                return BadRequest("Invalid User data.");
            }

            UserBusiness user = UserBusiness.Find(UserID);
            if (user == null)
            {
                return NotFound($"User with ID {UserID} not found.");
            }
            user.Username = updatedUserDTO.Username;
            user.Password = updatedUserDTO.Password;
            user.Email = updatedUserDTO.Email;
            user.Emage = updatedUserDTO.Emage;
            user.Phone = updatedUserDTO.Phone;

            try
            {
                if (user.Save())
                    return Ok(user.UDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }

                return StatusCode(500, new { massage = "An error occurred while accessing the database" });
        }





        [HttpDelete("DeleteUser/{UserID}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteUser(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Not Accepted ID {UserID}");
            }
            int Count = UserBusiness.DeleteUser(UserID) ? 1 : 0;

            if (Count == 0)
            {
                return NotFound($"Student with Id {UserID} not found. no rows deleted!");
            }
            try
            {
                if (Count == 1)
                    return Ok($"User With Id {UserID} has been deleted.");                    
            }
            catch (SqlException ex)
            {
                // فشل الحذف بسبب العلاقة مع جدول آخر أو أي خطأ SQL آخر
                if (ex.Number == 547) // كود الخطأ 547 يعني أنه فشل بسبب علاقة المفتاح الأجنبي
                {
                    return BadRequest("You cannot delete this item because it is related to other records in different tables");
                }
                else
                {
                    // التعامل مع أخطاء SQL الأخرى
                    return StatusCode(500, new { massage = ex.Message });
                }
            }
            return StatusCode(500, new { massage = "An error occurred while accessing the database" });
        }


        
        [HttpGet("DoesUserExist/{UserID}", Name = "DoesUserExist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DoesUserExist(int UserID)
        {
            if (UserID < 1)
            {
                return BadRequest($"Not Accepted ID {UserID}");
            }

            try
            {
                if (UserBusiness.IsUserExist(UserID))
                {
                    return Ok($"User With Id {UserID} is Exist.");
                }
                else
                {
                    return NotFound($"User with Id {UserID} is not Exist");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }




    }
}
