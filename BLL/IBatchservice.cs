using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IBatchDataService
    {
        Task<BatchCreationResult> CreateBatchDataAsync();
        Task ClearAllDataAsync();
    }

}
