using ReadingList.Domain;

namespace ReadingList.Application.Interfaces;

public interface IRepository<T>
{
    Result<T> Add(T entity);
    Result Delete(T entity);
    Result<IEnumerable<T>> GetAll();
    Result<T> GetById(int id);
    Result<T> Update(T entity);
}