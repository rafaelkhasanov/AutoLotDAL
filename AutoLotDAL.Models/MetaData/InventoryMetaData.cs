using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AutoLotDAL.Models.MetaData
{
    public class InventoryMetaData
    {
        [Display(Name = "Pet Name")]
        public string PetName;
    }
}
