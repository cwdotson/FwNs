namespace FwNs.Linq.Xprsns
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Resources;

    internal class EMExpressions
    {
        private static System.Resources.ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal EMExpressions()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new System.Resources.ResourceManager("C1.LiveLinq.Linq.Expressions.EMExpressions", typeof(EMExpressions).Assembly);
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

        internal static string Eval_Cannot
        {
            get
            {
                return "EMExpressions.ResourceManager.GetString(Eval_Cannot, EMExpressions.resourceCulture)";
            }
        }

        internal static string Func_DifferentTypeValues
        {
            get
            {
                return "EMExpressions.ResourceManager.GetString(Func_DifferentTypeValues, EMExpressions.resourceCulture)";
            }
        }

        internal static string InvalidParameterCount
        {
            get
            {
                return "EMExpressions.ResourceManager.GetString(InvalidParameterCount, EMExpressions.resourceCulture)";
            }
        }

        internal static string NestedLambdaNotSupported
        {
            get
            {
                return "EMExpressions.ResourceManager.GetString(NestedLambdaNotSupported, EMExpressions.resourceCulture)";
            }
        }

        internal static string Visitor_ExpressionTypeNotSupported
        {
            get
            {
                return "EMExpressions.ResourceManager.GetString(Visitor_ExpressionTypeNotSupported, EMExpressions.resourceCulture)";
            }
        }

        internal static string Visitor_ExpressionTypePairNotSupportedByVisitor
        {
            get
            {
                return "EMExpressions.ResourceManager.GetString(Visitor_ExpressionTypePairNotSupportedByVisitor, EMExpressions.resourceCulture)";
            }
        }
    }
}

