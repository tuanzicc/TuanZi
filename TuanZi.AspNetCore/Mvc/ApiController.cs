using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.AspNetCore.Mvc
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class ApiController : Controller
    { }
}
