using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace microservice_sketch.Models
{
    public class user_role
    {
        [Key]
        public int id { get; set; }

        [ForeignKey(nameof(user))]
        public int user_id { get; set; }

        public string role_type { get; set; }

        public virtual user user { get; set; }
    }
}
