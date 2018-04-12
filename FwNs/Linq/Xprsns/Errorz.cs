namespace FwNs.Linq.Xprsns
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Resources;

    public class Errorz
    {
        private static System.Resources.ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new System.Resources.ResourceManager("Lib.Errorz", typeof(Errorz).Assembly);
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static string Argument_ArrayIsEmpty
        {
            get
            {
                return "Argument_ArrayIsEmpty";
            }
        }

        internal static string Argument_ArrayIsTooSmall
        {
            get
            {
                return "Argument_ArrayIsTooSmall";
            }
        }

        internal static string Argument_NotGreater
        {
            get
            {
                return "Error";
            }
        }

        internal static string Argument_NotLess
        {
            get
            {
                return "Error";
            }
        }

        internal static string Assert_ValuesEqual
        {
            get
            {
                return "Error";
            }
        }

        internal static string Assert_ValuesNotEqual
        {
            get
            {
                return "Error";
            }
        }

        internal static string Cast_InvalidAnonumousType
        {
            get
            {
                return "Error";
            }
        }

        internal static string Cloner_CanNotClone
        {
            get
            {
                return "Error";
            }
        }

        internal static string Cloner_InsufficientPermissionsForFieldCopy
        {
            get
            {
                return "Error";
            }
        }

        internal static string Collection_AddNull
        {
            get
            {
                return "Error";
            }
        }

        internal static string Collection_DuplicateItem
        {
            get
            {
                return "Error";
            }
        }

        internal static string CompiledQuery_InvalidTResult
        {
            get
            {
                return "Error";
            }
        }

        internal static string Convert_IConvertable
        {
            get
            {
                return "Convert_IConvertable";
            }
        }

        internal static string Convert_NullToValueType
        {
            get
            {
                return "Convert_NullToValueType";
            }
        }

        internal static string CreateNew_IsDirty
        {
            get
            {
                return "CreateNew_IsDirty";
            }
        }

        internal static string DataStructures_KeyNotFound
        {
            get
            {
                return "DataStructures_KeyNotFound";
            }
        }

        internal static string Errors_ObjIsNull
        {
            get
            {
                return "Errors_ObjIsNull";
            }
        }

        internal static string Errors_StringIsNull
        {
            get
            {
                return "Errors_StringIsNull";
            }
        }

        internal static string Errors_UnexpectedEnumValue
        {
            get
            {
                return "Errors_UnexpectedEnumValue";
            }
        }

        internal static string Exception_InCollectionSelector
        {
            get
            {
                return "Error";
            }
        }

        internal static string Exception_InElementSelector
        {
            get
            {
                return "Error";
            }
        }

        internal static string Exception_InPredicate
        {
            get
            {
                return "Error";
            }
        }

        internal static string Exception_InResultSelectorWithBody
        {
            get
            {
                return "Error";
            }
        }

        internal static string Exception_InResultSelectorWithoutBody
        {
            get
            {
                return "Error";
            }
        }

        internal static string ExceptionInKeySelector
        {
            get
            {
                return "Error";
            }
        }

        internal static string IObservableSource_CantProvideItemOrdinal
        {
            get
            {
                return "Error";
            }
        }

        internal static string KeyComparer_InvalidTypes
        {
            get
            {
                return "Error";
            }
        }

        internal static string Liseners_PropertyChangingEventNotSupported
        {
            get
            {
                return "Error";
            }
        }

        internal static string Listener_PropertiesOverlap
        {
            get
            {
                return "Listener_PropertiesOverlap";
            }
        }

        internal static string Listener_UnionDifferentSupportOfChangingEvent
        {
            get
            {
                return "Listener_UnionDifferentSupportOfChangingEvent";
            }
        }

        internal static string Listeners_DefaultCannotBeCreated
        {
            get
            {
                return "Listeners_DefaultCannotBeCreated";
            }
        }

        internal static string MultiThreadAccess
        {
            get
            {
                return "MultiThreadAccess";
            }
        }

        internal static string ObservableSource_ChangingExpected
        {
            get
            {
                return "ObservableSource_ChangingExpected";
            }
        }

        internal static string ObservableSource_ItemWasNotAdded
        {
            get
            {
                return "ObservableSource_ItemWasNotAdded";
            }
        }

        internal static string ObservableSource_ValidOrdinalExpectedInChangeEvent
        {
            get
            {
                return "ObservableSource_ValidOrdinalExpectedInChangeEvent";
            }
        }

        internal static string ParametrizedQuery_ArgsChanged
        {
            get
            {
                return "ParametrizedQuery_ArgsChanged";
            }
        }

        internal static string Property_IsReadOnly
        {
            get
            {
                return "Property_IsReadOnly";
            }
        }

        internal static string ResultOrder_KeysSeqEmpty
        {
            get
            {
                return "ResultOrder_KeysSeqEmpty";
            }
        }

        internal static string RowListener_TableIsNull
        {
            get
            {
                return "RowListener_TableIsNull";
            }
        }

        internal static string Security_MemberIsNotAccessible
        {
            get
            {
                return "Security_MemberIsNotAccessible";
            }
        }

        internal static string Source_IsReadOnly
        {
            get
            {
                return "Source_IsReadOnly";
            }
        }

        internal static string TypeArgument_InvalidHandlerType
        {
            get
            {
                return "TypeArgument_InvalidHandlerType";
            }
        }

        internal static string TypeArgument_NotDelegate
        {
            get
            {
                return "TypeArgument_NotDelegate";
            }
        }

        internal static string WeakEvents_EventNotFound
        {
            get
            {
                return "WeakEvents_EventNotFound";
            }
        }

        internal static string WeakEvents_InvalidEventArgsType
        {
            get
            {
                return "WeakEvents_InvalidEventArgsType";
            }
        }
    }
}

