using System;
using System.Collections.Generic;
using System.Reflection;
using Svelto.DataStructures;

namespace Svelto.IoC
{
    public class Container: IContainer, IInternalContainer
    {
        public Container()
        {
            _providers = ProviderBehaviour();
            _cachedProperties = new Dictionary<Type, MemberInfo[]>();
            _delegateToSearchCriteria = DelegateToSearchCriteria;
        }

        //
        // IContainer interface
        //

        public IBinder<TContractor> Bind<TContractor>() where TContractor:class
        {
            IBinder<TContractor> binder = InternalBind<TContractor>();

            return binder;
        }

        public void BindSelf<TContractor>() where TContractor:class, new()
        {
            IBinder<TContractor> binder = InternalBind<TContractor>();

            binder.AsSingle<TContractor>();
        }

        public TContractor Build<TContractor>() where TContractor:class
        {
            Type contract = typeof(TContractor);

            TContractor instance = Get(contract) as TContractor;

            DesignByContract.Check.Ensure(instance != null, "IoC.Container instance failed to be built (contractor not found - must be registered)");
            
            return instance;
        }

        public void Release<TContractor>() where TContractor:class
        {
            Type type = typeof(TContractor);

            _providers.Remove(type);
        }

        public TContractor Inject<TContractor>(TContractor instance)
        {
            if (instance != null)
                InternalInject(instance);
                
            return instance;
        }

        //
        // IInternalContainer interface
        //

        public void Register<T, K>(Type type, K provider) where K:IProvider<T>
        {
            _providers.Register<T>(type, provider);
        }

        public bool TryGetProvider<T>(out IProvider provider)
        {
            return _providers.Retrieve(typeof(T), out provider);
        }

        protected virtual IProviderContainer ProviderBehaviour()
        {
            return new ProviderContainer();
        }

        /// <summary>
        /// Users can define their own IBinder and override this function to use it
        /// </summary>
        /// <typeparam name="TContractor"></typeparam>
        /// <returns></returns>

        protected virtual IBinder<TContractor> BinderProvider<TContractor>() where TContractor:class
        {
            return new Binder<TContractor>();
        }

        //
        // protected Members
        //

        protected object Get(Type contract) 
        {	
            return CreateDependency(contract, null, null);
        }

        /// <summary>
        /// Called when an instance is first built. Useful to add new Container behaviours,
        /// but not needed for the final user since OnDependenciesInjected must be used instead.
        /// </summary>
        /// <typeparam name="TContractor"></typeparam>
        /// <param name="instance"></param>

        protected virtual void OnInstanceGenerated<TContractor>(TContractor instance) where TContractor : class
        {}
#if TO_COMPLETE
        void CallInjection(object injectable, MethodInfo info, Type contract)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] parameterBuffer = new object[parameters.Length];

            for (int i = parameters.Length - 1; i >= 0; --i)
            {
                ParameterInfo parameter = parameters[i];

                object valueObj = Get(parameter.ParameterType, contract);

                //inject in Injectable the valueObj
                if (valueObj != null)
                    parameterBuffer[i] = valueObj;
            }

            info.Invoke(injectable, parameterBuffer);
        }
