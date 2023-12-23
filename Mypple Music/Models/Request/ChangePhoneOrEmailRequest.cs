using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.Models.Request
{
    public record ChangePhoneOrEmailRequest(string Account, string Token);
}
