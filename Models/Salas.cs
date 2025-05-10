using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Salas
    {
        [Key]
        public int IdSala { get; set; }

        public string Nombre { get; set; }

        public string URL_Imagen { get; set; }

        public int Capacidad { get; set; }

        public int IdTipoSala { get; set; }

        public int IdSede { get; set; }

        public bool Bloqueado { get; set; }

        [ForeignKey("IdSede")]
        public Sedes Sede { get; set; }

        [ForeignKey("IdTipoSala")]
        public TiposSalas TipoSala { get; set; }

        public Salas() { }

        public Salas(int idSala, string nombre, string urlImagen, int capacidad, int idTipoSala, int idSede, bool bloqueado = false)
        {
            IdSala = idSala;
            Nombre = nombre;
            URL_Imagen = urlImagen;
            Capacidad = capacidad;
            IdTipoSala = idTipoSala;
            IdSede = idSede;
            Bloqueado = bloqueado;
        }
    }
}
