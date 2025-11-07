using ReadingList.Application.Interfaces;
using ReadingList.Domain;

namespace ReadingList.Infrastructure.Repositories;

public class InMemoryRepository<T, TKey> : IRepository<T>
{
    private Dictionary<TKey, T> _storage = new Dictionary<TKey, T>();
    private Func<T, TKey> _keySelector;

    public InMemoryRepository(Func<T, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public Result<T> Add(T entity)
    {
        var key = _keySelector(entity);
        if (!_storage.TryAdd(key, entity))
        {
            return Result<T>.Failure($"Duplicate key: {key}");
        }
        return Result<T>.Success(entity);
    }

    public Result Delete(T entity)
    {
        var key = _keySelector(entity);
        return _storage.Remove(key)
            ? Result.Success()
            : Result.Failure($"Entity with id {key} not found.");
    }

    public Result<IEnumerable<T>> GetAll() =>
        Result<IEnumerable<T>>.Success(_storage.Values);

    public Result<T> GetById(int id)
    {
        if (_storage.TryGetValue((TKey)(object)id, out var entity))
            return Result<T>.Success(entity);
        return Result<T>.Failure($"Entity with id {id} not found.");
    }

    public Result<T> Update(T entity)
    {
        var key = _keySelector(entity);
        if (!_storage.ContainsKey(key))
            return Result<T>.Failure($"Entity with id {key} not found.");
        _storage[key] = entity;
        return Result<T>.Success(entity);
    }
}
