using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Models;

namespace WebAPITemplate.Interface
{
    public interface ITestService
    {
        public List<TestDataModel> GetTestData();
    }
}
