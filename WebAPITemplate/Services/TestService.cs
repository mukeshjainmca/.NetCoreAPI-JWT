using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Interface;
using WebAPITemplate.Models;

namespace WebAPITemplate.Services
{
    public class TestService : ITestService
    {
        public List<TestDataModel> GetTestData()
        {
            return new List<TestDataModel>
            {
                new TestDataModel{ID = 1,Name="Name 1",Address="Address 1",City="City 1",ZipCode="11111" },
                new TestDataModel{ID = 2,Name="Name 2",Address="Address 2",City="City 2",ZipCode="22222" },
                new TestDataModel{ID = 3,Name="Name 3",Address="Address 3",City="City 3",ZipCode="33333" },
                new TestDataModel{ID = 4,Name="Name 4",Address="Address 4",City="City 4",ZipCode="44444" },
                new TestDataModel{ID = 5,Name="Name 5",Address="Address 5",City="City 5",ZipCode="55555" },
            };
        }
    }
}
