using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStation.Models;

// ПУНКТ 1: конкретный класс №3 — уникальные поля для подкаста.
public class Podcast : MediaBase
{
    public string Host { get; }
    public int EpisodeNumber { get; }

    public Podcast(string title, TimeSpan duration, string host, int episodeNumber)
        : base(title, duration)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("Ведущий не указан.", nameof(host));

        // ПУНКТ 2: валидация номера эпизода
        if (episodeNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(episodeNumber), "Номер эпизода должен быть >= 1.");

        Host = host;
        EpisodeNumber = episodeNumber;
    }

    public override string ToString() =>
        base.ToString() + $" [host={Host}, ep={EpisodeNumber}]";
}
