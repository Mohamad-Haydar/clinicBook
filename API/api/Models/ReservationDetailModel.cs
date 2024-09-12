using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    [Table("reservationdetail")]
    public class ReservationDetailModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("doctorserviceid")]
        public int DoctorServiceId { get; set; }
        [Column("clientreservationid")]
        public int ClientReservationId { get; set; }
    }
}
