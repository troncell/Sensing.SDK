namespace SensingBase.Dictonary
{
    public interface IPropertyDictionary
    {
        T GetValue<T>(string propertyName);
        void SetValue<T>(string propertyName, T value);
        void RegisterPropertyChangeHandler(string propertyName, PropertyChanged handler);
    }
}
