using CoWorking.Models.DTO;

namespace CoWorking.Service
{
    public interface IEmailService
    {
        Task SendReservationConfirmationAsync(string toEmail, string userName, ReservationEmailData reservationData);
        Task MandarCorreoFormulario(string fromName, string fromEmail, string subject, string messageBody);

    }
}