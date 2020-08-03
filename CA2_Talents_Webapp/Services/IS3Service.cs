using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CA2_Talents_Webapp.Services
{
    public interface IS3Service
    {
        Task UploadFileAsync(string path);
    }
}
