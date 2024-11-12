using System.Collections;
using System.Reflection;
using Boom.Common;
using Boom.Common.DTOs;
using Boom.Common.Extensions;
using Claunia.PropertyList;

namespace Boom.Business.Services;

public class PlistSerializationService : IPlistSerializationService
{
    /// <summary>
    /// Serialize a dto type class to a NSDictionary
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public NSDictionary SerializeToNSDictionary(IPlistSerializable dto)
    {
        var dict = new Dictionary<string, object>();
        foreach (PropertyInfo prop in dto.GetType().GetProperties())
        {
            object? val = prop.GetValue(dto, null);
            var customPropertyName = prop.GetCustomAttribute<PlistPropertyNameAttribute>();
            string name = customPropertyName == null ? prop.Name.ToSnakeCase() : customPropertyName.Name;
            if (val == null) continue;
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
            else if (prop.PropertyType.IsAssignableTo(typeof(IList)) && val.GetType().GetGenericArguments()
                         .FirstOrDefault().IsAssignableTo(typeof(IPlistSerializable)))
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