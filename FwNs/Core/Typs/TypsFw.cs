namespace FwNs.Core.Typs
{
    using FwNs.Core;
    using FwNs.FwNs.Core.Typs;
    using FwNs.Linq.Xprsns;
    using FwNs.Txt;
    using Newtonsoft.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml.Linq;

    [Extension]
    public static class TypsFw
    {
        private static readonly ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache = null;
        private static readonly Dictionary<MethodInfo, PropertyInfo> _normalizedMembersCache = new Dictionary<MethodInfo, PropertyInfo>();
        private static readonly MethodInfo _getDefaultValueMethod = Type.GetType("FwNs.Typs.TypsFw").GetMethod("GetDefaultValueImpl", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly Dictionary<Assembly, bool> _isVisibleToLiveLinqCache = new Dictionary<Assembly, bool>();

        [IteratorStateMachine(typeof(<Ancestors>d__76)), Extension]
        internal static IEnumerable<Type> Ancestors(Type type)
        {
            return new <Ancestors>d__76(-2) { <>3__type = type };
        }

        [Extension]
        public static bool AreInternalsVisibleToLiveLinq(Assembly testAssembly)
        {
            Dictionary<Assembly, bool> dictionary = _isVisibleToLiveLinqCache;
            lock (dictionary)
            {
                bool flag3;
                if (!_isVisibleToLiveLinqCache.TryGetValue(testAssembly, out flag3))
                {
                    flag3 = Enumerable.Any<InternalsVisibleToAttribute>(Enumerable.Select(Enumerable.Where(Enumerable.Select(Enumerable.Cast<InternalsVisibleToAttribute>(Attribute.GetCustomAttributes(testAssembly, typeof(InternalsVisibleToAttribute))), <>c.<>9__100_0 ?? (<>c.<>9__100_0 = new Func<InternalsVisibleToAttribute, <>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName>>(<>c.<>9, this.<AreInternalsVisibleToLiveLinq>b__100_0))), <>c.<>9__100_1 ?? (<>c.<>9__100_1 = new Func<<>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName>, bool>(<>c.<>9, this.<AreInternalsVisibleToLiveLinq>b__100_1))), <>c.<>9__100_2 ?? (<>c.<>9__100_2 = new Func<<>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName>, InternalsVisibleToAttribute>(<>c.<>9, this.<AreInternalsVisibleToLiveLinq>b__100_2))));
                    _isVisibleToLiveLinqCache[testAssembly] = flag3;
                }
                return flag3;
            }
        }

        [Extension]
        public static bool AssignableToTypeName(Type type, string fullTypeName)
        {
            Type type2;
            return AssignableToTypeName(type, fullTypeName, out type2);
        }

        [Extension]
        public static bool AssignableToTypeName(Type type, string fullTypeName, out Type match)
        {
            for (Type type2 = type; type2 != null; type2 = type2.BaseType)
            {
                if (string.Equals(type2.FullName, fullTypeName, StringComparison.Ordinal))
                {
                    match = type2;
                    return true;
                }
            }
            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (string.Equals(interfaces[i].Name, fullTypeName, StringComparison.Ordinal))
                {
                    match = type;
                    return true;
                }
            }
            match = null;
            return false;
        }

        [Extension]
        public static string BuildString(Delegate @delegate)
        {
            if (@delegate == null)
            {
                return string.Empty;
            }
            return Sequence.ConcatString<Delegate>(@delegate.GetInvocationList(), <>c.<>9__94_0 ?? (<>c.<>9__94_0 = new Func<Delegate, string>(<>c.<>9, this.<BuildString>b__94_0)), Environment.NewLine);
        }

        [Extension]
        public static bool CanBeDelegateSource(MethodInfo method)
        {
            if (method == null)
            {
                return false;
            }
            return (method.IsStatic || ((method.DeclaringType != null) && !method.DeclaringType.IsValueType));
        }

        [Extension]
        public static bool CanReadMemberValue(MemberInfo member, bool nonPublic)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType != MemberTypes.Property)
                {
                    return false;
                }
            }
            else
            {
                FieldInfo info = (FieldInfo) member;
                return (nonPublic || info.IsPublic);
            }
            PropertyInfo info2 = (PropertyInfo) member;
            if (!info2.CanRead)
            {
                return false;
            }
            return (nonPublic || (info2.GetGetMethod(nonPublic) != null));
        }

        public static TResult ConvertDelegate<TSource, TResult>(TSource source) where TSource: class where TResult: class
        {
            if (typeof(TSource) == typeof(TResult))
            {
                return (source as TResult);
            }
            TypeArgument.IsDelegate<TSource>("TSource");
            TypeArgument.IsDelegate<TResult>("TResult");
            Delegate delegate2 = source as Delegate;
            return (Delegate.CreateDelegate(typeof(TResult), delegate2.Target, delegate2.Method) as TResult);
        }

        [Extension]
        public static TDelegate CreateDelegate<TDelegate>(MethodInfo method) where TDelegate: class
        {
            return CreateDelegate<TDelegate>(method, null);
        }

        [Extension]
        public static TDelegate CreateDelegate<TDelegate>(MethodInfo method, object target) where TDelegate: class
        {
            TypeArgument.IsDelegate<TDelegate>("TDelegate");
            return (Delegate.CreateDelegate(typeof(TDelegate), target, method) as TDelegate);
        }

        [Extension]
        public static T CreateInstance<T>(Type type, params object[] arguments)
        {
            return (T) Activator.CreateInstance(type, arguments);
        }

        [Extension]
        internal static Func<object, object> CreatePropertyGetter(PropertyInfo property)
        {
            ParameterExpression expression = null;
            ParameterExpression[] parameters = new ParameterExpression[] { expression };
            return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(expression, property.DeclaringType), property), typeof(object)), parameters).Compile();
        }

        [Extension]
        internal static Action<object, object> CreatePropertySetter(PropertyInfo property)
        {
            ParameterExpression expression = null;
            ParameterExpression expression2 = null;
            Expression[] arguments = new Expression[] { Expression.Convert(expression2, property.PropertyType) };
            ParameterExpression[] parameters = new ParameterExpression[] { expression, expression2 };
            return Expression.Lambda<Action<object, object>>(Expression.Call(Expression.Convert(expression, property.DeclaringType), property.GetSetMethod(true), arguments), parameters).Compile();
        }

        [Extension]
        public static Type FindGenericType(Type type, Type genericType)
        {
            for (Type type2 = type; type2 != null; type2 = type2.BaseType)
            {
                if (IsOfGenericType(type2, genericType))
                {
                    return type2;
                }
            }
            return null;
        }

        [Extension]
        public static Type[] GenericTypeArguments(Type typ)
        {
            if (typ.IsGenericType && typ.ContainsGenericParameters)
            {
                return typ.GetGenericArguments();
            }
            return new Type[0];
        }

        public static AccessModifier GetAccessModifier(FieldInfo field)
        {
            if (field.IsPublic)
            {
                return AccessModifier.Public;
            }
            if (field.IsPrivate)
            {
                return AccessModifier.Private;
            }
            if (field.IsAssembly)
            {
                return AccessModifier.Internal;
            }
            if (field.IsFamily)
            {
                return AccessModifier.Protected;
            }
            if (field.IsFamilyOrAssembly)
            {
                return AccessModifier.ProtectedOrInternal;
            }
            if (field.IsFamilyAndAssembly)
            {
                return AccessModifier.ProtectedAndInternal;
            }
            return AccessModifier.Unknown;
        }

        [Extension]
        public static AccessModifier GetAccessModifier(MemberInfo member)
        {
            AccessModifier unknown;
            try
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Constructor:
                    case MemberTypes.Method:
                        return GetAccessModifier(member as MethodBase);

                    case MemberTypes.Event:
                        return GetAccessModifier((MethodBase) (member as EventInfo).GetAddMethod(true));

                    case MemberTypes.Field:
                        return GetAccessModifier(member as FieldInfo);

                    case MemberTypes.Property:
                    {
                        PropertyInfo info = member as PropertyInfo;
                        MethodInfo getMethod = info.GetGetMethod(true);
                        if (getMethod == null)
                        {
                            getMethod = info.GetSetMethod(true);
                        }
                        return GetAccessModifier((MethodBase) getMethod);
                    }
                    case MemberTypes.TypeInfo:
                    case MemberTypes.NestedType:
                        return GetAccessModifier(member as Type);

                    case MemberTypes.Custom:
                    case MemberTypes.All:
                        return AccessModifier.Unknown;
                }
                throw new ArgumentOutOfRangeException();
            }
            catch (MemberAccessException)
            {
                unknown = AccessModifier.Unknown;
            }
            return unknown;
        }

        public static AccessModifier GetAccessModifier(MethodBase method)
        {
            if (method.IsPublic)
            {
                return AccessModifier.Public;
            }
            if (method.IsPrivate)
            {
                return AccessModifier.Private;
            }
            if (method.IsAssembly)
            {
                return AccessModifier.Internal;
            }
            if (method.IsFamily)
            {
                return AccessModifier.Protected;
            }
            if (method.IsFamilyOrAssembly)
            {
                return AccessModifier.ProtectedOrInternal;
            }
            if (method.IsFamilyAndAssembly)
            {
                return AccessModifier.ProtectedAndInternal;
            }
            return AccessModifier.Unknown;
        }

        public static AccessModifier GetAccessModifier(Type type)
        {
            if (type.DeclaringType == null)
            {
                if (type.IsPublic)
                {
                    return AccessModifier.Public;
                }
                return AccessModifier.Internal;
            }
            if (type.IsNestedPublic)
            {
                return AccessModifier.Public;
            }
            if (type.IsNestedAssembly)
            {
                return AccessModifier.Internal;
            }
            if (type.IsNestedPrivate)
            {
                return AccessModifier.Private;
            }
            if (type.IsNestedFamily)
            {
                return AccessModifier.Protected;
            }
            if (type.IsNestedFamORAssem)
            {
                return AccessModifier.ProtectedOrInternal;
            }
            if (type.IsNestedFamANDAssem)
            {
                return AccessModifier.ProtectedAndInternal;
            }
            return AccessModifier.Unknown;
        }

        [IteratorStateMachine(typeof(<GetAllInterfaces>d__39)), Extension]
        public static IEnumerable<Type> GetAllInterfaces(Type target)
        {
            foreach (Type <i>5__1 in target.GetInterfaces())
            {
                yield return <i>5__1;
                Type[] interfaces = <i>5__1.GetInterfaces();
                int index = 0;
            Label_PostSwitchInIterator:;
                if (index < interfaces.Length)
                {
                    Type type = interfaces[index];
                    yield return type;
                    index++;
                    goto Label_PostSwitchInIterator;
                }
                interfaces = null;
                <i>5__1 = null;
            }
        }

        [Extension]
        public static IEnumerable<MethodInfo> GetAllMethods(Type target)
        {
            List<Type> source = Enumerable.ToList<Type>(GetAllInterfaces(target));
            source.Add(target);
            return Enumerable.SelectMany<Type, MethodInfo, MethodInfo>(source, <>c.<>9__38_0 ?? (<>c.<>9__38_0 = new Func<Type, IEnumerable<MethodInfo>>(<>c.<>9, this.<GetAllMethods>b__38_0)), <>c.<>9__38_1 ?? (<>c.<>9__38_1 = new Func<Type, MethodInfo, MethodInfo>(<>c.<>9, this.<GetAllMethods>b__38_1)));
        }

        private static Type GetAssociatedMetadataType(Type type)
        {
            return AssociatedMetadataTypesCache.Get(type);
        }

        private static Type GetAssociateMetadataTypeFromAttribute(Type type)
        {
            throw new Exception();
        }

        public static T GetAttrb<T>(object provider) where T: Attribute
        {
            Type type = provider as Type;
            if (type != null)
            {
                return GetAttrb<T>(type);
            }
            MemberInfo memberInfo = provider as MemberInfo;
            if (memberInfo != null)
            {
                return GetAttrb<T>(memberInfo);
            }
            return GetAttrb<T>(provider, true);
        }

        private static T GetAttrb<T>(MemberInfo memberInfo) where T: Attribute
        {
            if (GetAssociatedMetadataType(memberInfo.DeclaringType) != null)
            {
                throw new Exception();
            }
            T attrb = GetAttrb<T>(memberInfo, true);
            if (attrb != null)
            {
                return attrb;
            }
            if (memberInfo.DeclaringType != null)
            {
                throw new Exception();
            }
            return default(T);
        }

        [Extension]
        private static T GetAttrb<T>(Type type) where T: Attribute
        {
            T attrb;
            Type associatedMetadataType = GetAssociatedMetadataType(type);
            if (associatedMetadataType != null)
            {
                attrb = GetAttrb<T>(associatedMetadataType, true);
                if (attrb != null)
                {
                    return attrb;
                }
            }
            attrb = GetAttrb<T>(type, true);
            if (attrb != null)
            {
                return attrb;
            }
            Type[] interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                attrb = GetAttrb<T>(interfaces[i], true);
                if (attrb != null)
                {
                    return attrb;
                }
            }
            return default(T);
        }

        private static T GetAttrb<T>(object attributeProvider, bool inherit) where T: Attribute
        {
            T[] attrbs = GetAttrbs<T>(attributeProvider, inherit);
            if (attrbs == null)
            {
                return default(T);
            }
            return Enumerable.SingleOrDefault<T>(attrbs);
        }

        public static T[] GetAttrbs<T>(object attributeProvider, bool inherit) where T: Attribute
        {
            if (attributeProvider == null)
            {
                throw new ArgumentNullException("attributeProvider");
            }
            object obj2 = attributeProvider;
            if (obj2 is Type)
            {
                return (T[]) ((Type) obj2).GetCustomAttributes(typeof(T), inherit);
            }
            if (obj2 is Assembly)
            {
                return (T[]) Attribute.GetCustomAttributes((Assembly) obj2, typeof(T));
            }
            if (obj2 is MemberInfo)
            {
                return (T[]) Attribute.GetCustomAttributes((MemberInfo) obj2, typeof(T), inherit);
            }
            if (obj2 is Module)
            {
                return (T[]) Attribute.GetCustomAttributes((Module) obj2, typeof(T), inherit);
            }
            if (obj2 is ParameterInfo)
            {
                return (T[]) Attribute.GetCustomAttributes((ParameterInfo) obj2, typeof(T), inherit);
            }
            return (T[]) ((ICustomAttributeProvider) attributeProvider).GetCustomAttributes(typeof(T), inherit);
        }

        public static MethodBase GetCallingMethod()
        {
            return GetCallingMethod(1);
        }

        public static MethodBase GetCallingMethod(int offset)
        {
            return new StackTrace().GetFrame(offset + 2).GetMethod();
        }

        public static IEnumerable<MethodBase> GetCallStack()
        {
            return GetCallStack(1);
        }

        public static IEnumerable<MethodBase> GetCallStack(int skipFrames)
        {
            return Enumerable.Skip<MethodBase>(ToMethods(new StackTrace()), skipFrames + 1);
        }

        [Extension]
        public static Type GetCollectionElementType(ICollection collection)
        {
            return GetCollectionElementType(collection.GetType());
        }

        [Extension]
        public static Type GetCollectionElementType(Type collectionType)
        {
            if (typeof(ICollection).IsAssignableFrom(collectionType))
            {
                if (collectionType.IsArray)
                {
                    if (collectionType.HasElementType)
                    {
                        return collectionType.GetElementType();
                    }
                }
                else if (collectionType.IsGenericType)
                {
                    Type[] genericArguments = collectionType.GetGenericArguments();
                    if ((genericArguments.Length == 1) && typeof(ICollection<>).MakeGenericType(genericArguments).IsAssignableFrom(collectionType))
                    {
                        return genericArguments[0];
                    }
                }
            }
            return null;
        }

        [Extension]
        public static Type GetCstrctdTypDef<T>(Dictionary<int, Type> dyct) where T: new()
        {
            return GetCstrctdTypDef(new KeyValuePair<string, Dictionary<int, Type>>(typeof(T).FullName, dyct));
        }

        [Extension]
        public static Type GetCstrctdTypDef(KeyValuePair<string, Dictionary<int, Type>> kv)
        {
            Type cstrctdTypDef = Type.GetType(kv.Key);
            if (cstrctdTypDef.IsGenericType)
            {
                if (Enumerable.Any<Type>(cstrctdTypDef.GetGenericArguments()) && (Enumerable.Count<Type>(cstrctdTypDef.GetGenericArguments()) == kv.Value.Count))
                {
                    <>c__DisplayClass49_0 class_;
                    cstrctdTypDef = cstrctdTypDef.MakeGenericType(Enumerable.ToArray<Type>(Enumerable.Select<int, Type>(kv.Value.Keys, new Func<int, Type>(class_, this.<GetCstrctdTypDef>b__0))));
                }
                return cstrctdTypDef;
            }
            if (cstrctdTypDef.IsInterface)
            {
                string key = kv.Key.Substring(1);
                cstrctdTypDef = GetCstrctdTypDef(new KeyValuePair<string, Dictionary<int, Type>>(key, kv.Value));
            }
            return cstrctdTypDef;
        }

        [Extension]
        public static Type GetCstrctdTypDef<T>(Type typ)
        {
            Type[] typs = new Type[] { typeof(T) };
            return GetCstrctdTypDef(typ, typs);
        }

        [Extension]
        public static Type GetCstrctdTypDef<TK, TV>(Type typ)
        {
            Type[] typs = new Type[] { typeof(TK), typeof(TV) };
            return GetCstrctdTypDef(typ, typs);
        }

        [Extension]
        public static Type GetCstrctdTypDef(Type typ, params Type[] typs)
        {
            Type type = null;
            if (!typ.IsGenericType || (Enumerable.Count<Type>(typ.GetGenericArguments()) != Enumerable.Count<Type>(typs)))
            {
                return type;
            }
            Type genericTypeDefinition = null;
            if (!typ.IsGenericTypeDefinition)
            {
                genericTypeDefinition = typ.GetGenericTypeDefinition();
                if (genericTypeDefinition != GetGenericTypDef(typ, new Type[0]))
                {
                    throw new Exception();
                }
            }
            else
            {
                genericTypeDefinition = typ;
            }
            return genericTypeDefinition.MakeGenericType(typs);
        }

        [Extension]
        public static T GetCustomAttribute<T>(MemberInfo member) where T: Attribute
        {
            return (T) Attribute.GetCustomAttribute(member, typeof(T));
        }

        [Extension]
        public static T GetCustomAttribute<T>(MemberInfo member, bool inherit) where T: Attribute
        {
            return (T) Attribute.GetCustomAttribute(member, typeof(T), inherit);
        }

        [Extension]
        public static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }
            Type[] typeArguments = new Type[] { type };
            return _getDefaultValueMethod.MakeGenericMethod(typeArguments).Invoke(null, null);
        }

        private static object GetDefaultValueImpl<T>()
        {
            return default(T);
        }

        [Extension]
        public static string GetFullTypNm(Type typ)
        {
            string arg = FwNs.Txt.Xtnz.Splyt(typ.Assembly.ToString(), ", ")[1];
            arg = FwNs.Txt.Xtnz.Splyt(arg, "=")[1];
            return GetFullTypNm(typ, arg);
        }

        [Extension]
        public static string GetFullTypNm(Type typ, string vrsn)
        {
            string arg = FwNs.Txt.Xtnz.Splyt(typ.Assembly.ToString(), ", ")[2];
            arg = FwNs.Txt.Xtnz.Splyt(arg, "=")[1];
            return GetFullTypNm(typ, vrsn, arg);
        }

        [Extension]
        public static string GetFullTypNm(Type typ, string vrsn, string cltr)
        {
            string arg = FwNs.Txt.Xtnz.Splyt(typ.Assembly.ToString(), ", ")[3];
            arg = FwNs.Txt.Xtnz.Splyt(arg, "=")[1];
            return GetFullTypNm(typ, vrsn, cltr, arg);
        }

        [Extension]
        public static string GetFullTypNm(Type typ, string vrsn, string cltr, string kyTkn)
        {
            string fullName = typ.Assembly.FullName;
            fullName = typ.Assembly.GetName().Name;
            return GetFullTypNm(typ.Name, typ.Namespace, fullName, vrsn, cltr, kyTkn);
        }

        public static string GetFullTypNm(string typNm, string nmspc, string assmbly, string vrsn, string cltr, string kyTkn)
        {
            throw new Exception();
        }

        public static Type GetFuncOf<TR>(params Type[] inputTs)
        {
            return GetFuncOf(typeof(TR), inputTs);
        }

        [Extension]
        public static Type GetFuncOf(Type returnT, params Type[] inputTs)
        {
            Type type = typeof(Func<>);
            Type[] second = new Type[] { returnT };
            if (Enumerable.Any<Type>(inputTs))
            {
                second = Enumerable.ToArray<Type>(Enumerable.Concat<Type>(inputTs, second));
                if (Enumerable.Count<Type>(inputTs) >= 2)
                {
                    if (Enumerable.Count<Type>(inputTs) >= 3)
                    {
                        if (Enumerable.Count<Type>(inputTs) >= 4)
                        {
                            if (Enumerable.Count<Type>(inputTs) >= 5)
                            {
                                if (Enumerable.Count<Type>(inputTs) >= 6)
                                {
                                    throw new Exception();
                                }
                                type = typeof(Func<,,,,,>);
                            }
                            else
                            {
                                type = typeof(Func<,,,,>);
                            }
                        }
                        else
                        {
                            type = typeof(Func<,,,>);
                        }
                    }
                    else
                    {
                        type = typeof(Func<,,>);
                    }
                }
                else
                {
                    type = typeof(Func<,>);
                }
            }
            return type.MakeGenericType(second);
        }

        [Extension]
        public static Type GetGenericTypDef<TK, TV>(Type typ)
        {
            Type[] typs = new Type[] { typeof(TK), typeof(TV) };
            return GetGenericTypDef(typ, typs);
        }

        [Extension]
        public static Type GetGenericTypDef(Type typ, params Type[] typs)
        {
            Type genericTypeDefinition = null;
            if (typ.IsGenericType)
            {
                if (typ.IsGenericTypeDefinition)
                {
                    return typ;
                }
                if (IsConstructedGenericType(typ))
                {
                    genericTypeDefinition = typ.GetGenericTypeDefinition();
                }
            }
            return genericTypeDefinition;
        }

        [Extension]
        public static Type GetGenericTypDef(Type typ, int pCnt)
        {
            Type genericTypeDefinition = null;
            if (typ.IsGenericType)
            {
                if (typ.IsGenericTypeDefinition)
                {
                    return typ;
                }
                if (IsConstructedGenericType(typ))
                {
                    genericTypeDefinition = typ.GetGenericTypeDefinition();
                }
            }
            return genericTypeDefinition;
        }

        public static Type GetGenrcTyp<T>(int cnt)
        {
            return GetGenrcTyp(typeof(T), cnt);
        }

        [Extension]
        public static Type GetGenrcTyp(string typnm, int cnt)
        {
            string str = "";
            string str2 = "{0}'{1}[[{2}]]";
            if (Enumerable.Contains<char>(typnm, '\''))
            {
                char[] separator = new char[] { '\'' };
                typnm = Enumerable.First<string>(typnm.Split(separator));
            }
            if (0 < cnt)
            {
                str = FwNs.Txt.Xtnz.Joyn<IEnumerable<string>>(Enumerable.Repeat<string>(GetTypNm(typeof(object)), cnt), "],[");
            }
            else
            {
                str2 = "{0}'{1}{2}";
            }
            object[] a = new object[] { typnm, cnt, str };
            typnm = FwNs.Txt.Xtnz.Frmt(str2, a);
            return Type.GetType(typnm).GetGenericTypeDefinition();
        }

        [Extension]
        public static Type GetGenrcTyp(Type typB, params Type[] inputTs)
        {
            if (!typB.IsGenericType)
            {
                throw new Exception();
            }
            if (Enumerable.Any<Type>(inputTs))
            {
                return Type.GetType(GetTypNm(typB) + "'" + Enumerable.Count<Type>(inputTs)).MakeGenericType(inputTs);
            }
            if (IsDelegate(typB))
            {
                if (typB != typeof(Func<>))
                {
                    throw new Exception();
                }
                return Type.GetType(GetTypNm(typB) + "'1").MakeGenericType(new Type[0]);
            }
            if (IsAction(typB))
            {
                if (typB != typeof(Action))
                {
                    throw new Exception();
                }
                return typeof(Action);
            }
            if (!FwNs.Core.Xtnz.IsNull<Type[]>(inputTs))
            {
                throw new Exception();
            }
            if (!Enumerable.Any<Type>(typB.GetGenericArguments()))
            {
                throw new Exception();
            }
            return Type.GetType(GetTypNm(typB) + "'1").MakeGenericType(new Type[0]);
        }

        [Extension]
        public static Type GetGenrcTyp(Type typT, int cnt)
        {
            return GetGenrcTyp(GetTypNm(typT), cnt);
        }

        private static Type GetMetadataTypeAttributeType()
        {
            throw new Exception();
        }

        internal static Type GetNonNullableType(Type type)
        {
            if (IsOfGenericType(type, typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }
            return type;
        }

        [Extension]
        public static ConstructorInfo GetParameterlessConstructor(Type type)
        {
            return GetParameterlessConstructor(type, false);
        }

        [Extension]
        public static ConstructorInfo GetParameterlessConstructor(Type type, bool publicOnly)
        {
            BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;
            if (!publicOnly)
            {
                bindingAttr |= BindingFlags.NonPublic;
            }
            return type.GetConstructor(bindingAttr, null, Type.EmptyTypes, null);
        }

        [Extension]
        public static Type GetSequenceElementType(Type sequenceType)
        {
            if (IsOfGenericType(sequenceType, typeof(IEnumerable<>)))
            {
                return sequenceType.GetGenericArguments()[0];
            }
            foreach (Type type in sequenceType.GetInterfaces())
            {
                if (IsOfGenericType(type, typeof(IEnumerable<>)))
                {
                    return type.GetGenericArguments()[0];
                }
            }
            return null;
        }

        [Extension]
        public static string GetTypNm(Type typ)
        {
            object[] a = new object[] { typ.Namespace, typ.Name };
            string str = FwNs.Txt.Xtnz.Frmt("{0}.{1}", a);
            if (!Enumerable.Any<Type>(typ.GetGenericArguments()))
            {
                return str;
            }
            string[] tA = Enumerable.ToArray<string>(Enumerable.Select<string, string>(Enumerable.ToArray<string>(Enumerable.Select<Type, string>(typ.GetGenericArguments(), new Func<Type, string>(null, GetTypNm))), <>c.<>9__0_0 ?? (<>c.<>9__0_0 = new Func<string, string>(<>c.<>9, this.<GetTypNm>b__0_0))));
            object[] objArray2 = new object[2];
            objArray2[0] = str;
            object[] objArray3 = new object[] { FwNs.Txt.Xtnz.Joyn<string[]>(tA, ", ") };
            objArray2[1] = FwNs.Txt.Xtnz.Frmt("[{0}]", objArray3);
            return FwNs.Txt.Xtnz.Frmt("{0}{1}", objArray2);
        }

        [Extension]
        public static Type GetValueType(MemberInfo member)
        {
            MemberTypes memberType = member.MemberType;
            if (memberType <= MemberTypes.Property)
            {
                switch (memberType)
                {
                    case MemberTypes.Constructor:
                        return member.DeclaringType;

                    case MemberTypes.Event:
                        return (member as EventInfo).EventHandlerType;

                    case MemberTypes.Field:
                        return (member as FieldInfo).FieldType;

                    case MemberTypes.Method:
                        return (member as MethodInfo).ReturnType;

                    case MemberTypes.Property:
                        return (member as PropertyInfo).PropertyType;
                }
                goto Label_0090;
            }
            if (memberType <= MemberTypes.Custom)
            {
                if (memberType != MemberTypes.TypeInfo)
                {
                    if (memberType == MemberTypes.Custom)
                    {
                        return null;
                    }
                    goto Label_0090;
                }
            }
            else
            {
                switch (memberType)
                {
                    case MemberTypes.NestedType:
                        goto Label_002F;

                    case MemberTypes.All:
                        return null;
                }
                goto Label_0090;
            }
        Label_002F:
            return (member as Type);
        Label_0090:
            throw new ArgumentOutOfRangeException();
        }

        [Extension]
        public static Type GetXprsnOf(Type returnT, params Type[] inputTs)
        {
            Type[] typeArguments = new Type[] { GetFuncOf(returnT, inputTs) };
            return typeof(Expression<>).MakeGenericType(typeArguments);
        }

        [Extension]
        public static bool HasAncstr<TAncstr>(Type typ)
        {
            return HasAncstr(typ, typeof(TAncstr));
        }

        [Extension]
        public static bool HasAncstr(Type typ, Type typAncstr)
        {
            if (typAncstr.IsInterface)
            {
                return Enumerable.Contains<Type>(typ.GetInterfaces(), typAncstr);
            }
            if (!typAncstr.IsClass)
            {
                throw new Exception();
            }
            return typAncstr.IsSubclassOf(typ);
        }

        [Extension]
        public static bool HasParameters(MethodInfo method, params Type[] parameterTypes)
        {
            Type[] typeArray = Enumerable.ToArray<Type>(Enumerable.Select<ParameterInfo, Type>(method.GetParameters(), <>c.<>9__37_0 ?? (<>c.<>9__37_0 = new Func<ParameterInfo, Type>(<>c.<>9, this.<HasParameters>b__37_0))));
            if (typeArray.Length != parameterTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < typeArray.Length; i++)
            {
                if (typeArray[i].ToString() != parameterTypes[i].ToString())
                {
                    return false;
                }
            }
            return true;
        }

        [Extension]
        internal static bool ImplementsInterface(Type type, Type interfaceType)
        {
            Func<Type, bool> func;
            if (interfaceType.IsGenericTypeDefinition)
            {
                <>c__DisplayClass77_0 class_;
                func = new Func<Type, bool>(class_, this.<ImplementsInterface>b__0);
            }
            else
            {
                func = new Func<Type, bool>(interfaceType, interfaceType.IsAssignableFrom);
            }
            return Enumerable.Any<Type>(Ancestors(type), func);
        }

        [Extension]
        public static bool IsAction(Type typ)
        {
            return typ.Name.StartsWith("Action");
        }

        [Extension]
        public static bool IsActionOf<T>(Type typ)
        {
            return IsActionOf(typ, typeof(T));
        }

        [Extension]
        public static bool IsActionOf(Type typ, Type possbleTargTyp)
        {
            Type[] typeArguments = new Type[] { possbleTargTyp };
            return typeof(Action<>).MakeGenericType(typeArguments).IsAssignableFrom(typ);
        }

        [Extension]
        public static bool IsAnonymous(Type type)
        {
            if (type.IsDefined(typeof(AnonymousAttribute), true))
            {
                return true;
            }
            if (!type.IsDefined(typeof(CompilerGeneratedAttribute), false) || !type.IsSealed)
            {
                return false;
            }
            ConstructorInfo[] constructors = type.GetConstructors();
            return ((constructors.Length == 1) && constructors[0].IsSpecialName);
        }

        [Extension]
        public static bool IsComparable(Type type)
        {
            if (!typeof(IComparable).IsAssignableFrom(type))
            {
                Type[] typeArguments = new Type[] { type };
                return typeof(IComparable<>).MakeGenericType(typeArguments).IsAssignableFrom(type);
            }
            return true;
        }

        [Extension]
        public static bool IsConstructedGenericType(Type typ)
        {
            return (typ.IsGenericType && !typ.ContainsGenericParameters);
        }

        [Extension]
        public static bool IsDelegate(Type type)
        {
            return type.IsSubclassOf(typeof(Delegate));
        }

        [Extension]
        public static bool IsDscndnt<TTest>(Type typ)
        {
            return ((typ == typeof(TTest)) || typ.IsSubclassOf(typeof(TTest)));
        }

        [Extension]
        public static bool IsFunc(Type typ)
        {
            return typ.Name.StartsWith("Func");
        }

        [Extension]
        public static bool IsFuncOf(Type typ, Type tR, params Type[] types)
        {
            return (typ == GetFuncOf(tR, types));
        }

        [Extension]
        public static bool IsGeneric(Type typ)
        {
            return Enumerable.Any<Type>(typ.GetGenericArguments());
        }

        [Extension]
        public static bool IsGloballyPublic(MemberInfo member)
        {
            while (member != null)
            {
                if (GetAccessModifier(member) != AccessModifier.Public)
                {
                    return false;
                }
                member = member.DeclaringType;
            }
            return true;
        }

        public static bool IsImmutable(Type type)
        {
            if (((type != typeof(string)) && !type.IsPrimitive) && ((type != typeof(DateTime)) && (type != typeof(TimeSpan))))
            {
                return IsAnonymous(type);
            }
            return true;
        }

        [Extension]
        public static bool IsKeyVal(Type typ)
        {
            throw new Exception();
        }

        [Extension]
        public static bool IsKeyValuePair(Type typ)
        {
            return ((((typ != null) && Enumerable.Any<Type>(typ.GetGenericArguments())) && (Enumerable.Count<Type>(typ.GetGenericArguments()) == 2)) && (typ.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)));
        }

        [Extension]
        public static bool IsKeyValuePair<TK, TV>(Type typ)
        {
            Type[] typeArguments = new Type[] { typeof(TK), typeof(TV) };
            Type type = typeof(KeyValuePair<,>).MakeGenericType(typeArguments);
            if (type != typ)
            {
                return type.IsAssignableFrom(typ);
            }
            return true;
        }

        [Extension]
        internal static bool IsNumeric(Type type)
        {
            type = GetNonNullableType(type);
            return (!type.IsEnum && ((Type.GetTypeCode(type) - 4) <= TypeCode.UInt32));
        }

        [Extension]
        internal static bool IsNumerical(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if ((((type != typeof(int)) && (type != typeof(short))) && ((type != typeof(long)) && (type != typeof(float)))) && (type != typeof(double)))
            {
                return (type == typeof(decimal));
            }
            return true;
        }

        [Extension]
        public static bool IsOfGenericType(Type type, Type genericType)
        {
            return IsOfGenericType(type, genericType, false);
        }

        [Extension]
        public static bool IsOfGenericType(Type type, Type genericType, bool inherit)
        {
            <>c__DisplayClass75_0 class_1;
            Func<Type, bool> predicate = new Func<Type, bool>(class_1, this.<IsOfGenericType>b__0);
            if (!inherit)
            {
                return predicate.Invoke(type);
            }
            return Enumerable.Any<Type>(Ancestors(type), predicate);
        }

        [Extension]
        public static bool IsTypeOrDerivedFrom<TAncstr>(Type typ)
        {
            return IsTypeOrDerivedFrom(typ, typeof(TAncstr));
        }

        [Extension]
        public static bool IsTypeOrDerivedFrom(Type typ, Type typAncstr)
        {
            if (typ != typAncstr)
            {
                return HasAncstr(typ, typAncstr);
            }
            return true;
        }

        [Extension]
        public static bool IsTypeOrSubclassOf(Type typ, Type typ2)
        {
            if ((typ != typ2) && !typ.IsSubclassOf(typ2))
            {
                return false;
            }
            return true;
        }

        [Extension]
        public static bool IsValidValue(Type type, object value)
        {
            if (value != null)
            {
                return type.IsAssignableFrom(value.GetType());
            }
            if (type.IsValueType)
            {
                return IsOfGenericType(type, typeof(Nullable<>));
            }
            return true;
        }

        [Extension]
        public static bool IsVisibleToLiveLinq(MemberInfo member)
        {
            throw new Exception();
        }

        [Extension]
        public static MemberInfo Normalize(MemberInfo member)
        {
            if ((member.MemberType == MemberTypes.Method) && (member.DeclaringType != null))
            {
                PropertyInfo info;
                MethodInfo method = (MethodInfo) member;
                if (!method.IsSpecialName)
                {
                    return member;
                }
                BindingFlags declaredOnly = BindingFlags.DeclaredOnly;
                declaredOnly |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
                declaredOnly |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
                if (member.Name.StartsWith("get_"))
                {
                    Type[] emptyTypes = Type.EmptyTypes;
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length != 0)
                    {
                        emptyTypes = Enumerable.ToArray<Type>(Enumerable.Select<ParameterInfo, Type>(parameters, <>c.<>9__87_0 ?? (<>c.<>9__87_0 = new Func<ParameterInfo, Type>(<>c.<>9, this.<Normalize>b__87_0))));
                    }
                    info = member.DeclaringType.GetProperty(member.Name.Substring("get_".Length), declaredOnly, null, method.ReturnType, emptyTypes, null);
                    if ((info != null) && (info.GetGetMethod(true) == method))
                    {
                        return info;
                    }
                }
                Dictionary<MethodInfo, PropertyInfo> dictionary = _normalizedMembersCache;
                lock (dictionary)
                {
                    if (!_normalizedMembersCache.TryGetValue(method, out info))
                    {
                        <>c__DisplayClass87_0 class_;
                        info = Enumerable.FirstOrDefault<PropertyInfo>(member.DeclaringType.GetProperties(declaredOnly), new Func<PropertyInfo, bool>(class_, this.<Normalize>b__1));
                        _normalizedMembersCache.Add(method, info);
                    }
                }
                if (info != null)
                {
                    return info;
                }
            }
            return member;
        }

        public static string ToCSharpString(Type type)
        {
            StringBuilder target = new StringBuilder();
            ToCSharpString(type, target);
            return target.ToString();
        }

        public static void ToCSharpString(Type type, StringBuilder target)
        {
            target.Append(type.Name);
            if (type.IsGenericType)
            {
                target.Append("<");
                for (int i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    if (i > 0)
                    {
                        target.Append(", ");
                    }
                    ToCSharpString(type.GetGenericArguments()[i], target);
                }
                target.Append(">");
            }
        }

        [Extension]
        public static string ToLongString(MethodBase method)
        {
            if (method.DeclaringType != null)
            {
                return (method.DeclaringType.Name + "." + method.Name);
            }
            if (method.Module != null)
            {
                return (method.Module.Name + "." + method.Name);
            }
            return method.Name;
        }

        [Extension]
        public static MethodInfo ToMethodInfo(RuntimeMethodHandle handle)
        {
            return (MethodInfo) MethodBase.GetMethodFromHandle(handle);
        }

        [IteratorStateMachine(typeof(<ToMethods>d__106)), Extension]
        internal static IEnumerable<MethodBase> ToMethods(StackTrace trace)
        {
            return new <ToMethods>d__106(-2) { <>3__trace = trace };
        }

        [Extension]
        internal static bool TryChooseDisplayProperty(Type clrType, out PropertyInfo displayProperty, bool ignoreToString)
        {
            displayProperty = null;
            if (!ignoreToString)
            {
                MethodInfo method = clrType.GetMethod("ToString");
                if ((method != null) && (method.DeclaringType.Assembly == clrType.Assembly))
                {
                    return true;
                }
            }
            PropertyInfo[] properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            DisplayColumnAttribute displayColumnAttr = GetCustomAttribute<DisplayColumnAttribute>(clrType);
            if ((displayColumnAttr != null) && !string.IsNullOrEmpty(displayColumnAttr.DisplayColumn))
            {
                <>c__DisplayClass111_0 class_;
                displayProperty = Enumerable.FirstOrDefault<PropertyInfo>(properties, new Func<PropertyInfo, bool>(class_, this.<TryChooseDisplayProperty>b__0));
            }
            if (displayProperty == null)
            {
                DefaultPropertyAttribute defaultPropAttr = GetCustomAttribute<DefaultPropertyAttribute>(clrType);
                if (defaultPropAttr != null)
                {
                    <>c__DisplayClass111_1 class_2;
                    displayProperty = Enumerable.FirstOrDefault<PropertyInfo>(properties, new Func<PropertyInfo, bool>(class_2, this.<TryChooseDisplayProperty>b__1));
                }
            }
            if (displayProperty == null)
            {
                IEnumerable<PropertyInfo> source = Enumerable.Where<PropertyInfo>(properties, <>c.<>9__111_2 ?? (<>c.<>9__111_2 = new Func<PropertyInfo, bool>(<>c.<>9, this.<TryChooseDisplayProperty>b__111_2)));
                PropertyInfo info2 = Enumerable.FirstOrDefault<PropertyInfo>(source, <>c.<>9__111_3 ?? (<>c.<>9__111_3 = new Func<PropertyInfo, bool>(<>c.<>9, this.<TryChooseDisplayProperty>b__111_3)));
                if (info2 == null)
                {
                    info2 = Enumerable.FirstOrDefault<PropertyInfo>(source, <>c.<>9__111_4 ?? (<>c.<>9__111_4 = new Func<PropertyInfo, bool>(<>c.<>9, this.<TryChooseDisplayProperty>b__111_4)));
                }
                displayProperty = info2;
            }
            return (displayProperty != null);
        }

        [Extension]
        public static object TypNfo(Type typ)
        {
            <>c__DisplayClass53_0 class_;
            string[] strArray5;
            string str;
            Func<Type, string> t2sf = <>c.<>9__53_0 ?? (<>c.<>9__53_0 = new Func<Type, string>(<>c.<>9, this.<TypNfo>b__53_0));
            Func<IEnumerable<Type>, string[]> te2saf = new Func<IEnumerable<Type>, string[]>(class_, this.<TypNfo>b__1);
            Func<Type[], string[]> func = new Func<Type[], string[]>(class_, this.<TypNfo>b__3);
            Stack<Type> source = null;
            if (typ.BaseType != null)
            {
                Type[] collection = new Type[] { typ.BaseType };
                source = new Stack<Type>(collection);
                while (source.Peek().BaseType != null)
                {
                    source.Push(source.Peek().BaseType);
                }
            }
            string[] strArray = te2saf.Invoke(Enumerable.AsEnumerable<Type>(source));
            string[] strArray2 = func.Invoke(GenericTypeArguments(typ));
            string[] strArray3 = func.Invoke(typ.GetNestedTypes());
            string[] strArray4 = func.Invoke(typ.GetInterfaces());
            if (typ.IsGenericParameter)
            {
                strArray5 = func.Invoke(typ.GetGenericParameterConstraints());
            }
            else
            {
                strArray5 = new string[0];
            }
            string[] strArray6 = func.Invoke(typ.GetGenericArguments());
            if (typ.IsGenericType)
            {
                str = t2sf.Invoke(GetGenericTypDef(typ, new Type[0]));
            }
            else
            {
                str = "null";
            }
            string str2 = t2sf.Invoke(typ.UnderlyingSystemType);
            return new { 
                Namespace = typ.Namespace,
                Name = typ.Name,
                BaseType = t2sf.Invoke(typ.BaseType),
                IsArray = typ.IsArray,
                IsValueType = typ.IsValueType,
                IsInterface = typ.IsInterface,
                IsGenericType = typ.IsGenericType,
                IsGenericTypeDefinition = typ.IsGenericTypeDefinition,
                ContainsGenericParameters = typ.ContainsGenericParameters,
                IsConstructedGenericType = IsConstructedGenericType(typ),
                GenericTypeArguments = strArray2,
                Ancestors = strArray,
                NestedTypes = strArray3,
                Interfaces = strArray4,
                GenericParameterConstraints = strArray5,
                GenericArguments = strArray6,
                GenericTypeDefinition = str,
                UnderlyingSystemType = str2
            };
        }

        [Extension]
        public static XElement TypNfoXElement(Type typ)
        {
            JsonConvert.SerializeObject(TypNfo(typ));
            throw new Exception();
        }

        [Extension]
        public static XElement TypNfoXElement(Type[] typs)
        {
            return new XElement("typs", Enumerable.Select<Type, XElement>(typs, new Func<Type, XElement>(null, TypNfoXElement)));
        }

        public static Assembly LiveLinqAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }

        public static BindingFlags AllInstanceMembers
        {
            get
            {
                return (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly TypsFw.<>c <>9 = new TypsFw.<>c();
            public static Func<string, string> <>9__0_0;
            public static Func<ParameterInfo, Type> <>9__37_0;
            public static Func<Type, IEnumerable<MethodInfo>> <>9__38_0;
            public static Func<Type, MethodInfo, MethodInfo> <>9__38_1;
            public static Func<Type, string> <>9__53_0;
            public static Func<ParameterInfo, Type> <>9__87_0;
            public static Func<Delegate, string> <>9__94_0;
            public static Func<InternalsVisibleToAttribute, <>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName>> <>9__100_0;
            public static Func<<>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName>, bool> <>9__100_1;
            public static Func<<>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName>, InternalsVisibleToAttribute> <>9__100_2;
            public static Func<PropertyInfo, bool> <>9__111_2;
            public static Func<PropertyInfo, bool> <>9__111_3;
            public static Func<PropertyInfo, bool> <>9__111_4;

            internal <>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName> <AreInternalsVisibleToLiveLinq>b__100_0(InternalsVisibleToAttribute attr)
            {
                return new { 
                    attr = attr,
                    name = new AssemblyName(attr.AssemblyName)
                };
            }

            internal bool <AreInternalsVisibleToLiveLinq>b__100_1(<>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName> <>h__TransparentIdentifier0)
            {
                if (<>h__TransparentIdentifier0.name.Name != "C1.LiveLinq")
                {
                    return false;
                }
                return Enumerable.SequenceEqual<byte>(<>h__TransparentIdentifier0.name.GetPublicKey() ?? EmptyAry<byte>.Instance, FwNs.Core.Statk.PublkKey());
            }

            internal InternalsVisibleToAttribute <AreInternalsVisibleToLiveLinq>b__100_2(<>f__AnonymousType3<InternalsVisibleToAttribute, AssemblyName> <>h__TransparentIdentifier0)
            {
                return <>h__TransparentIdentifier0.attr;
            }

            internal string <BuildString>b__94_0(Delegate d)
            {
                return (d.Method.DeclaringType.Name + d.Method.Name);
            }

            internal IEnumerable<MethodInfo> <GetAllMethods>b__38_0(Type type)
            {
                return type.GetMethods();
            }

            internal MethodInfo <GetAllMethods>b__38_1(Type type, MethodInfo method)
            {
                return method;
            }

            internal string <GetTypNm>b__0_0(string ga)
            {
                object[] a = new object[] { ga };
                return FwNs.Txt.Xtnz.Frmt("[{0}]", a);
            }

            internal Type <HasParameters>b__37_0(ParameterInfo parameter)
            {
                return parameter.ParameterType;
            }

            internal Type <Normalize>b__87_0(ParameterInfo p)
            {
                return p.ParameterType;
            }

            internal bool <TryChooseDisplayProperty>b__111_2(PropertyInfo p)
            {
                return (p.PropertyType == typeof(string));
            }

            internal bool <TryChooseDisplayProperty>b__111_3(PropertyInfo p)
            {
                return (p.Name.IndexOf("Name", StringComparison.OrdinalIgnoreCase) >= 0);
            }

            internal bool <TryChooseDisplayProperty>b__111_4(PropertyInfo p)
            {
                return (p.Name.IndexOf("Description", StringComparison.OrdinalIgnoreCase) >= 0);
            }

            internal string <TypNfo>b__53_0(Type typx)
            {
                if (typx == null)
                {
                    return "null";
                }
                return string.Format("{0}.{1}", typx.Namespace, typx.Name);
            }
        }

        [CompilerGenerated]
        private sealed class <Ancestors>d__76 : IEnumerable<Type>, IEnumerable, IEnumerator<Type>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private Type <>2__current;
            private int <>l__initialThreadId;
            private Type type;
            public Type <>3__type;

            [DebuggerHidden]
            public <Ancestors>d__76(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    while (this.type != null)
                    {
                        this.<>2__current = this.type;
                        this.<>1__state = 1;
                        return true;
                    Label_002E:
                        this.<>1__state = -1;
                        this.type = this.type.BaseType;
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_002E;
            }

            [DebuggerHidden]
            IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
            {
                TypsFw.<Ancestors>d__76 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new TypsFw.<Ancestors>d__76(0);
                }
                d__.type = this.<>3__type;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.Type>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            Type IEnumerator<Type>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }


        [CompilerGenerated]
        private sealed class <ToMethods>d__106 : IEnumerable<MethodBase>, IEnumerable, IEnumerator<MethodBase>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private MethodBase <>2__current;
            private int <>l__initialThreadId;
            private StackTrace trace;
            public StackTrace <>3__trace;
            private int <i>5__1;

            [DebuggerHidden]
            public <ToMethods>d__106(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.get_CurrentManagedThreadId();
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<i>5__1 = 0;
                    while (this.<i>5__1 < this.trace.FrameCount)
                    {
                        this.<>2__current = this.trace.GetFrame(this.<i>5__1).GetMethod();
                        this.<>1__state = 1;
                        return true;
                    Label_0045:
                        this.<>1__state = -1;
                        int num2 = this.<i>5__1;
                        this.<i>5__1 = num2 + 1;
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_0045;
            }

            [DebuggerHidden]
            IEnumerator<MethodBase> IEnumerable<MethodBase>.GetEnumerator()
            {
                TypsFw.<ToMethods>d__106 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.get_CurrentManagedThreadId()))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new TypsFw.<ToMethods>d__106(0);
                }
                d__.trace = this.<>3__trace;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.Reflection.MethodBase>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            MethodBase IEnumerator<MethodBase>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }
    }
}

