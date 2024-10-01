using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Dtos
{
    public class UpdatePermissionDto
    {
        public required string Role { get; set; }
        public List<int>? PermissionIds { get; set; }
    }
}