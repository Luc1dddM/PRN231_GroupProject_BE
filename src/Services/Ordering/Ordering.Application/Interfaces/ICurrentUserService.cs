using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetCurrentUserId();
        void SetCurrentUserId(string userId);
    }
}
