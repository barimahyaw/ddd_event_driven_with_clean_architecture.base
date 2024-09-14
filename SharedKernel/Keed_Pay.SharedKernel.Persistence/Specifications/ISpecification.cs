using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Enums;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using System.Linq.Expressions;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Specifications;

public interface ISpecification<T> where T : Entity
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    SortDirection SortDirection { get; }
}