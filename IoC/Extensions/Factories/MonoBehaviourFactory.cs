using System;

namespace Svelto.IoC
{
    public class MonoBehaviourFactory : Context.MonoBehaviourFactory
    {
        public MonoBehaviourFactory(IContainer container):base()
        {
            _container = container;
        }

        override public M Build<M>(Func<M> constructor)
        {
            var copy =  base.Build(constructor);
            
            _container.Inject(copy);

            return copy;
        }
 
        IContainer _container;
    }
}
