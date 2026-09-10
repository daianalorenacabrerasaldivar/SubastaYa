namespace Aplication.Common.Interface;
public interface IRepositoryQuery
{
    IQueryable<T> Query<T>() where T : class;
}