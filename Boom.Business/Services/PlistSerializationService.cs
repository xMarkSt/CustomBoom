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
            switch (val)
            {
                case null:
                    continue;
                case IPlistSerializable serializable:
                    // Recursively serialize this member
                    dict.Add(name, SerializeToNSDictionary(serializable));
                    break;
                case Guid guid:
                    dict.Add(name, guid.ToString());
                    break;
                case IList list:
                    
                    // Serialize each list item into an NSArray and add it to the dictionary
                    var newList = new NSArray();
                    foreach (var listItem in list)
                    {
                        if (listItem is IPlistSerializable item)
                        {
                            newList.Add(SerializeToNSDictionary(item));
                        }
                        else
                        {
                            // Use default serialization
                            newList.Add(listItem);
                        }
                    }

                    dict.Add(name, newList);
                    break;
                // Add rest of types as-is to be handled by plist-cil
                default:
                {
                    dict.Add(name, val);
                    break;
                }
            }
        }

        NSDictionary? wrap = NSObject.Wrap(dict);
        return wrap;
    }
}