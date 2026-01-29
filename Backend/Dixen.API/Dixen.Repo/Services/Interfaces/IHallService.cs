using Dixen.Repo.DTOs.Hall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixen.Repo.Services.Interfaces
{
    public interface IHallService
    {
        Task<List<HallResponse>> GetAllHallsAsync();
        Task<HallResponse?> GetByIdAsync(int id);
        Task<List<HallResponse>> GetHallsByEventAsync(int eventId);
        Task<HallResponse> CreateHallAsync(int eventId, HallDto dto);
        Task<HallResponse?> UpdateHallAsync(int hallId, HallDto dto);
        Task<bool> DeleteHallAsync(int hallId);
    }

}
