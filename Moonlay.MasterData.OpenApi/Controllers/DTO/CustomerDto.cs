using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Moonlay.MasterData.OpenApi.Controllers.DTO
{
    [DataContract]
    public class CustomerDto
    {
        public CustomerDto(Customer entity)
        {
            if (entity is null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }

            this.Id = entity.Id;
            this.FirstName = entity.FirstName;
            this.LastName = entity.LastName;

            this.CreatedAt = entity.CreatedAt;
            this.CreatedBy = entity.CreatedBy;
            this.UpdatedAt = entity.UpdatedAt;
            this.UpdatedBy = entity.UpdatedBy;

        }

        [JsonProperty("id")]
        public Guid Id { get; }

        [JsonProperty("first_name")]
        public string FirstName { get; }

        [JsonProperty("last_name")]
        public string LastName { get; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; }

        [JsonProperty("created_by")]
        public string CreatedBy { get; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; }

        [JsonProperty("updated_by")]
        public string UpdatedBy { get; }
    }

    public class Customer
    {

        public Customer(Guid guid)
        {
            this.Id = guid;
        }

        public string LastName { get; internal set; }
        public string FirstName { get; internal set; }
        public DateTimeOffset CreatedAt { get; internal set; }
        public Guid Id { get; internal set; }
        public DateTimeOffset UpdatedAt { get; internal set; }
        public string CreatedBy { get; internal set; }
        public string UpdatedBy { get; internal set; }
    }
}