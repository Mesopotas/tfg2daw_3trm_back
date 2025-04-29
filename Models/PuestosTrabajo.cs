namespace Models;

public class PuestosTrabajo
{
        public int IdPuestoTrabajo { get; set; }
        public int NumeroAsiento { get; set; }
        public int CodigoMesa { get; set; }
        public string URL_Imagen { get; set; }
        public bool Disponible { get; set; }
        public bool Bloqueado { get; set; }
    public PuestosTrabajo() { }

    public PuestosTrabajo(int idPuestoTrabajo, int numeroAsiento, int codigoMesa, string urlImagen, bool disponible, bool bloqueado)
    {
        IdPuestoTrabajo = idPuestoTrabajo;
          NumeroAsiento = numeroAsiento;
        CodigoMesa = codigoMesa;
        URL_Imagen = urlImagen;
        Disponible = disponible;
        Bloqueado = bloqueado;
    }
}
