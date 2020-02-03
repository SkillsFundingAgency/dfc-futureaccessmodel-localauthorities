using System;
using System.Collections.Generic;
using System.Reflection;
using DFC.FutureAccessModel.LocalAuthorities.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace DFC.FutureAccessModel.LocalAuthorities.Registration.Internal
{
    /// <summary>
    /// service registration provider
    /// </summary>
    internal sealed class ServiceRegistrationProvider :
        IRegisterServices
    {
        /// <summary>
        /// the action map
        /// </summary>
        internal Dictionary<TypeOfRegistrationScope, Action<IServiceCollection, ContainerRegistrationAttribute>> ActionMap { get; }

        /// <summary>
        /// the (full) list of registrations
        /// </summary>
        internal List<ContainerRegistrationAttribute> Registrations { get; } = new List<ContainerRegistrationAttribute>();

        /// <summary>
        /// instantiates an instance of <see cref="ServiceRegistrationProvider"/>
        /// </summary>
        /// <param name="theRegistrant">for the registrant</param>
        public ServiceRegistrationProvider(Assembly theRegistrant)
        {
            It.IsNull(theRegistrant)
                .AsGuard<ArgumentNullException>(nameof(theRegistrant));

            ActionMap = new Dictionary<TypeOfRegistrationScope, Action<IServiceCollection, ContainerRegistrationAttribute>>()
            {
                [TypeOfRegistrationScope.Scoped] = AddScoped,
                [TypeOfRegistrationScope.Transient] = AddTransient,
                [TypeOfRegistrationScope.Singleton] = AddSingleton,
            };

            var inherited = theRegistrant
                .GetCustomAttributes<ExternalRegistrationAttribute>()
                .AsSafeReadOnlyList();
            var local = theRegistrant
                .GetCustomAttributes<InternalRegistrationAttribute>()
                .AsSafeReadOnlyList();

            Registrations.AddRange(local);
            Registrations.AddRange(inherited);
        }

        /// <summary>
        /// compose...
        /// </summary>
        /// <param name="usingCollection">using (the) service collection</param>
        public void Compose(IServiceCollection usingCollection) =>
            Registrations.ForEach(x => ActionMap[x.Scope].Invoke(usingCollection, x));

        /// <summary>
        /// add scoped (item)
        /// </summary>
        /// <param name="usingCollection">using (the) service collection</param>
        /// <param name="andRegistration">and registration</param>
        internal void AddScoped(IServiceCollection usingCollection, ContainerRegistrationAttribute andRegistration) =>
            usingCollection.AddScoped(andRegistration.ContractType, andRegistration.ImplementationType);

        /// <summary>
        /// add transient (item)
        /// </summary>
        /// <param name="usingCollection">using (the) service collection</param>
        /// <param name="andRegistration">and registration</param>
        internal void AddTransient(IServiceCollection usingCollection, ContainerRegistrationAttribute andRegistration) =>
            usingCollection.AddTransient(andRegistration.ContractType, andRegistration.ImplementationType);

        /// <summary>
        /// add singleton (item)
        /// </summary>
        /// <param name="usingCollection">using (the) service collection</param>
        /// <param name="andRegistration">and registration</param>
        internal void AddSingleton(IServiceCollection usingCollection, ContainerRegistrationAttribute andRegistration) =>
            usingCollection.AddSingleton(andRegistration.ContractType, andRegistration.ImplementationType);

        /// <summary>
        /// </summary>
        /// <returns>i register services</returns>

        /// <summary>
        /// create (a registrar) service
        /// a chicken and egg situation...
        /// </summary>
        /// <param name="forTheRegistrant">for the registrant</param>
        /// <returns>i register services</returns>
        public static IRegisterServices CreateService(Assembly forTheRegistrant) =>
            new ServiceRegistrationProvider(forTheRegistrant);
    }
}
