using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ApplicationCore.Utilities
{
    public static class ExpressionUtils
    {
        static MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs,
    Type[] argTypes, BindingFlags flags)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));

            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        static bool IsIEnumerable(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        static Type GetIEnumerableImpl(Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsIEnumerable(type))
                return type;
            Type[] t = type.FindInterfaces((m, o) => IsIEnumerable(m), null);
            return t[0];
        }

        public static Expression CallAny(Expression collection, Expression predicateExpression, string method_name)
        {
            Type cType = GetIEnumerableImpl(collection.Type);
            //collection = Expression.Convert(collection, cType);

            Type elemType = cType.GetGenericArguments()[0];
            //Type predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));

            // Enumerable.Any<T>(IEnumerable<T>, Func<T,bool>)
            //MethodInfo method = (MethodInfo)
            //    GetGenericMethod(typeof(Enumerable), method_name, new[] { elemType },
            //        new[] { cType, predType }, BindingFlags.Static);

            var method = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                      .First(x => x.Name == method_name && x.GetParameters().Length == 2)
                                                      .MakeGenericMethod(elemType);

            return Expression.Call(
                method,
                    collection,
                    predicateExpression);
        }
    }
}
