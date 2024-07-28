using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Dtos
{
    public class MTTicketDto
    {
        public virtual Guid Oid { get; set; }
        public virtual string TicketId { get; set; }
        public virtual string TicketDescription { get; set; }
        public virtual int? Priority { get; set; }
        public virtual DateTime? TicketStartDate { get; set; }
        public virtual DateTime? TicketEstEndDate { get; set; }
        public virtual DateTime? TicketCompletedDate { get; set; }
        public virtual MTTicketStatusDto TicketStatus { get; set; }
        public virtual bool? IsCompleted { get; set; }
    }

    public class MTTicketStatusDto
    {
        public virtual Guid Oid { get; set; }
    }

    public class TicketItemsResponse
    {
        public virtual List<MTTicketDto> Items { get; set; }
    }
}
