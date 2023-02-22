using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace ApplicationCore.Models
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Filters<T>(this IQueryable<T> queryable, Filter filter, List<object> errors)
        {
            if (filter?.Logic != null)
            {
                // Pretreatment some work
                filter = PreliminaryWork(typeof(T), filter);

                // Collect a flat list of all filters
                var filters = filter.All();

                /* Method.1 Use the combined expression string */
                // Step.1 Create a predicate expression e.g. Field1 = @0 And Field2 > @1
                string predicate;
                try
                {
                    predicate = filter.ToExpression(typeof(T), filters);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                    return queryable;
                }

                // Step.2 Get all filter values as array (needed by the Where method of Dynamic Linq)
                var values = filters.Select(f => f.Value).ToArray();

                // Step.3 Use the Where method of Dynamic Linq to filter the data
                queryable = queryable.Where(predicate, values);

                /* Method.2 Use the combined lambda expression */
                // Step.1 Create a parameter "p"
                //var parameter = Expression.Parameter(typeof(T), "p");

                // Step.2 Make up expression e.g. (p.Number >= 3) AndAlso (p.Company.Name.Contains("M"))
                //Expression expression;
                //try 
                //{
                //    expression = filter.ToLambdaExpression<T>(parameter, filters);         
                //}
                //catch(Exception ex)
                //{
                //    errors.Add(ex.Message);
                //    return queryable;
                //} 

                // Step.3 The result is e.g. p => (p.Number >= 3) AndAlso (p.Company.Name.Contains("M"))
                //var predicateExpression = Expression.Lambda<Func<T, bool>>(expression, parameter);
                //queryable = queryable.Where(predicateExpression);
            }

            return queryable;
        }

        /// <summary>
        /// Pretreatment of specific DateTime type and convert some illegal value type
        /// </summary>
        /// <param name="filter"></param>
        private static Filter PreliminaryWork(Type type, Filter filter)
        {
            if (filter.Filters != null && filter.Logic != null)
            {
                var newFilters = new List<Filter>();
                foreach (var f in filter.Filters)
                {
                    newFilters.Add(PreliminaryWork(type, f));
                }

                filter.Filters = newFilters;
            }

            if (filter.Value == null) return filter;

            // When we have a decimal value, it gets converted to an integer/double that will result in the query break
            var currentPropertyType = Filter.GetLastPropertyType(type, filter.Field);
            if ((currentPropertyType == typeof(decimal)) && decimal.TryParse(filter.Value.ToString(), out decimal number))
            {
                filter.Value = number;
                return filter;
            }

            // if(currentPropertyType.GetTypeInfo().IsEnum && int.TryParse(filter.Value.ToString(), out int enumValue))
            // {           
            //     filter.Value = Enum.ToObject(currentPropertyType, enumValue);
            //     return filter;
            // }

            // Convert datetime-string to DateTime
            if (currentPropertyType == typeof(DateTime) && DateTime.TryParse(filter.Value.ToString(), out DateTime dateTime))
            {
                filter.Value = dateTime;

                // Copy the time from the filter
                var localTime = dateTime.ToLocalTime();

                // Used when the datetime's operator value is eq and local time is 00:00:00 
                if (filter.Operator == "eq")
                {
                    if (localTime.Hour != 0 || localTime.Minute != 0 || localTime.Second != 0)
                        return filter;

                    var newFilter = new Filter { Logic = "and" };
                    newFilter.Filters = new List<Filter>
                    {
                        // Instead of comparing for exact equality, we compare as greater than the start of the day...
                        new Filter
                        {
                            Field = filter.Field,
                            Filters = filter.Filters,
                            Value = new DateTime(localTime.Year, localTime.Month, localTime.Day, 0, 0, 0),
                            Operator = "gte"
                        },
                        // ...and less than the end of that same day (we're making an additional filter here)
                        new Filter
                        {
                            Field = filter.Field,
                            Filters = filter.Filters,
                            Value = new DateTime(localTime.Year, localTime.Month, localTime.Day, 23, 59, 59),
                            Operator = "lte"
                        }
                    };

                    return newFilter;
                }

                // Convert datetime to local 
                filter.Value = new DateTime(localTime.Year, localTime.Month, localTime.Day, localTime.Hour, localTime.Minute, localTime.Second, localTime.Millisecond);
            }

            return filter;
        }
    }
}
