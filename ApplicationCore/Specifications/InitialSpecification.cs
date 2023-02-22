using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationCore.Specifications
{
    public class InitialSpecification<T> : BaseSpecification<T>
    {
        public InitialSpecification(Expression<Func<T, bool>> criteria) : base(criteria)
        {
        }
    }
}
