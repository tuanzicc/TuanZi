
namespace TuanZi.Filter
{
    public enum FilterOperate
    {
        [OperateCode("and")]
        And = 1,

        [OperateCode("or")]
        Or = 2,

        [OperateCode("equal")]
        Equal = 3,

        [OperateCode("notequal")]
        NotEqual = 4,

        [OperateCode("less")]
        Less = 5,

        [OperateCode("lessorequal")]
        LessOrEqual = 6,

        [OperateCode("greater")]
        Greater = 7,

        [OperateCode("greaterorequal")]
        GreaterOrEqual = 8,

        [OperateCode("startswith")]
        StartsWith = 9,

        [OperateCode("endswith")]
        EndsWith = 10,

        [OperateCode("contains")]
        Contains = 11,

        [OperateCode("notcontains")]
        NotContains = 12,

    }
}