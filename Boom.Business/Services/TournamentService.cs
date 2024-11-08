using System.Collections;
using System.Reflection;
using AutoMapper;
using Boom.Common;
using Boom.Common.DTOs;
using Boom.Common.Extensions;
using Boom.Infrastructure.Data;
using Boom.Infrastructure.Data.Entities;
using Claunia.PropertyList;
using Microsoft.EntityFrameworkCore;

namespace Boom.Business.Services;

public class TournamentService : ITournamentService
{
    private readonly IRepository _repository;
    private readonly IMapper _mapper;

    public TournamentService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Get the currently scheduled tournament group
    /// </summary>
    /// <returns></returns>
    public async Task<ScheduleDto> GetScheduled()
    {
        var current = await _repository.GetAll<TournamentGroup>()
            // .Where(x => x.EndsAt > DateTime.Now) // For testing
            .OrderBy(x => x.EndsAt)
            .FirstOrDefaultAsync();
        return new ScheduleDto()
        {
            Schedule = new List<TournamentGroupDto>()
            {
                _mapper.Map<TournamentGroupDto>(current)
            }
        };
    }
    
    /// <summary>
    /// Serialize a dto type class to a NSDictionary
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public NSDictionary SerializeToNSDictionary(IPlistSerializable dto)
    {
        var dict = new Dictionary<string, object>();
        foreach(PropertyInfo prop in dto.GetType().GetProperties())
        {
            object? val = prop.GetValue(dto, null);
            var name = prop.Name.ToSnakeCase();
            if(val == null) continue;
            if (prop.PropertyType.IsAssignableTo(typeof(IPlistSerializable)))
            {
                // Recursively serialize this member
                dict.Add(name, SerializeToNSDictionary((IPlistSerializable)val));
            }
            else if (prop.PropertyType.IsAssignableTo(typeof(Guid)))
            {
                dict.Add(name, val.ToString());
            }
            // If list of IPlistSerializable
            else if (prop.PropertyType.IsAssignableTo(typeof(IList)) && val.GetType().GetGenericArguments().FirstOrDefault().IsAssignableTo(typeof(IPlistSerializable)))
            {
                // Serialize each list item into an NSArray and add it to the dictionary
                var newList = new NSArray();
                foreach (object? listItem in (IList)val)
                {
                    newList.Add(SerializeToNSDictionary((IPlistSerializable)listItem));
                }
                dict.Add(name, newList);
            }
            
            // Add rest of types as-is to be handled by plist-cil
            else
            {
                dict.Add(name, val);
            }
        }

        NSDictionary? wrap = NSObject.Wrap(dict);
        return wrap;
    }
}