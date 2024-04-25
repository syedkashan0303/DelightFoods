namespace DelightFoods_Live.Utilites
{
    public enum CityClass
    {
        KARACHI = 1,
        HYDERABAD = 2,
        ISLAMABAD = 3,
        MULTAN = 4,
        PESHAWAR = 5,
        QUETTA = 6,
        SIALKOT = 7,
        LAHORE = 8
    }

    public enum OrderStatusEnum
    {
        Pending = 0,
		Processing = 1,
        ReadytoShip = 2,
        Shipped = 3,
        Delivered = 4,
		Cancelled = 5,
		Returned = 6,
	}
}
