using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_DoListDataAccessLayer;

namespace To_DoListAPIBusinessLayer
{
    public class StickyNoteBusiness
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public StickyNoteDTO SNDTO { get { return (new StickyNoteDTO(this.SN_ID, this.Titel, this.Description, this.Color, this.User_ID, this.Group_ID)); } }

        public int SN_ID { get; set; }
        public string Titel { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int User_ID { get; set; }
        public int Group_ID { get; set; }

        public StickyNoteBusiness(StickyNoteDTO SNDTO, enMode Cmode = enMode.AddNew)
        {
            this.SN_ID = SNDTO.SN_ID;
            this.Titel = SNDTO.Titel;
            this.Description = SNDTO.Description;
            this.Color = SNDTO.Color;
            this.User_ID = SNDTO.User_ID;
            this.Group_ID = SNDTO.Group_ID;

            Mode = Cmode;
        }

        private bool _AddNewStickyNote()
        {
            this.SN_ID = StickyNoteData.AddNewStickyNote(SNDTO);
            return (this.SN_ID != -1);
        }
        private bool _UpdateStickyNote()
        {
            return StickyNoteData.UpdateStickyNote(SNDTO);
        }

        public static List<StickyNoteDTO> GetAllStickyNotes()
        {
            return StickyNoteData.GetAllStickyNote();
        }


        public static StickyNoteBusiness Find(int sn_ID)
        {
            StickyNoteDTO SNDTO = StickyNoteData.GetStickyNoteById(sn_ID);
            if (SNDTO != null)
            {
                return new StickyNoteBusiness(SNDTO, enMode.Update);
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
                    if (_AddNewStickyNote())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return _UpdateStickyNote();
            }
            return false;
        }



        public static bool DeleteStickyNote(int sn_ID)
        {
            return StickyNoteData.DeleteStickyNote(sn_ID);
        }


        public static bool IsStickyNoteExist(int sn_ID)
        {
            return StickyNoteData.IsStickyNoteExist(sn_ID);
        }


    }
}
