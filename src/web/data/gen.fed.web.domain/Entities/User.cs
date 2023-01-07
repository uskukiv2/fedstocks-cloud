using System;
using System.ComponentModel.DataAnnotations;
using gen.fed.web.domain.Abstract;

namespace gen.fed.web.domain.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }

        public string AuthenticationId { get; set; }
        
        public int InternalId { get; set; }

        public Guid RoleId { get; set; }
    }
}