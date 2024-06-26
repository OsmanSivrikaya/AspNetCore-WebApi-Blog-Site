﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyBlogSite.Entity.Abstract;

namespace MyBlogSite.Entity
{
    public class Ticket : BaseEntity
    {
        [Required]
        [Column("ticket_name", TypeName = "nvarchar(255)")]
        public required string TicketName { get; set; }
    }
}
