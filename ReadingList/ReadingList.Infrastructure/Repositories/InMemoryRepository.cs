using ReadingList.Application.Interfaces;

namespace ReadingList.Infrastructure.Repositories;

public class InMemoryRepository<T, TKey> : IRepository<T>
{
    private Dictionary<TKey, T> _storage = new Dictionary<TKey, T>();
    private Func<T, TKey> _keySelector;

    public InMemoryRepository(Func<T, TKey> keySelector)
    {
        _keySelector = keySelector;
    }

    public void Add(T entity)
    {
        var key = _keySelector(entity);
        _storage[key] = entity;
    }

    public void Delete(T entity)
    {
        _storage.Remove(_keySelector(entity));
    }

    public IEnumerable<T> GetAll()
    {
        return _storage.Values;
    }

    public T GetById(int id)
    {
        if(_storage.TryGetValue((TKey)(object)id, out var entity))
            return entity;
        throw new KeyNotFoundException($"Entity with id {id} not found.");
    }

    public void Update(T entity)
    {
        var key = _keySelector(entity);
        if (_storage.ContainsKey(key))
        {
            _storage[key] = entity;
        }
        else
        {
            throw new KeyNotFoundException($"Entity with id {key} not found.");
        }
    }
}
