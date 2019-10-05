using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class NotSpecification<T> : CompositeSpecification<T>
    {
        ISpecification<T> other;
        public NotSpecification(ISpecification<T> other) => this.other = other;

        public override Expression<Func<T, bool>> AsExpression()
        {
            throw new NotImplementedException();
        }

        public override bool IsSatisfiedBy(T candidate) => !other.IsSatisfiedBy(candidate);
    }
}
