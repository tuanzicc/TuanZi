using System.Drawing;


namespace TuanZi.AspNetCore
{
    public interface IVerifyCodeService
    {
        bool CheckCode(string code, string id, bool removeIfSuccess = true);
        
        void SetCode(string code, out string id);

        string GetImageString(Image image, string id);
    }
}