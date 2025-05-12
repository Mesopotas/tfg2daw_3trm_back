using System.Collections.Generic;

namespace Models
{
    public class SalasDTO
    {
        public int IdSala { get; set; }
        public string Nombre { get; set; }
        public string URL_Imagen { get; set; }
        public int Capacidad { get; set; }
        public int IdTipoSala { get; set; }
        public int IdSede { get; set; }
        public bool Bloqueado { get; set; }
        // no es necesario cargar los todos los asientos ni las zonas en algunos endpoints que lo haran mas lento
    }


}