﻿using System.Collections.Generic;

namespace AspNetIdentity.WebApi.Models
{
    public class UsersInRoleDto
    {
        public string Id { get; set; }
        public List<string> EnrolledUsers { get; set; }
        public List<string> RemovedUsers { get; set; }
    }
}