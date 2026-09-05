using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace FitMaster.Infrastructure.Persistence.Conversions;


public static class EnumListJsonConverter
{
    public static void ConfigureAsJson<TEnum>(this PropertyBuilder<List<TEnum>> builder, string columnName)
        where TEnum : struct, Enum
    {
        var converter = new ValueConverter<List<TEnum>, string>(
            list => JsonSerializer.Serialize(list, (JsonSerializerOptions?)null),
            json => JsonSerializer.Deserialize<List<TEnum>>(json, (JsonSerializerOptions?)null) ?? new List<TEnum>());

        var comparer = new ValueComparer<List<TEnum>>(
            (a, b) => (a ?? new()).SequenceEqual(b ?? new()),
            list => list.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            list => list.ToList());

        builder
            .HasColumnName(columnName)
            .HasConversion(converter)
            .Metadata.SetValueComparer(comparer);
    }
}
