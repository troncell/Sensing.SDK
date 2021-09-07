using System;
using System.Collections.Generic;

namespace SensingBase.Dictonary
{
    public delegate void PropertyChanged(object oldValue, object newValue);

    public class PropertyDictionary : IPropertyDictionary
    {
        private Dictionary<string, PropertyDictionaryEntry> propertyDictionary;

        private class PropertyDictionaryEntry
        {
            public Type Type
            {
                get;
                private set;
            }

            public object Value
            {
                get;
                set;
            }

            public IList<PropertyChanged> ChangeHandlers
            {
                get;
                private set;
            }

            public PropertyDictionaryEntry(Type type, object value)
            {
                Type = type;
                Value = value;
                ChangeHandlers = new List<PropertyChanged>();
            }
        }

        public PropertyDictionary()
        {
            propertyDictionary = new Dictionary<string, PropertyDictionaryEntry>();
        }

        public T GetValue<T>(string propertyName)
        {
            PropertyDictionaryEntry entry;
            if (!propertyDictionary.TryGetValue(propertyName, out entry))
            {
                entry = new PropertyDictionaryEntry(typeof(T), default(T));
                propertyDictionary.Add(propertyName, entry);
            }
            return (T)entry.Value;
        }

        public void SetValue<T>(string propertyName, T value)
        {
            T oldValue;
            PropertyDictionaryEntry entry;
            if (!propertyDictionary.TryGetValue(propertyName, out entry))
            {
                entry = new PropertyDictionaryEntry(typeof(T), default(T));
                propertyDictionary.Add(propertyName, entry);
            }
            oldValue = (T)entry.Value;
            entry.Value = value;

            foreach (PropertyChanged handler in entry.ChangeHandlers)
            {
                handler(oldValue, value);
            }
        }

        public void RegisterPropertyChangeHandler(string propertyName, PropertyChanged handler)
        {
            PropertyDictionaryEntry entry;
            if (!propertyDictionary.TryGetValue(propertyName, out entry))
            {
                throw new InvalidOperationException("Cannot register a ChangeProperty handler on a property that has not been registered.");
            }
            entry.ChangeHandlers.Add(handler);
        }
    }
}
