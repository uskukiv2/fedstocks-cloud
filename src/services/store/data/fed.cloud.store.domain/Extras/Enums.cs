namespace fed.cloud.store.domain.Extras
{
    public enum UnitType
    {
        Gramms,
        Kilogramms,
        Peace,
        Litres,
        MiliLitres,
        CubicMeter,
        Kwh
    }

    public enum StatusType
    {
        New = 1,
        Receiving = 2,
        Complete = 3,
        CreationFailed = 500,
    }
}