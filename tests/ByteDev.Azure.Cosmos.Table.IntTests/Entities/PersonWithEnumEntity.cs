namespace ByteDev.Azure.Cosmos.Table.IntTests.Entities
{
    public class PersonWithEnumEntity : CustomTableEntity
    {
        public Country ResidentCountry { get; set; }

        public Country MovingCountry { get; set; }
    }

    public enum Country
    {
        Uk = 1,
        Usa = 2,
        France = 3
    }
}