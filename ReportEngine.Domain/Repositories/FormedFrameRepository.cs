using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Domain.Repositories
{
    public class FormedFrameRepository : IFrameRepository
    {
        private readonly ReAppContext _context;

        public FormedFrameRepository(ReAppContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FormedFrame entity)
        {
            await _context.Set<FormedFrame>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddComponentAsync(int frameId, IBaseEquip component)
        {
            var frame = await GetByIdAsync(frameId);
            if (frame == null)
                return;

            switch (component)
            {
                case FrameDetail frameDetail:
                    frame.FrameDetails.Add(frameDetail);
                    break;
                case FrameRoll frameRoll:
                    frame.FrameRolls.Add(frameRoll);
                    break;
                case PillarEqiup pillarEqiup:
                    frame.PillarEqiups.Add(pillarEqiup);
                    break;
                default:
                    throw new ArgumentException($"Не поддерживаемый компонент: {component.GetType().Name}");
            }

            await _context.SaveChangesAsync();
            
        }
        public Task DeleteAsync(FormedFrame entity)
        {
            if (entity != null)
            {
                _context.Set<FormedFrame>().Remove(entity);
                return _context.SaveChangesAsync();
            }
            return null;
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
            var frame = await GetByIdAsync(id);
            if(frame != null)
            {
                _context.Set<FormedFrame>().Remove(frame);
                return await _context.SaveChangesAsync();
                
            }
            return 0;
        }
        public async Task<IEnumerable<FormedFrame>> GetAllAsync()
        {
            return await _context.Set<FormedFrame>()
                        .Include(f => f.FrameDetails)
                        .Include(f => f.FrameRolls)
                        .Include(f => f.PillarEqiups)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<FormedFrame> GetByIdAsync(int id)
        {
            return await _context.Set<FormedFrame>()
                        .Include(f => f.FrameDetails)
                        .Include(f => f.FrameRolls)
                        .Include(f => f.PillarEqiups)
                        .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task UpdateAsync(FormedFrame entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
