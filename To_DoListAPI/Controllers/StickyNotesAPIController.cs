using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using To_DoListAPIBusinessLayer;
using To_DoListDataAccessLayer;

namespace To_DoListAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/StickyNotes")]

    [ApiController]
    public class StickyNotesAPIController : ControllerBase
    {

        [HttpGet("AllStickyNotes", Name = "GetAllStickyNotes")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<IEnumerable<StickyNoteDTO>> GetAllStickyNotes()
        {
            List<StickyNoteDTO> StickyNotesList = StickyNoteBusiness.GetAllStickyNotes();

            if (StickyNotesList.Count == 0)
            {
                return NotFound("No StickyNotes Found!");
            }
            return Ok(StickyNotesList);
        }





        [HttpGet("GetStickyNoteByID/{SN_ID}", Name = "GetStickyNoteById")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<StickyNoteBusiness> GetStickyNoteById(int SN_ID)
        {
            if (SN_ID < 1)
            {
                return BadRequest($"Not Accepted ID {SN_ID}");
            }

            StickyNoteBusiness SN = StickyNoteBusiness.Find(SN_ID);

            if (SN == null)
            {
                return NotFound($"StickyNote With ID {SN_ID} Not Found.");
            }

            try
            {
                StickyNoteDTO SNDTO = SN.SNDTO;

                return Ok(SNDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }




        [HttpPost(Name = "AddStickyNote")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StickyNoteDTO> AddStickyNot(StickyNoteDTO NewStickyNoteDTO)
        {
            var user = UserBusiness.Find(NewStickyNoteDTO.User_ID);
            var group = GroupBusiness.Find(NewStickyNoteDTO.Group_ID);

            if (string.IsNullOrEmpty(NewStickyNoteDTO.Titel) || string.IsNullOrEmpty(NewStickyNoteDTO.Description) || string.IsNullOrEmpty(NewStickyNoteDTO.Color))
            {
                return BadRequest("Invalid StickyNote data.");
            }

            if (user == null)
            {
                return NotFound("The User does not exist.");
            }

            if (group == null && NewStickyNoteDTO.Group_ID != 0)
            {
                return NotFound("The Group does not exist.");
            }

            if (NewStickyNoteDTO.User_ID == group?.User_ID)
            {
                StickyNoteBusiness SN =
                new StickyNoteBusiness(new StickyNoteDTO(NewStickyNoteDTO.SN_ID, NewStickyNoteDTO.Titel, NewStickyNoteDTO.Description, NewStickyNoteDTO.Color, NewStickyNoteDTO.User_ID, NewStickyNoteDTO.Group_ID));


                try
                {
                    if (SN.Save())
                    {
                        NewStickyNoteDTO.SN_ID = SN.SN_ID;
                    }
                    return CreatedAtRoute("GetStickyNoteByID", new { NewStickyNoteDTO.SN_ID }, NewStickyNoteDTO);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error adding StickyNote", error = ex.Message });
                }
            }
            else
            {
                return BadRequest("The User in the StickyNote does not match the User in the Group.");
            }
        }




        [HttpPut("UpdateStickyNote/{SN_ID}", Name = "UpdateStickyNote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StickyNoteDTO> UpdateStickyNote(int SN_ID, StickyNoteDTO updatedStickyNoteDTO)
        {
            var user = UserBusiness.Find(updatedStickyNoteDTO.User_ID);
            var group = GroupBusiness.Find(updatedStickyNoteDTO.Group_ID);

            if (SN_ID < 1 || string.IsNullOrEmpty(updatedStickyNoteDTO.Titel) || string.IsNullOrEmpty(updatedStickyNoteDTO.Description) || string.IsNullOrEmpty(updatedStickyNoteDTO.Color))
            {
                return BadRequest("Invalid StickyNote data.");
            }

            if (user == null)
            {
                return NotFound("The User does not exist.");
            }

            if (group == null && updatedStickyNoteDTO.Group_ID != 0)
            {
                return NotFound("The Group does not exist.");
            }
            if (updatedStickyNoteDTO.User_ID != group?.User_ID)
            {
                return BadRequest("The User in the StickyNote does not match the User in the Group.");
            }

            StickyNoteBusiness SN = StickyNoteBusiness.Find(SN_ID);
            if (SN == null)
            {
                return NotFound($"StickyNote with ID {SN_ID} not found.");
            }
            SN.Titel = updatedStickyNoteDTO.Titel;
            SN.Description = updatedStickyNoteDTO.Description;
            SN.Color = updatedStickyNoteDTO.Color;
            SN.User_ID = updatedStickyNoteDTO.User_ID;
            SN.Group_ID = updatedStickyNoteDTO.Group_ID;

            try
            {
                if (SN.Save())
                    return Ok(SN.SNDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }

            return StatusCode(500, new { massage = "An error occurred while accessing the database" });
        }



        [HttpDelete("DeleteStickyNote/{SN_ID}", Name = "DeleteStickyNote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteStickyNote(int SN_ID)
        {
            if (SN_ID < 1)
            {
                return BadRequest($"Not Accepted ID {SN_ID}");
            }

            int Count = StickyNoteBusiness.DeleteStickyNote(SN_ID) ? 1 : 0;

            if (Count == 0)
            {
                return NotFound($"StickyNote with Id {SN_ID} not found. no rows deleted!");
            }
            try
            {
                if (Count == 1)
                    return Ok($"StickyNote With Id {SN_ID} has been deleted.");
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



        [HttpGet("DoesStickyNoteExist/{SN_ID}", Name = "DoesStickyNoteExist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DoesStickyNoteExist(int SN_ID)
        {
            if (SN_ID < 1)
            {
                return BadRequest($"Not Accepted ID {SN_ID}");
            }

            try
            {
                if (StickyNoteBusiness.IsStickyNoteExist(SN_ID))
                {
                    return Ok($"StickyNote With Id {SN_ID} is Exist.");
                }
                else
                {
                    return NotFound($"StickyNote with Id {SN_ID} is not Exist");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }



    }
}
