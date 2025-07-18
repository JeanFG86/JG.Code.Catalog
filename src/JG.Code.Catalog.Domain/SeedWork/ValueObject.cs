﻿namespace JG.Code.Catalog.Domain.SeedWork;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract bool Equals(ValueObject? other);
    
    protected abstract int GetCustomHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ValueObject)obj);
    }

    public override int GetHashCode() => GetCustomHashCode();

    public static bool operator ==(ValueObject? left, ValueObject? right) => left?.Equals(right) ?? false;
    
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}