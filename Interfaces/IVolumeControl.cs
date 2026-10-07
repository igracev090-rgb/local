using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStation.Interfaces
{
    // ПУНКТ 1: второй интерфейс — управление звуком.
    // Отдельный от IPlayable (разделение ответственности).
    public interface IVolumeControl
    {
        int Volume { get; set; }   // 0..100
        int Balance { get; set; }  // -100 (лево) .. +100 (право)

        // ПУНКТ 2: методы с валидацией — UI ловит исключения в try-catch
        void SetVolume(int value);
        void SetBalance(int value);
    }
}
