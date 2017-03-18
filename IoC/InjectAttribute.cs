using System;

namespace Svelto.IoC
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class InjectAttribute: Attribute
	{
        public string name { get; set; }
	}
}
