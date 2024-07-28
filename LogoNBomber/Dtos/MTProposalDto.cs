using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Dtos
{
    public class MTProposalDto
    {
        public virtual string ActiveRevisalId { get; set; }
        public virtual int? ProposalId { get; set; }
        public virtual int? Alternative { get; set; }
        public virtual string ProposalDescription { get; set; }
    }

    public class MTProposalResponse : MTProposalDto
    {
        public virtual Guid Oid { get; set; }
    }
}
