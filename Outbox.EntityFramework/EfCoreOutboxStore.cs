using Microsoft.EntityFrameworkCore;
using Outbox.Abstractions.Dtos;
using Outbox.Stores;

namespace Outbox.EntityFramework;

public class EfCoreOutboxStore : IOutboxStore
{
    private readonly EfCoreOutboxContext _context;

    public EfCoreOutboxStore(EfCoreOutboxContext context)
    {
        _context = context;
    }

    public async Task Add(OutboxMessage message)
    {
        await _context.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(IEnumerable<Guid> ids)
    {
        var messages = await _context.OutboxMessages.Where(message => ids.Contains(message.Id)).ToListAsync();
        _context.RemoveRange(messages);

        await _context.SaveChangesAsync();
    }

    public async Task<OutboxMessage> GetMessage(Guid id)
    {
        var query = from message in _context.OutboxMessages
                    where message.Id == id
                    select message;

        var result = await query.AsNoTracking().FirstAsync();

        return result;
    }

    public async Task<IEnumerable<Guid>> GetUnprocessedMessageIds()
    {
        var query = from message in _context.OutboxMessages
                    where !message.Processed.HasValue
                    select message.Id;

        var result = await query.ToListAsync();

        return result;
    }

    public async Task SetMessageToProcessed(Guid id)
    {
        var message = new OutboxMessage
        {
            Id = id,
            Processed = DateTime.UtcNow
        };

        _context.OutboxMessages.Attach(message);
        _context.Entry(message).Property(p => p.Processed).IsModified = true;

        await _context.SaveChangesAsync();
    }
}