using System;
namespace DFC.FutureAccessModel.LocalAuthorities.Storage
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PartitionKeyAttribute :
        Attribute
    {
        public PartitionKeyAttribute()
        {
        }
    }
}
