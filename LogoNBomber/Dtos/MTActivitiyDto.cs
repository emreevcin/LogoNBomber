using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Dtos
{
    public class MTActivitiyDto
    {
        public virtual string Id { get; set; }
        public virtual string ActivitySubject { get; set; }
        public virtual DateTime? ActivityDate { get; set; }
        public virtual DateTime? ActivityRepeatDate { get; set; }
        public virtual int? Priority { get; set; }
    }

    public class MTActivityResponse
    {
        //public virtual List<MTActivitiyDto> Data { get; set; }
        public virtual Guid Oid { get; set; }
        public virtual string Id { get; set; }
        public virtual string ActivitySubject { get; set; }
        public virtual DateTime? ActivityDate { get; set; }
        public virtual DateTime? ActivityRepeatDate { get; set; }
        public virtual int? Priority { get; set; }
    }
}
