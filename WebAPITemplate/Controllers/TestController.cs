using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Interface;

namespace WebAPITemplate.Controllers
{
    [Authorize]
    [Route("api/Test")]
    public class TestController : Controller, ITestController
    {
        private readonly ITestService _testService;
        public TestController(ITestService testService)
        {
            _testService = testService ?? throw new ArgumentNullException(nameof(testService));
        }

        public IActionResult GeTestData()
        {
            var testdata = _testService.GetTestData();
            return Ok(testdata);
        }
    }
}
