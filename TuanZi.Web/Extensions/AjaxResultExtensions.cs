
using Microsoft.AspNetCore.Identity;

using TuanZi.AspNetCore.UI;
using TuanZi.Data;

namespace TuanZi.Web
{
    public static class AjaxResultExtensions
    {
        public static AjaxResult ToAjaxResult(this int result, string success = "Done", string fail = "Failed")
        {
            var obj = new AjaxResult();
            if (result > 0)
            {
                obj.Type = AjaxResultType.Success;
                obj.Content = success;
            }
            else if (result == 0)
            {
                obj.Type = AjaxResultType.Info;
                obj.Content = "Saved";
            }
            else
            {
                obj.Type = AjaxResultType.Error;
                obj.Content = fail;
            }
            return obj;
        }

        public static AjaxResult ToAjaxResult(this bool result, string success = "Done", string fail = "Failed")
        {
            var obj = new AjaxResult();
            if (result)
            {
                obj.Type = AjaxResultType.Success;
                obj.Content = success;
            }
            else
            {
                obj.Type = AjaxResultType.Error;
                obj.Content = fail;
            }
            return obj;
        }

        public static AjaxResult ToAjaxResult(this SignInResult result, string success = "Done", string fail = "Invalid Username or Password")
        {
            var obj = new AjaxResult();
            if (result.Succeeded)
            {
                obj.Type = AjaxResultType.Success;
                obj.Content = success;
            }
            else
            {
                obj.Type = AjaxResultType.Error;
                obj.Content = fail;
            }
            return obj;
        }
    }

    
    
}
