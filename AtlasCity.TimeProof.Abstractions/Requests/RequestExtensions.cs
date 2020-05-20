using System.Collections.Generic;
using System.Linq;
using AtlasCity.TimeProof.Abstractions.DAO;
using AtlasCity.TimeProof.Abstractions.Responses;

namespace AtlasCity.TimeProof.Abstractions.Requests
{
    public static class RequestExtensions
    {
        public static UserDao ToDao(this CreateUserRequest userRequest)
        {
            if (userRequest == null)
                return null;

            return new UserDao
            {
                Email = userRequest.Email,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                Address = userRequest.Address.ToDao(),
            };
        }

        public static AddressDao ToDao(this AddressRequest addressRequest)
        {
            if (addressRequest == null)
                return null;

            return new AddressDao
            {
                Line1 = addressRequest.Line1,
                Line2 = addressRequest.Line2,
                City = addressRequest.City,
                State = addressRequest.State,
                Postcode = addressRequest.Postcode,
                Country = addressRequest.Country
            };
        }

        public static TimestampDao ToDao(this CreateTimestampRequest timestampRequest)
        {
            if (timestampRequest == null)
                return null;

            return new TimestampDao
            {
                PublicKey = timestampRequest.PublicKey,
                FileName = timestampRequest.FileName,
                FileHash = timestampRequest.FileHash,
                Signature = timestampRequest.Signature,
            };
        }

        public static UserResponse ToResponse(this UserDao user)
        {
            if (user == null)
                return null;

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address.ToResponse(),
                RemainingTimeStamps = user.RemainingTimeStamps,
                PricePlanId = user.CurrentPricePlanId,
                PendingPricePlanId = user.PendingPricePlanId,
                PaymentIntentId = user.PaymentIntentId,
                MembershipRenewDate = user.MembershipRenewDate,
                KeyEmailDate = user.KeyEmailSentDate
            };
        }

        public static AddressResponse ToResponse(this AddressDao address)
        {
            if (address == null)
                return null;

            return new AddressResponse
            {
                Line1 = address.Line1,
                Line2 = address.Line2,
                City = address.City,
                State = address.State,
                Postcode = address.Postcode,
                Country = address.Country
            };
        }

        public static TimestampResponse ToResponse(this TimestampDao timestamp)
        {
            if (timestamp == null)
                return null;

            return new TimestampResponse
            {
                Id = timestamp.Id,
                TransactionId = timestamp.TransactionId,
                FileName = timestamp.FileName,
                PublicKey = timestamp.PublicKey,
                Timestamp = timestamp.Timestamp,
                BlockNumber = timestamp.BlockNumber,
                FileHash = timestamp.FileHash,
                Signature = timestamp.Signature,
                Nonce = timestamp.Nonce,
                Network = timestamp.Network,
                UserId = timestamp.UserId,
                Status = timestamp.Status,
            };
        }

        public static IEnumerable<TimestampResponse> ToResponse(this IEnumerable<TimestampDao> timestamps)
        {
            if (timestamps == null || !timestamps.Any())
                return Enumerable.Empty<TimestampResponse>();

            return timestamps.Select(s => s.ToResponse());
        }

        public static PricePlanResponse ToResponse(this PricePlanDao pricePlan)
        {
            if (pricePlan == null)
                return null;

            return new PricePlanResponse
            {
                Id = pricePlan.Id,
                Title = pricePlan.Title,
                Description = pricePlan.Description,
                Price = pricePlan.Price,
                NoOfStamps = pricePlan.NoOfStamps,
                FreqDesc = pricePlan.PaymentFrquencyDescription,
            };
        }

        public static IEnumerable<PricePlanResponse> ToResponse(this IEnumerable<PricePlanDao> pricePlans)
        {
            if (pricePlans == null || !pricePlans.Any())
                return Enumerable.Empty<PricePlanResponse>();

            return pricePlans.Select(s => s.ToResponse());
        }
    }
}