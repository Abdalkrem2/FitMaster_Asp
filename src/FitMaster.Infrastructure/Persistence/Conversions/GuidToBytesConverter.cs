using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitMaster.Infrastructure.Persistence.Conversions;

public class GuidToBytesConverter : ValueConverter<Guid, byte[]>
{
    public GuidToBytesConverter()
        : base(
            guid => guid.ToByteArray(),
            bytes => new Guid(bytes))
    {
    }
}
