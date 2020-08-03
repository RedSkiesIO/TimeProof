using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.PaymentServiceObjects;

namespace AtlasCity.TimeProof.Abstractions.Responses
{
    public static class ResponseExtensions
    {
        public static PaymentIntentResponse ToResponse(this PaymentIntentDao source)
        {
            if (source == null)
                return null;

            return new PaymentIntentResponse
            {
                Id = source.Id,
                ClientSecret = source.ClientSecret,
                Amount = source.Amount
            };
        }

        public static SetupIntentResponse ToResponse(this SetupIntentDao source)
        {
            if (source == null)
                return null;

            return new SetupIntentResponse
            {
                Id = source.Id,
                CustomerId = source.CustomerId,
                ClientSecret = source.ClientSecret
            };
        }

        public static PaymentMethodResponse ToResponse(this PaymentMethodDao source)
        {
            if (source == null)
                return null;

            return new PaymentMethodResponse
            {
                Id = source.Id,
                PaymentCustomerId = source.PaymentCustomerId,
                Card = source.Card.ToResponse(),
                Address = source.Address.ToResponse()
            };
        }

        public static CardResponse ToResponse(this PaymentCard source)
        {
            if (source == null)
                return null;

            return new CardResponse
            {
                Last4 = source.Last4,
                Brand = source.Brand,
                ExpMonth = source.ExpMonth,
                ExpYear = source.ExpYear,
                Issuer = source.Issuer,
                Country = source.Country,
            };
        }

        public static AddressResponse ToResponse(this AddressDao source)
        {
            if (source == null)
                return null;

            return new AddressResponse
            {
                Line1 = source.Line1,
                Line2 = source.Line2,
                City = source.City,
                State = source.State,
                Postcode = source.Postcode,
                Country = source.Country,
            };
        }
    }
}