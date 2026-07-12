using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Boom.UnitTests;

/// <summary>
/// Builds an <see cref="IMapper"/> for tests. AutoMapper 14+ requires an
/// <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/> on the
/// <see cref="MapperConfiguration"/> constructor; the license key (AutoMapper 15+)
/// is read from the AutoMapper__LicenseKey environment variable when present so it
/// is never hardcoded in the test source.
/// </summary>
internal static class TestMapperFactory
{
    public static IMapper Create(params Profile[] profiles)
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            var licenseKey = Environment.GetEnvironmentVariable("AutoMapper__LicenseKey");
            if (!string.IsNullOrEmpty(licenseKey))
            {
                cfg.LicenseKey = licenseKey;
            }

            foreach (var profile in profiles)
            {
                cfg.AddProfile(profile);
            }
        }, NullLoggerFactory.Instance);

        return new Mapper(configuration);
    }
}
