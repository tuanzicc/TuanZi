using TuanZi.Reflection;


namespace TuanZi.Core.Systems
{
    public class SettingOutputDto
    {
        public SettingOutputDto(ISetting setting)
        {
            Setting = setting;
            SettingTypeName = setting.GetType().GetFullNameWithModule();
        }

        public string SettingTypeName { get; }

        public ISetting Setting { get; }
    }
}