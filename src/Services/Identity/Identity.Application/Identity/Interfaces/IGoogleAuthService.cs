using Identity.Application.DTOs;
using Identity.Application.Utils;
using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<BaseResponse<User>> GoogleSignIn(GoogleSignInVM model);
    }
}
