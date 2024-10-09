using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using To_DoListAPIBusinessLayer;
using To_DoListDataAccessLayer;

namespace To_DoListAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/ToDoList")]

    [ApiController]
    public class ToDoListAPIController : ControllerBase
    {
        [HttpGet("AllToDoList", Name = "GetAllToDoList")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<IEnumerable<ToDoListDTO>> GetAllToDoList()
        {
            List<ToDoListDTO> ToDoListList = ToDoListData.GetAllToDoList();

            if (ToDoListList.Count == 0)
            {
                return NotFound("No ToDoList Found!");
            }
            return Ok(ToDoListList);
        }


        [HttpGet("GetToDoListByID/{ToDo_ID}", Name = "GetToDoListById")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<ToDoListBusiness> GetToDoListById(int ToDo_ID)
        {
            if (ToDo_ID < 1)
            {
                return BadRequest($"Not Accepted ID {ToDo_ID}");
            }

            ToDoListBusiness ToDo = ToDoListBusiness.Find(ToDo_ID);

            if (ToDo == null)
            {
                return NotFound($"ToDoList With ID {ToDo_ID} Not Found.");
            }

            try
            {
                ToDoListDTO TDLDTO = ToDo.TDLDTO;

                return Ok(TDLDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }


        [HttpPost(Name = "AddToDoList")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ToDoListDTO> AddToDoListt(ToDoListDTO NewToDoListDTO)
        {
            var user = UserBusiness.Find(NewToDoListDTO.User_ID);
            var group = GroupBusiness.Find(NewToDoListDTO.Group_ID);

            if (string.IsNullOrEmpty(NewToDoListDTO.Titel))
            {
                return BadRequest("Invalid ToDoList data.");
            }

            if (user == null)
            {
                return NotFound("The User does not exist.");
            }

            if (group == null && NewToDoListDTO.Group_ID != 0)
            {
                return NotFound("The Group does not exist.");
            }

            if (NewToDoListDTO.User_ID == group?.User_ID)
            {
                ToDoListBusiness ToDo =
                new ToDoListBusiness(new ToDoListDTO(NewToDoListDTO.ToDo_ID, NewToDoListDTO.Titel, NewToDoListDTO.Status, NewToDoListDTO.Group_ID, NewToDoListDTO.User_ID));


                try
                {
                    if (ToDo.Save())
                    {
                        NewToDoListDTO.ToDo_ID = ToDo.ToDo_ID;
                    }
                    return CreatedAtRoute("GetToDoListByID", new { NewToDoListDTO.ToDo_ID }, NewToDoListDTO);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error adding StickyNote", error = ex.Message });
                }
            }
            else
            {
                return BadRequest("The User in the ToDoList does not match the User in the Group.");
            }
        }



        [HttpPut("UpdateToDoList/{ToDo_ID}", Name = "UpdateToDoList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ToDoListDTO> UpdateToDoList(int ToDo_ID, ToDoListDTO updatedToDoListDTO)
        {
            var user = UserBusiness.Find(updatedToDoListDTO.User_ID);
            var group = GroupBusiness.Find(updatedToDoListDTO.Group_ID);

            if (ToDo_ID < 1 || string.IsNullOrEmpty(updatedToDoListDTO.Titel))
            {
                return BadRequest("Invalid ToDoList data.");
            }

            if (user == null)
            {
                return NotFound("The User does not exist.");
            }

            if (group == null && updatedToDoListDTO.Group_ID != 0)
            {
                return NotFound("The Group does not exist.");
            }
            if (updatedToDoListDTO.User_ID != group?.User_ID)
            {
                return BadRequest("The User in the ToDoList does not match the User in the Group.");
            }

            ToDoListBusiness ToDo = ToDoListBusiness.Find(ToDo_ID);
            if (ToDo == null)
            {
                return NotFound($"ToDoList with ID {ToDo_ID} not found.");
            }

            ToDo.Titel = updatedToDoListDTO.Titel;
            ToDo.Status = updatedToDoListDTO.Status;
            ToDo.Group_ID = updatedToDoListDTO.Group_ID;
            ToDo.User_ID = updatedToDoListDTO.User_ID;

            try
            {
                if (ToDo.Save())
                    return Ok(ToDo.TDLDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }

            return StatusCode(500, new { massage = "An error occurred while accessing the database" });
        }



        [HttpDelete("DeleteToDoList/{ToDo_ID}", Name = "DeleteToDoList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteToDoList(int ToDo_ID)
        {
            if (ToDo_ID < 1)
            {
                return BadRequest($"Not Accepted ID {ToDo_ID}");
            }

            int Count = ToDoListBusiness.DeleteToDoList(ToDo_ID) ? 1 : 0;

            if (Count == 0)
            {
                return NotFound($"ToDoList with Id {ToDo_ID} not found. no rows deleted!");
            }
            try
            {
                if (Count == 1)
                    return Ok($"ToDoList With Id {ToDo_ID} has been deleted.");
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



        [HttpGet("DoesToDoListExist/{ToDo_ID}", Name = "DoesToDoListExist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DoesStickyNoteExist(int ToDo_ID)
        {
            if (ToDo_ID < 1)
            {
                return BadRequest($"Not Accepted ID {ToDo_ID}");
            }

            try
            {
                if (ToDoListBusiness.IsToDoListExist(ToDo_ID))
                {
                    return Ok($"ToDoList With Id {ToDo_ID} is Exist.");
                }
                else
                {
                    return NotFound($"ToDoList with Id {ToDo_ID} is not Exist");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }



    }
}
