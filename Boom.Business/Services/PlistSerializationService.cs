using System.Collections;
using System.Reflection;
using Boom.Common;
using Boom.Common.DTOs;
using Boom.Common.Extensions;
using Boom.Common.Serialization;
using Claunia.PropertyList;

namespace Boom.Business.Services;

public class PlistSerializationService : IPlistSerializationService
{
    public string ToPlistString(IPlistSerializable dto)
    {
        return SerializeToNSDictionary(dto).ToXmlPropertyList();
    }
    
    /// <summary>
    /// Serialize a dto type class to a NSDictionary.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public NSDictionary SerializeToNSDictionary(IPlistSerializable dto)
    {
        // Some responses (e.g. tournament results) have no fixed set of properties: the plist
        // root is a dictionary keyed by a runtime value (a tournament uuid). Those are modeled
        // as an IDictionary rather than reflected-over properties.
        if (dto is IDictionary dtoDictionary)
        {
            var dynamicDict = new Dictionary<string, object>();
            foreach (DictionaryEntry entry in dtoDictionary)
            {
                var value = ConvertToPlistCompatibleType(entry.Value);
                if (value != null)
                    dynamicDict[entry.Key.ToString()!] = value;
            }

            return (NSDictionary)NSObject.Wrap(dynamicDict);
        }

        var dict = new Dictionary<string, object>();

        // Use reflection to convert each member to a type compatible with plist-cil.
        foreach (PropertyInfo prop in dto.GetType().GetProperties())
        {
            object? val = prop.GetValue(dto, null);
            
            object? objToAdd = ConvertToPlistCompatibleType(val);
            if (objToAdd != null)
            {
                var customPropertyName = prop.GetCustomAttribute<PlistPropertyNameAttribute>();
                string name = customPropertyName == null ? prop.Name.ToSnakeCase() : customPropertyName.Name;
                
                dict.Add(name, objToAdd);
            }
        }

        // Use plist-cil to convert Dictionary to NSDictionary.
        var nsDict = NSObject.Wrap(dict);
        return nsDict;
    }

    /// <summary>
    /// Convert the object to a type that is compatible with plist-cil.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private object? ConvertToPlistCompatibleType(object? obj)
    {
        switch (obj)
        {
            case IPlistSerializable serializable:
                return SerializeToNSDictionary(serializable);
            case Guid guid:
                return guid.ToString();
            case IList list:
                // Convert list type to NSArray. 
                var newList = new NSArray();
                foreach (var listItem in list)
                {
                    newList.Add(ConvertToPlistCompatibleType(listItem));
                }
                return newList;
            default:
                // Return type as-is so it can be handled by plist-cil.
                return obj;
        }
    }
}