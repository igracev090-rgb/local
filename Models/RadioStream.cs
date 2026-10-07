using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStation.Models;

// ПУНКТ 1: конкретный класс №2 — уникальные поля для радио.
public class RadioStream : MediaBase
{
    public string Frequency { get; }
    public string Genre { get; }

    public RadioStream(string title, TimeSpan duration, string frequency, string genre)
        : base(title, duration)
    {
        if (string.IsNullOrWhiteSpace(frequency))
            throw new ArgumentException("Частота не указана.", nameof(frequency));

        Frequency = frequency;
        Genre = genre ?? "Unknown";
    }

    public override string ToString() =>
        base.ToString() + $" [{Frequency}, {Genre}]";
}