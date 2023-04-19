namespace Scamark.Microservice.OData;

public class ODataCollectionParameter<T> : ODataObjectParameter
{
    private IReadOnlyCollection<T> _value;

    public IReadOnlyCollection<T> Value
    {
        get => _value;
        set
        {
            _value = value;
            ObjectValue = value;
        }
    }
}