#endif
        object CreateDependency(Type contract, Type containerContract, PropertyInfo info)
        {
            IProvider provider = null;

            if (_providers.Retrieve(contract, out provider))
            {
                object obj;

                if (provider.Create(containerContract, info, out obj) == true)
                {
                    InternalInject(obj);
                    OnInstanceGenerated(obj);
                }

                return obj;
            }

            return null;
        }

        static bool DelegateToSearchCriteria(MemberInfo objMemberInfo, object objSearch)
        {
            return objMemberInfo.IsDefined((Type)objSearch, true);
        }

        object InternalGet(Type contract, Type containerContract, PropertyInfo info) 
        {
            return CreateDependency(contract, containerContract, info);
        }

        void InjectProperty(object instanceToFullfill, PropertyInfo info, Type contract)
        {
            if (info.PropertyType == typeof(IContainer)) //self inject
                throw new Exception("Inject containers automatically is considered a design error");
            
             object referenceToInject;

             if (info.PropertyType.IsGenericType == true &&
                 info.PropertyType.GetGenericTypeDefinition() == _weakReferenceType)
             {
                 referenceToInject = InternalGet(info.PropertyType.GetGenericArguments()[0], contract, info);

                 if (referenceToInject != null)
                 {
                     object o = Activator.CreateInstance(info.PropertyType, referenceToInject);
                     
                     info.SetValue(instanceToFullfill, o, null);
                 }
             }
             else
             {
                 referenceToInject = InternalGet(info.PropertyType, contract, info);

                 //inject in Injectable the valueObj
                 if (referenceToInject != null)
                     info.SetValue(instanceToFullfill, referenceToInject, null);
             }
        }

        //
        // Private Members
        //

        IBinder<TContractor> InternalBind<TContractor>() where TContractor : class
        {
            IBinder<TContractor> binder = BinderProvider<TContractor>();

            binder.Bind<TContractor>(this);

            return binder;
        }

        void InternalInject(object instanceToFullfill)
        {
            DesignByContract.Check.Require(instanceToFullfill != null);

            Type contract = instanceToFullfill.GetType();
            Type injectAttributeType = typeof(InjectAttribute);
            
            MemberInfo[] properties = null;

            if (_cachedProperties.TryGetValue(contract, out properties) == false)
            {
               
                properties = contract.FindMembers(MemberTypes.Property,
                                                    BindingFlags.SetProperty | 
                                                    BindingFlags.Public | 
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance,
                                                  _delegateToSearchCriteria, injectAttributeType);

                _cachedProperties[contract] = properties;
            }

            for (int i = 0; i < properties.Length; i++)
                InjectProperty(instanceToFullfill, properties[i] as PropertyInfo, contract);

            try
            {
                var fullfill = instanceToFullfill as IInitialize;
                if (fullfill != null)
                    fullfill.OnDependenciesInjected();
            }
            catch (Exception e)
            {
                Utility.Console.LogException(new Exception("OnDependenciesInjected Crashes inside " + instanceToFullfill.GetType(), e));
            }
        }

        readonly Dictionary<Type, MemberInfo[]>     _cachedProperties;
        readonly MemberFilter                       _delegateToSearchCriteria;
        readonly Type                               _weakReferenceType = typeof(WeakReference<>);

        IProviderContainer                          _providers;

        public interface IProviderContainer
        {
            void Remove(Type type);
            bool Retrieve(Type contract, out IProvider provider);
            void Register<T>(Type type, IProvider<T> provider);
        }

        sealed class ProviderContainer:IProviderContainer
        {
            readonly Dictionary<Type, IProvider> 		_providers;
            readonly Dictionary<Type, IProvider> 		_standardProvidersPerInstanceType;
            public ProviderContainer()
            {
                _providers = new Dictionary<Type, IProvider>();
                _standardProvidersPerInstanceType = new Dictionary<Type, IProvider>();
            }

            public void Remove(Type type)
            {
                _providers.Remove(type);
            }

            public bool Retrieve(Type contract, out IProvider provider)
            {
                return _providers.TryGetValue(contract, out provider);
            }

            public void Register<T>(Type type, IProvider<T> provider)
            {
                var providerType = provider.GetType().GetGenericTypeDefinition();

                if (providerType == _standardProviderType)
                {
                    IProvider standardProvider;
                    var instanceType = typeof(T);
                    if (_standardProvidersPerInstanceType.TryGetValue(instanceType, out standardProvider) == false)
                        standardProvider = _standardProvidersPerInstanceType[instanceType] = new WeakProviderDecorator<T>(provider); //this should be harmless and allows to query for unique providers        

                    provider = ((WeakProviderDecorator<T>) standardProvider).provider;
                }

                _providers[type] = provider; //providers are normally saved by contract, not instance type
            }

            readonly Type  _standardProviderType = typeof(StandardProvider<>);
        }

        /// <summary>
        /// Use this class to register an interface
        /// or class into the container.
        /// </summary>
        sealed class Binder<Contractor>: IBinder<Contractor> where Contractor:class
        {
            public void Bind<ToBind>(IInternalContainer container) where ToBind:class
            {
                _container = container;

                _interfaceType = typeof(ToBind);
            }

            public void AsSingle<T>() where T:Contractor, new()
            {
                _container.Register<T, StandardProvider<T>>(_interfaceType, new StandardProvider<T>());
            }

            public void AsInstance<T>(T instance) where T : class, Contractor
            {
                _container.Register<T, SelfProvider<T>>(_interfaceType, new SelfProvider<T>(instance));
            }

            public void ToProvider<T>(IProvider<T> provider) where T:class, Contractor
            {
                _container.Register<T, IProvider<T>>(_interfaceType, provider);
            }

            IInternalContainer          _container;
            Type                        _interfaceType;
        }
    }
}

//things to do:
//DAG detection warning
//Injection by constructor
//Hierarchical container
//Not found dependency warning
//After 4 injection, add warning about too many injection
