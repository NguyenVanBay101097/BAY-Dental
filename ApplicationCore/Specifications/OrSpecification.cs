using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class OrSpecification<T> : CompositeSpecification<T>
    {
        ISpecification<T> left;
        ISpecification<T> right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.left = left;
            this.right = right;
        }

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
             Expression.OrElse(lft, rght), parameter);
        }

        public override bool IsSatisfiedBy(T candidate) => left.IsSatisfiedBy(candidate) || right.IsSatisfiedBy(candidate);
    }
}
