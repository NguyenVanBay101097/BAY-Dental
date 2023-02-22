using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        ISpecification<T> left;
        ISpecification<T> right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.left = left;
            this.right = right;
        }

        public override bool IsSatisfiedBy(T candidate) => left.IsSatisfiedBy(candidate) && right.IsSatisfiedBy(candidate);

        public override Expression<Func<T, bool>> AsExpression()
        {
            var parameter = Expression.Parameter(typeof(T));
            var expr1 = this.left.AsExpression();
            var expr2 = this.right.AsExpression();

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var lft = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var rght = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
             Expression.AndAlso(lft, rght), parameter);
        }
    }

    public  class ReplaceExpressionVisitor
        : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node);
        }
    }
}
