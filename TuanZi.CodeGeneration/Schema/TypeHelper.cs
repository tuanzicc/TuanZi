using TuanZi.Data;


namespace TuanZi.CodeGeneration.Schema
{
    public static class TypeHelper
    {
        public static string ToSingleTypeName(string fullName, bool isNullable = false)
        {
            Check.NotNull(fullName, nameof(fullName));
            int startIndex = fullName.LastIndexOf('.');
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            string name = fullName.Substring(startIndex);
            switch (fullName)
            {
                case "System.Byte":
                    name = "byte";
                    break;
                case "System.Int32":
                    name = "int";
                    break;
                case "System.Int64":
                    name = "long";
                    break;
                case "System.Decimal":
                    name = "decimal";
                    break;
                case "System.Single":
                    name = "float";
                    break;
                case "System.Double":
                    name = "double";
                    break;
                case "System.String":
                    name = "string";
                    break;
                case "System.Guid":
                    name = "Guid";
                    break;
                case "System.Boolean":
                    name = "bool";
                    break;
                case "System.DateTime":
                    name = "DateTime";
                    break;
            }
            if (isNullable)
            {
                name = name + "?";
            }
            return name;
        }

    }
}