using Microsoft.EntityFrameworkCore;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
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
        public async Task AddComponentAsync(int frameId, IBaseEquip component, float? length = null)
        {
            var frame = await _context.FormedFrames
                .Include(f => f.Components)
                .FirstOrDefaultAsync(f => f.Id == frameId);

            if (frame == null || component == null)
                return;

            string type = component.GetType().Name;

            var exisingComponent = frame.Components
                .FirstOrDefault(fc => fc.ComponentId == component.Id && fc.ComponentType == type);

            if (exisingComponent != null)
            {
                if (component is BaseFrame baseFrame && baseFrame.Measure == "м" && length.HasValue)
                    exisingComponent.Length += length.Value;
                else
                    exisingComponent.Count++;
                _context.FrameComponents.Update(exisingComponent);
            }
            else
            {
                var newComponent = new FrameComponent
                {
                    FormedFrameId = frameId,
                    ComponentId = component.Id,
                    ComponentType = type,
                    Count = (component is BaseFrame baseFrame && baseFrame.Measure == "м") ? 0 : 1,
                    Length = (component is BaseFrame baseFrame2 && baseFrame2.Measure == "м") ? length : null
                };
                await _context.FrameComponents.AddAsync(newComponent);
            }

            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(FormedFrame entity)
        {
            if (entity != null)
            {
                // Удаляем все компоненты, связанные с рамой
                var components = _context.FrameComponents.Where(fc => fc.FormedFrameId == entity.Id);
                _context.FrameComponents.RemoveRange(components);

                _context.Set<FormedFrame>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
            var frame = await GetByIdAsync(id);
            if (frame != null)
            {
                await DeleteAsync(frame);
                return 1;
            }
            return 0;
        }
        public async Task<IEnumerable<FormedFrame>> GetAllAsync()
        {
            return await _context.FormedFrames
                .Include(f => f.Components)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<FormedFrame> GetByIdAsync(int id)
        {
            return await _context.FormedFrames
                .Include(f => f.Components)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task UpdateAsync(FormedFrame entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveComponentAsync(int frameId, IBaseEquip component)
        {
            var frameComponent = await _context.FrameComponents
                .FirstOrDefaultAsync(fc => fc.FormedFrameId == frameId
                                           && fc.ComponentId == component.Id
                                           && fc.ComponentType == component.GetType().Name);

            if (frameComponent != null)
            {
                if (frameComponent.Count > 1)
                {
                    frameComponent.Count--;
                    _context.FrameComponents.Update(frameComponent);
                }
                else
                {
                    _context.FrameComponents.Remove(frameComponent);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
