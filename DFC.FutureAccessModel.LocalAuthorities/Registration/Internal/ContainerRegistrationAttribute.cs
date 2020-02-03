using System;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    internal abstract class ContainerRegistrationAttribute : Attribute
    {
        public Type ContractType { get; }
        public Type ImplementationType { get; }
        public TypeOfRegistrationScope Scope { get; }

        protected ContainerRegistrationAttribute(Type contractType, Type implementationType, TypeOfRegistrationScope scope)
        {
            ContractType = contractType;
            ImplementationType = implementationType;
            Scope = scope;
        }
    }
}
