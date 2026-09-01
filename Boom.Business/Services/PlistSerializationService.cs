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
            case IDictionary dictionary:
                // Convert dictionary type to NSDictionary, keyed by the dictionary's own keys.
                var newDict = new Dictionary<string, object>();
                foreach (DictionaryEntry entry in dictionary)
                {
                    var objToAdd = ConvertToPlistCompatibleType(entry.Value);
                    if (objToAdd != null)
                    {
                        newDict.Add(entry.Key.ToString()!, objToAdd);
                    }
                }
                return NSObject.Wrap(newDict);
            default:
                // Return type as-is so it can be handled by plist-cil.
                return obj;
        }
    }
}