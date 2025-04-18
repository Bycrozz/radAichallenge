using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

public static class DataSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (!context.MobileFoodTrucks.Any())
        {
            using (var reader = new StreamReader("csv/Mobile_Food_Facility_Permit.csv"))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HeaderValidated = null, MissingFieldFound = null }))
            {
                var records = csv.GetRecords<MobileFoodTruck>().ToList();
                context.MobileFoodTrucks.AddRange(records);
                context.SaveChanges();
            }
        }
    }
}
