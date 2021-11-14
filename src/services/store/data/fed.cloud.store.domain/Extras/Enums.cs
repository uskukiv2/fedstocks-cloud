namespace fed.cloud.store.domain.Extras
{
    public enum UnitType
    {
        Grams,
        Kilograms,
        Peace,
        Liters,
        MiloLiters,
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