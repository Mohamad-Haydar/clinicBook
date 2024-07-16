using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.library.Models.Request
{
    public class UpdateReservationRequest
    {
        #pragma warning disable IDE1006
        public required int client_reservation_id { get; set; }
        public required IList<int> doctor_service_ids { get; set; }
        #pragma warning restore IDE1006
    }
}
