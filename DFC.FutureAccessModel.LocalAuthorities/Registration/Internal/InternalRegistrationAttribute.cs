using System;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    internal sealed class InternalRegistrationAttribute :
        ContainerRegistrationAttribute
    {
        public InternalRegistrationAttribute(Type contractType, Type implementationType, TypeOfRegistrationScope scope) :
            base(contractType, implementationType, scope)
        {
            (!ContractType.IsAssignableFrom(ImplementationType))
                .AsGuard<ArgumentException>(ImplementationType.Name);
            (!typeof(ISupportServiceRegistration).IsAssignableFrom(ContractType))
                .AsGuard<ArgumentException>(ContractType.Name);
        }
    }
}
