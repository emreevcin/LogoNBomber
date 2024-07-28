using Bogus;
using LogoNBomber.Dtos;
using System;

namespace LogoNBomber.Fakers
{
    public class MTProposalDtoFaker : Faker<MTProposalDto>
    {
        public MTProposalDtoFaker()
        {
            RuleFor(p => p.ActiveRevisalId, f => f.Commerce.Ean13())
            .RuleFor(p => p.ProposalId, f => f.Random.Int(1, 1000))
            .RuleFor(p => p.Alternative, f => f.Random.Int(1, 5))
            .RuleFor(p => p.ProposalDescription, f => f.Commerce.ProductDescription());
        }
    }
}
