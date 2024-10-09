using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using To_DoListAPIBusinessLayer;
using To_DoListDataAccessLayer;

namespace To_DoListAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Groups")]

    [ApiController]
    public class GroupsAPIController : ControllerBase
    {
        [HttpGet("AllGroups", Name = "GetAllGroups")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<IEnumerable<GroupDTO>> GetAllGroups()
        {
            List<GroupDTO> GroupsList = GroupBusiness.GetAllGroups();

            if (GroupsList.Count == 0)
            {
                return NotFound("No Groups Found!");
            }
            return Ok(GroupsList);
        }




        [HttpGet("GetGroupByID/{GroupID}", Name = "GetGroupById")]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<GroupBusiness> GetGroupById(int GroupID)
        {
            if (GroupID < 1)
            {
                return BadRequest($"Not Accepted ID {GroupID}");
            }

            GroupBusiness group = GroupBusiness.Find(GroupID);

            if (group == null)
            {
                return NotFound($"Group With ID {GroupID} Not Found.");
            }

            try
            {
                GroupDTO GDTO = group.GDTO;

                return Ok(GDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }




        [HttpPost(Name = "AddGroup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<GroupDTO> AddGroup(GroupDTO NewGroupDTO)
        {
            UserBusiness userID = UserBusiness.Find(NewGroupDTO.User_ID);

            if (NewGroupDTO == null || string.IsNullOrEmpty(NewGroupDTO.Groupname) || string.IsNullOrEmpty(NewGroupDTO.Color))
            {
                return BadRequest("Invalid Group data.");
            }
            if(userID == null)
            {
                return NotFound("The User is not Exist.");
            }

            GroupBusiness group =
                new GroupBusiness(new GroupDTO(NewGroupDTO.Group_ID, NewGroupDTO.Groupname, NewGroupDTO.Color, NewGroupDTO.User_ID));

            try
            {
                if (group.Save())
                {
                    NewGroupDTO.Group_ID = group.Group_ID;
                }
                return CreatedAtRoute("GetGroupById", new { GroupID = NewGroupDTO.Group_ID }, NewGroupDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = "Error Adding Group" });
            }

        }




        [HttpPut("UpdateGroup/{GroupID}", Name = "UpdateGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<GroupDTO> UpdateGroup(int GroupID, GroupDTO updatedGroupDTO)
        {
            UserBusiness userID = UserBusiness.Find(updatedGroupDTO.User_ID);

            if (GroupID < 1 || string.IsNullOrEmpty(updatedGroupDTO.Groupname) || string.IsNullOrEmpty(updatedGroupDTO.Color))
            {
                return BadRequest("Invalid Group data.");
            }

            if (userID == null)
            {
                return NotFound("The User is not Exist.");
            }

            GroupBusiness group = GroupBusiness.Find(GroupID);
            if (group == null)
            {
                return NotFound($"Group with ID {GroupID} not found.");
            }
            group.Groupname = updatedGroupDTO.Groupname;
            group.Color = updatedGroupDTO.Color;
            group.User_ID = updatedGroupDTO.User_ID;

            try
            {
                if (group.Save())
                    return Ok(group.GDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }

            return StatusCode(500, new { massage = "An error occurred while accessing the database" });
        }


        [HttpDelete("DeleteGroup/{GroupID}", Name = "DeleteGroup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteGroup(int GroupID)
        {
            if (GroupID < 1)
            {
                return BadRequest($"Not Accepted ID {GroupID}");
            }
            
            try
            {
                if (GroupBusiness.DeleteGroup(GroupID))
                    return Ok($"Group With Id {GroupID} has been deleted.");
                else
                    return NotFound($"Group with Id {GroupID} not found. no rows deleted!");
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
            //return StatusCode(500, new { massage = "An error occurred while accessing the database" });
        }





        //error
        [HttpGet("DoesGroupExist/{GroupID}", Name = "DoesGroupExist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DoesGroupExist(int GroupID)
        {
            if (GroupID < 1)
            {
                return BadRequest($"Not Accepted ID {GroupID}");
            }

            try
            {
                if (GroupBusiness.IsGroupExist(GroupID))
                {
                    return Ok($"Group With Id {GroupID} is Exist.");
                }
                else
                {
                    return NotFound($"Group with Id {GroupID} is not Exist");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { massage = ex.Message });
            }
        }





    }
}
