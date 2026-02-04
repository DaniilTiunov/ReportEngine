using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories
{
    public class ObvyazkaInStandRepository
    {
        private readonly ReAppContext _context;
        public ObvyazkaInStandRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task AddObvyazkaPurposesAsync()
        {

        }

        public async Task DeleteObvyazkaPurposesAsync(int id)
        {
            var entity = await _context.ObvyazkaAdditionalEquipPurpose
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            _context.ObvyazkaAdditionalEquipPurpose.Remove(entity);

            await _context.SaveChangesAsync();
        }


        public async Task UpdateObvyazkaPurposesAsync(ObvyazkaAdditionalEquipPurpose obvyazka, int obvyazkaInStandId)
        {
            if(obvyazka == null)
                return;

            if(obvyazka.Id == 0)
            {
                obvyazka.ObvyazkaInStandId = obvyazkaInStandId;
                await _context.ObvyazkaAdditionalEquipPurpose.AddAsync(obvyazka);
            }
            else
            {
                obvyazka.ObvyazkaInStandId = obvyazkaInStandId;
                _context.ObvyazkaAdditionalEquipPurpose.Update(obvyazka);
            }

            await _context.SaveChangesAsync();
        }
    }
}
