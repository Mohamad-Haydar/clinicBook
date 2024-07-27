using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class UpdateReservationRequest
    {
        #pragma warning disable IDE1006
        [Required(ErrorMessage = "client reservation id is required"),
         Range(minimum: 1, maximum: Double.MaxValue, ErrorMessage = "client reservation id should be greater than 0")]
        public int? client_reservation_id { get; set; }
        [Required(ErrorMessage = "doctor service ids are required")]
        public IList<int> doctor_service_ids { get; set; }
        #pragma warning restore IDE1006
    }
}
