using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStation.Models;

// ПУНКТ 1: конкретный класс №1 — уникальные поля для видео.
public class VideoPlayer : MediaBase
{
    public string Resalution { get; }
    public bool HasSubtitles { get; }

    // Сначала base(...) — валидация общих полей, потом свои
    public VideoPlayer(string title, TimeSpan duration, string resolution, bool hasSubtitles)
        : base(title, duration)
    {
        if (string.IsNullOrWhiteSpace(resolution))
            throw new ArgumentException("Разрешение не указано.", nameof(resolution));

        Resalution = resolution;
        HasSubtitles = hasSubtitles;
    }

    public override string ToString() =>
        base.ToString() + $" [{Resalution}, subs={HasSubtitles}]";
}