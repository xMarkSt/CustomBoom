using Microsoft.AspNetCore.Mvc;

namespace Boom.Api.Filters;

public class EncryptResponseAttribute : TypeFilterAttribute
{
    public EncryptResponseAttribute()
        : base(typeof(EncryptResponseFilter))
    {
    }
}