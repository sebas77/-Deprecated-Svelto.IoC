using System;

namespace Svelto.IoC
{
	public interface IProvider
	{
		object Create(Type containerContract);
		
		Type    contract { get; }
        bool    single   { get; }
	}
	
	public interface IProvider<T>:IProvider
	{}
	
	public class StandardProvider<T>:IProvider<T> where T:new()
	{
		public object Create(Type containerContract)
		{
			return new T();
		}
		
		public Type contract { get { return typeof(T); } }
        public bool single          { get { return true; } }
	}

    public class SelfProvider<T>:IProvider<T> 
	{
        public SelfProvider(T instance)
        {
            _instance = instance;
        }

		public object Create(Type containerContract)
		{
			return _instance;
		}
		
		public Type contract { get { return typeof(T); } }
        public bool single          { get { return true; } }

        T           _instance; //should it be weak reference?
	}

    public class MultiProvider<T>:IProvider<T> where T:new()
	{
        public object Create(Type containerContract)
        {
            return new T();
        }

        public Type contract { get { return typeof(T); } }
        public bool single   { get { return false; } }
	}
}

