using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoWorking.DTO
{
    public class ZonasTrabajoDTO
    {
        [Key]
        public int IdZonaTrabajo { get; set; }

        public string Descripcion { get; set; }

        public int IdSala { get; set; }

        public ZonasTrabajoDTO() { }


    }
}
