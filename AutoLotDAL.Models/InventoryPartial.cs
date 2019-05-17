using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using AutoLotDAL.Models.Base;
using System.ComponentModel.DataAnnotations;
using AutoLotDAL.Models.MetaData;

namespace AutoLotDAL.Models
{
    [MetadataType(typeof(InventoryMetaData))]
    public partial class Inventory : EntityBase
    {
        [NotMapped]
        public string MakeColor => $"{Make} + ({Color})";
        public override string ToString()
        {
            //Т.к. столбец PetName может быть пустым, предоставить стандартное имя "NoName"
            return $"{this.PetName ?? "**No Name**"} is a {this.Color} {this.Make} with ID {this.Id}.";
        }
    }
}
