using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;
    public class Lineas
    {
        [Key]
        public int IdLinea { get; set; }
        
        public int IdReserva { get; set; }
        public int IdPuestoTrabajo { get; set; }
        
        public decimal Precio { get; set; }
        
        [ForeignKey("IdReserva")]
        public Reservas Reserva { get; set; }
        
        [ForeignKey("IdPuestoTrabajo")]
        public PuestosTrabajo PuestoTrabajo { get; set; }
    
    public Lineas() { }

    public Lineas(int idLinea, int idReserva, int idPuestoTrabajo, decimal precio)
    {
        IdLinea = idLinea;
        IdReserva = idReserva;
        IdPuestoTrabajo = idPuestoTrabajo;
        Precio = precio;
    }
}
