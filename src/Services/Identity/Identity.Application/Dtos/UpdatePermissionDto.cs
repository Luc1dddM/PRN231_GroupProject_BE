using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Dtos
{
    public class UpdatePermissionDto
    {
        public required string role {get; set;}
        public List<int>? permissionIds { get; set; }   
    }
}