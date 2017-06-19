using System;
using EPiServer.Core;
using JetBrains.Annotations;

namespace EEN.Web.AzureAD.Configuration.Episerver.ContentModels
{
    public abstract class TypedStringBasedProperty<TValue> : PropertyLongString where TValue : class
    {
        [CanBeNull]
        public virtual TValue TypedValue
        {
            get
            {
                if (string.IsNullOrEmpty(LongString))
                    return null;
                var result = DeserializeObject(LongString);
                return result;
            }
            set { LongString = SerializeObject(value); }
        }

        protected abstract string SerializeObject([CanBeNull] TValue value);
        protected abstract TValue DeserializeObject([CanBeNull] string json);

        public override object SaveData(PropertyDataCollection properties)
        {
            return base.LongString;
        }

        public override Type PropertyValueType => typeof(TValue);

        public override object Value
        {
            get { return TypedValue; }
            set
            {
                var typedValue = value as TValue;
                if (typedValue != null)
                {
                    TypedValue = typedValue;
                }
                else
                {
                    base.Value = value;
                }
            }
        }
    }
}