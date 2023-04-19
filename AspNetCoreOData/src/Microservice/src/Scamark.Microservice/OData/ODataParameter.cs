namespace Scamark.Microservice.OData;

public class ODataParameter<T> : ODataObjectParameter
{
    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            ObjectValue = value;
        }
    }
}