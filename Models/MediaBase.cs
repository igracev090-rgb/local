using System;
using MediaStation.Interfaces;

namespace MediaStation.Models;

// ПУНКТ 1: базовый класс иерархии. Реализует оба интерфейса —
// значит, любой наследник автоматически IPlayable + IVolumeControl + IComparable.
// abstract — нельзя создать «просто MediaBase».
public abstract class MediaBase : IPlayable, IVolumeControl
{
    private int _volume = 50;
    private int _balance = 0;

    // ПУНКТ 2: валидация общих полей
    protected MediaBase(string title, TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название не может быть пустым.", nameof(title));
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(duration), "Длительность должна быть положительной.");

        Title = title;
        Duration = duration;
    }

    public string Title { get; }
    public TimeSpan Duration { get; }
    public bool IsPlaying { get; protected set; }  // менять могут только наследники

    // Сеттер делегирует в SetVolume — обойти валидацию нельзя
    public int Volume
    {
        get => _volume;
        set => SetVolume(value);
    }

    public int Balance
    {
        get => _balance;
        set => SetBalance(value);
    }

    // ПУНКТ 2: методы воспроизведения (virtual — можно переопределить)
    public virtual void Play()
    {
        if (IsPlaying) return;   // идемпотентно
        IsPlaying = true;
    }

    public virtual void Pause()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
    }

    public virtual void Stop()
    {
        IsPlaying = false;
    }

    // ПУНКТ 2: единая точка валидации громкости/баланса
    public void SetVolume(int value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Громкость не должна быть в диапазоне 0..100.");
        _volume = value;
    }

    public void SetBalance(int value)
    {
        if (value < -100 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), "Баланс должен быть в диапазоне -100..100.");
        _balance = value;
    }

    // ПУНКТ 5: реализация стандартного IComparable<IPlayable>.
    // List<IPlayable>.Sort() без аргументов вызовет именно этот метод.
    // Критерий — длительность; при равенстве — по названию (детерминизм).
    public int CompareTo(IPlayable? other)
    {
        if (other is null) return 1;                     // любой объект > null
        if (ReferenceEquals(this, other)) return 0;      // тот же самый объект

        int byDuration = Duration.CompareTo(other.Duration);
        if (byDuration != 0) return byDuration;

        return string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() =>
        $"{GetType().Name}: {Title} ({Duration:hh\\:mm\\:ss}) vol={Volume} bal={Balance} playing={IsPlaying}";
}