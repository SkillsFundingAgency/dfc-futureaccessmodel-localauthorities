using System;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    internal sealed class ExternalRegistrationAttribute :
        ContainerRegistrationAttribute
    {
        public ExternalRegistrationAttribute(Type contractType, Type implementationType, TypeOfRegistrationScope scope) :
            base(contractType,implementationType,scope)
        {
        }
    }
}
