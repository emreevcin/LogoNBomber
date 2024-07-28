using Bogus;
using System;
using LogoNBomber.Dtos;

namespace LogoNBomber.Fakers
{
    public class MTActivityDtoFaker : Faker<MTActivityDto>
    {
        public MTActivityDtoFaker() 
        {
            RuleFor(f => f.Id, f => f.Random.Uuid().ToString());
            RuleFor(f => f.ActivitySubject, f => f.Lorem.Sentence());
            RuleFor(f => f.ActivityDate, f => f.Date.Future());
            RuleFor(f => f.Priority, f => f.Random.Number(0, 4));
        }
    }
}
