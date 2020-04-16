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
                Id = userRequest.Id,
                Email = userRequest.Email,
                FirstName = userRequest.FirstName,
                LastName = userRequest.LastName,
                PaymentCustomerId = userRequest.PaymentCustomerId,
                SetupIntentId = userRequest.SetupIntentId,
                Address = userRequest.Address.ToDao(),
                CurrentPricePlanId = userRequest.PricePlanId,
                PaymentIntentId = userRequest.PaymentIntentId
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
                Id = timestampRequest.Id,
                TransactionId = timestampRequest.TransactionId,
                FileName = timestampRequest.FileName,
                PublicKey = timestampRequest.PublicKey,
                Hash = timestampRequest.Hash,
                Timestamp = timestampRequest.Timestamp,
                BlockNumber = timestampRequest.BlockNumber,
                FileHash = timestampRequest.FileHash,
                Signature = timestampRequest.Signature,
                Nonce = timestampRequest.Nonce,
                Network = timestampRequest.Network,
                UserId = timestampRequest.UserId,
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
                PaymentCustomerId = user.PaymentCustomerId,
                SetupIntentId = user.SetupIntentId,
                Address = user.Address.ToResponse(),
                RemainingTimeStamps = user.RemainingTimeStamps,
                PricePlanId = user.CurrentPricePlanId,
                PaymentIntentId = user.PaymentIntentId,
                MembershipStartDate = user.MembershipStartDate
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
                Hash = timestamp.Hash,
                Timestamp = timestamp.Timestamp,
                BlockNumber = timestamp.BlockNumber,
                FileHash = timestamp.FileHash,
                Signature = timestamp.Signature,
                Nonce = timestamp.Nonce,
                Network = timestamp.Network,
                UserId = timestamp.UserId,
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
                ConfirmationDescription = pricePlan.ConfirmationDescription,
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