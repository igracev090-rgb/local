using System;
using System.Collections.Generic;
using System.Text;
using MediaStation.Interfaces;
using MediaStation.Models;

namespace MediaStation.Services;

// ПУНКТ 3: класс-менеджер списка объектов интерфейсного типа.
// Внутри могут лежать разные классы — снаружи работают как IPlayable.
public class MediaLibrary
{
    // readonly — сам список не подменить; List<IPlayable> — типобезопасно
    private readonly List<IPlayable> _items = new();

    // IReadOnlyList — снаружи только чтение, менять можно лишь через методы
    public IReadOnlyList<IPlayable> Items => _items;

    // CRUD
    public void Add(IPlayable item)
    {
        if (item is null) throw new ArgumentNullException(nameof(item));
        _items.Add(item);
    }

    public void Remove(IPlayable item) => _items.Remove(item);
    public void Clear() => _items.Clear();

    // ПУНКТ 3 (поиск): LINQ Where + Contains, регистронезависимо
    public IEnumerable<IPlayable> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Enumerable.Empty<IPlayable>();

        return _items.Where(i =>
            i.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    // ПУНКТ 3 (фильтр по типу): OfType<T> оставляет только нужный класс
    public IEnumerable<IPlayable> FilterByType<T>() where T : class, IPlayable
        => _items.OfType<T>();

    // ПУНКТ 3 (фильтр по диапазону длительности)
    public IEnumerable<IPlayable> FilterByDuration(TimeSpan min, TimeSpan max)
    {
        if (min > max)
            throw new ArgumentException("min не может быть больше max.");
        return _items.Where(i => i.Duration >= min && i.Duration <= max);
    }

    // ПУНКТ 3 (сортировка через LINQ): создаёт новую последовательность
    public IEnumerable<IPlayable> SortByTitle()
        => _items.OrderBy(i => i.Title, StringComparer.OrdinalIgnoreCase);

    public IEnumerable<IPlayable> SortByDuration()
        => _items.OrderBy(i => i.Duration);

    // ПУНКТ 5: List<T>.Sort() автоматически вызывает IComparable.CompareTo.
    // ToList() — чтобы не портить оригинальный _items (Sort сортирует на месте).
    public IEnumerable<IPlayable> SortByComparable()
    {
        var copy = _items.ToList();
        copy.Sort();
        return copy;
    }
}