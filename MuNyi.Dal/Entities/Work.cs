using MuNyi.Dal.Entities.Authentication;
using System;

namespace MuNyi.Dal.Entities
{
    public class Work
    {
        public Guid Id { get; set; }
        public double Time { get; set; }
        public string Comment { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Task Task { get; set; }
        public Guid TaskId { get; set; }
    }
}