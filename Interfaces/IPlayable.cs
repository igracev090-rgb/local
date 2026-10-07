using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStation.Interfaces
{
    // ПУНКТ 1: первый интерфейс — контракт «воспроизводимый медиа-объект».
    // ПУНКТ 5: наследует стандартный IComparable<IPlayable> для сортировки.
    public interface IPlayable : IComparable<IPlayable>
    {
        string Title { get; }        // только get — задаётся в конструкторе
        TimeSpan Duration { get; }   // длительность
        bool IsPlaying { get; }      // состояние воспроизведения

        // ПУНКТ 2: поведение — реализуется в MediaBase
        void Play();
        void Pause();
        void Stop();
    }
}
