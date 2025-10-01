using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SportSphere.Domain.Entities;
using SportSphere.Infrastructure.DataContext;

namespace SportSphere.Application.Services
{
    public class DbCountriesSeeder
    {
        public static void SeedCountriesFromDictionary(ApplicationDbContext context, string jsonFilePath)
        {
            if (context.Countries.Any()) return; // don’t seed twice

            var json = File.ReadAllText(jsonFilePath);

            // Deserialize into dictionary: CountryName -> List of CityNames
            var dict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);

            foreach (var kv in dict!)
            {
                var country = new Country { Name = kv.Key };

                foreach (var cityName in kv.Value)
                {
                    country.Cities.Add(new City { Name = cityName });
                }

                context.Countries.Add(country);
            }

            context.SaveChanges();
        }
    }
}
