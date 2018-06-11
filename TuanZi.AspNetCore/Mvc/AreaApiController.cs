using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.Core;

namespace TuanZi.AspNetCore.Mvc
{
    [RoleLimit]
    [Route("api/[area]/[controller]/[action]")]
    public abstract class AreaApiController : Controller
    { }
}
