namespace Vurdalakov
{
    using System;

    public static class TypeExtensions
    {
        public static Boolean HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.IsDefined(typeof(TAttribute), false);
        }
    }
}
