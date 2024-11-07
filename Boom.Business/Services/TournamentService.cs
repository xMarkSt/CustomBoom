using System.Collections;
using System.Reflection;
using Boom.Common.DTOs;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Claunia.PropertyList;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class TournamentService : ITournamentService
{
    // todo move to service
    private readonly BoomDbContext _context;

    public TournamentService(BoomDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Get the currently scheduled tournament group
    /// </summary>
    /// <returns></returns>
    public async Task<TournamentGroup?> GetScheduled()
    {
        return await _context.TournamentGroups
            .Where(x => x.EndsAt > DateTime.Now)
            .OrderBy(x => x.EndsAt)
            .FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Serialize a dto class to a NSDictionary
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public NSDictionary SerializeTest(IPlistSerializable dto)
    {
        var dict = new Dictionary<string, object>();
        foreach(PropertyInfo prop in dto.GetType().GetProperties())
        {
            var val = prop.GetValue(dto, null);
            if(val == null) continue;
            if (prop.PropertyType.IsAssignableTo(typeof(IPlistSerializable)))
            {
                dict.Add(prop.Name, SerializeTest((IPlistSerializable)val));
            }
            else if (prop.PropertyType.IsAssignableTo(typeof(Guid)))
            {
                dict.Add(prop.Name, val.ToString());
            }
            else if (prop.PropertyType.IsAssignableTo(typeof(IList)) && val.GetType().GetGenericArguments().FirstOrDefault().IsAssignableTo(typeof(IPlistSerializable)))
            {
                var newList = new NSArray();
                foreach (object? listItem in (IList)val)
                {
                    newList.Add(SerializeTest((IPlistSerializable)listItem));
                }
                dict.Add(prop.Name, newList);
            }
            
            else
            {
                dict.Add(prop.Name, val);
            }
        }

        var wrap = NSDictionary.Wrap(dict);
        return wrap;
    }
}