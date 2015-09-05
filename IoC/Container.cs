using System;
using System.Collections.Generic;
using System.Reflection;

namespace Svelto.IoC
{
	public class Container: IContainer, IInternalContainer
	{
        /// <summary>
	    /// Use this class to register an interface
	    /// or class into the container.
	    /// </summary>
	    private sealed class Binder<Contractor>: IBinder<Contractor> where Contractor:class
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

            public void AsSingle<T>(T instance) where T : class, Contractor
		    {
                _container.Register<T, SelfProvider<T>>(_interfaceType, new SelfProvider<T>(instance));
		    }

            public void ToFactory<T>(IProvider<T> provider) where T:class, Contractor
		    {
			    _container.Register<T, IProvider<T>>(_interfaceType, provider);
		    }
		
		    IInternalContainer  _container;
            Type                _interfaceType;
	    }

		public Container()
		{
			_providers = new Dictionary<Type, IProvider>();
			_uniqueInstances = new Dictionary<Type, object>();
            _cachedProperties = new Dictionary<Type, MemberInfo[]>();
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
				
			if (_providers.ContainsKey(type))
				_providers.Remove(type);
			
			if (_uniqueInstances.ContainsKey(type))
				_uniqueInstances.Remove(type);
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

        public void Register<T, K>(System.Type type, K provider) where K:IProvider<T>
		{
			_providers[type] = provider;
		}

        //
		// protected Members
		//

        protected object Get(Type contract) 
		{	
			if (_providers.ContainsKey(contract) == true)
			{	
				IProvider provider = _providers[contract];
				
				if (_uniqueInstances.ContainsKey(provider.contract) == false)
					return CreateDependency(provider, contract);
				else
				{
					object instance = _uniqueInstances[provider.contract];
				
					return instance; 	
				}
			}

			return null;
		}

        /// <summary>
        /// Users can define their own IBinder and override this function to use it
        /// </summary>
        /// <typeparam name="TContractor"></typeparam>
        /// <returns></returns>

        virtual protected IBinder<TContractor> BinderProvider<TContractor>() where TContractor:class
		{
			return new Binder<TContractor>();
		}

        /// <summary>
        /// Called when an instance is first built. Useful to add new Container behaviours,
        /// but not needed for the final user since OnDependenciesInjected must be used instead.
        /// </summary>
        /// <typeparam name="TContractor"></typeparam>
        /// <param name="instance"></param>

        virtual protected void OnInstanceGenerated<TContractor>(TContractor instance) where TContractor : class
        {}

        //
        // Private Members
        //

        IBinder<TContractor> InternalBind<TContractor>() where TContractor : class
        {
            IBinder<TContractor> binder = BinderProvider<TContractor>();

            binder.Bind<TContractor>(this);

            return binder;
        }

        object Get(Type contract, Type containerContract) 
		{
            IProvider provider = null;

            if (_providers.TryGetValue(contract, out provider) == true)
			{
                object instance;
                //get the provider linked to the contract
                //N.B. several contracts could be linked
                //to the provider of the same class

                //contract (left side) and provider.contract (right side) are different!
                if (_uniqueInstances.TryGetValue(provider.contract, out instance) == false)
					return CreateDependency(provider, containerContract);
				else
					return instance; 	
			}

			return null;
		}

		object CreateDependency(IProvider provider, Type containerContract)
		{
			object obj = provider.Create(containerContract);
	
            if (provider.single == true)
			    _uniqueInstances[provider.contract] = obj; //seriously, this must be done before obj is injected to avoid circular dependencies
			
			InternalInject(obj);

            OnInstanceGenerated(obj);

            return obj;
		}

        void InternalInject(object injectable)
        {
            DesignByContract.Check.Require(injectable != null);

            Type contract = injectable.GetType();
            Type injectAttributeType = typeof (InjectAttribute);

            MemberInfo[] properties = null;

            if (_cachedProperties.TryGetValue(contract, out properties) == false)
            {
                properties = contract.FindMembers(MemberTypes.Property,
                                                        BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                                        DelegateToSearchCriteria, injectAttributeType);

                _cachedProperties[contract] = properties;
            }

            for (int i = 0; i < properties.Length; i++)
                InjectProperty(injectable, properties[i] as PropertyInfo, contract);

            try
            {
                //transform in [tag] instead to use an interface
                if (injectable is IInitialize)
                    (injectable as IInitialize).OnDependenciesInjected();
            }
            catch (Exception e)
            {
                throw new Exception("OnDependenciesInjected Crash", e);
            }
        }
        private void CallInjection(object injectable, MethodInfo info, Type contract)
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

        static bool DelegateToSearchCriteria(MemberInfo objMemberInfo, Object objSearch)
        {
            return objMemberInfo.IsDefined((Type)objSearch, true);
        }

	    void InjectProperty(object injectable, PropertyInfo info, Type contract)
	    {
	        if (info.PropertyType == typeof (IContainer)) //self inject
	            info.SetValue(injectable, this, null);
	        else
	        {
	            object valueObj = Get(info.PropertyType, contract);

	            //inject in Injectable the valueObj
	            if (valueObj != null)
	                info.SetValue(injectable, valueObj, null);
	        }
	    }

	     Dictionary<Type, IProvider> 		_providers;
		 Dictionary<Type, object> 			_uniqueInstances; //should it be weak reference?
	     Dictionary<Type, MemberInfo[]>     _cachedProperties;
	}
}
