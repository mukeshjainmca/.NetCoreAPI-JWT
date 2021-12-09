using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Models;

namespace WebAPITemplate.Interface
{
    public interface IAccountController
    {
        IActionResult CreateUser([FromBody] UserRegistration user);
        IActionResult AddClient([FromBody] ClientBase client);
    }
}
