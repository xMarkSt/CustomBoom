namespace Boom.Common.DTOs;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PlistPropertyNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PlistPropertyNameAttribute"/> with the specified property name.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    public PlistPropertyNameAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The name of the property.
    /// </summary>
    public string Name { get; }
}