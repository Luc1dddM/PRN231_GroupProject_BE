﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Dtos
{
    public class AddRoleDto
    {
        public required string name { get; set; }
        public string[]? permissions { get; set; }
    }
}
