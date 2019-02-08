using System.Linq;
using System.Linq.Expressions;

using TuanZi.Collections;


namespace TuanZi.Caching
{
    public class ExpressionCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly Expression _expression;

        public ExpressionCacheKeyGenerator(Expression expression)
        {
            _expression = expression;
        }

        #region Implementation of ICacheKeyGenerator

        public string GetKey(params object[] args)
        {
            Expression expression = _expression;
            expression = Evaluator.PartialEval(expression, CanBeEvaluatedLocally);
            expression = LocalCollectionExpressionVisitor.Rewrite(expression);
            string key = expression.ToString();
            return key + args.ExpandAndToString();
        }

        #endregion

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter)
            {
                return false;
            }
            if (typeof(IQueryable).IsAssignableFrom(expression.Type))
            {
                return false;
            }
            return true;
        }
    }
}
