using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaStation.Interfaces;
using MediaStation.Models;
using MediaStation.Services;


namespace WpfApp2;

// ПУНКТ 4: code-behind окна. partial — вторая половина из XAML.
public partial class MainWindow : Window
{
    private readonly MediaLibrary _library = new();                     // ПУНКТ 3
    private readonly ObservableCollection<IPlayable> _playlist = new(); // авто-обновление ListBox
    private IPlayable? _current;                                        // что сейчас «играет»

    public MainWindow()
    {
        InitializeComponent();               // строит UI из XAML
        PlaylistBox.ItemsSource = _playlist; // привязка коллекции к списку
        SeedDemoData();                      // наполняем библиотеку
        RefreshGrid(_library.Items);         // показываем всё в DataGrid
    }

    // Демо-данные: разные типы — все упаковываются как IPlayable
    private void SeedDemoData()
    {
        try
        {
            _library.Add(new VideoPlayer("Интерстеллар", TimeSpan.FromMinutes(169), "4K", true));
            _library.Add(new VideoPlayer("Короткий клип", TimeSpan.FromMinutes(3), "1080p", false));
            _library.Add(new RadioStream("Европа Плюс", TimeSpan.FromHours(2), "106.2 FM", "Pop"));
            _library.Add(new RadioStream("Ретро FM", TimeSpan.FromMinutes(90), "98.4 FM", "Retro"));
            _library.Add(new Podcast("О технологиях #12", TimeSpan.FromMinutes(45), "Иван Петров", 12));
            _library.Add(new Podcast("История #3", TimeSpan.FromMinutes(60), "Анна Сидорова", 3));
        }
        catch (Exception ex)  // ПУНКТ 4: try-catch
        {
            MessageBox.Show($"Ошибка наполнения демо-данных: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // Единая точка обновления DataGrid. ToList() — материализуем LINQ-запрос
    private void RefreshGrid(IEnumerable<IPlayable> source)
        => LibraryGrid.ItemsSource = source.ToList();

    // ─── Поиск / фильтр / сортировка ───

    private void ApplyFilter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            IEnumerable<IPlayable> result = _library.Items;

            // ПУНКТ 3: поиск, если введён текст
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                result = _library.Search(SearchBox.Text);

            // ПУНКТ 3: фильтр по типу в зависимости от выбора ComboBox
            if (FilterCombo.SelectedIndex > 0)
            {
                result = FilterCombo.SelectedIndex switch
                {
                    1 => result.OfType<VideoPlayer>(),
                    2 => result.OfType<RadioStream>(),
                    3 => result.OfType<Podcast>(),
                    _ => result
                };
            }

            RefreshGrid(result);
        }
        catch (Exception ex)  // ПУНКТ 4: показываем ошибку, приложение не падает
        {
            MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ПУНКТ 3: сортировка через LINQ
    private void SortByTitle_Click(object sender, RoutedEventArgs e)
        => RefreshGrid(_library.SortByTitle());

    private void SortByDuration_Click(object sender, RoutedEventArgs e)
        => RefreshGrid(_library.SortByDuration());

    // ПУНКТ 5: сортировка через IComparable
    private void SortByComparable_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            RefreshGrid(_library.SortByComparable());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сортировки через IComparable: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ─── Мастер-плейлист ───

    // Копирует всю библиотеку в плейлист и последовательно запускает каждый объект.
    // Полиморфизм: item — IPlayable, но реально VideoPlayer/RadioStream/Podcast.
    private void PlayAll_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _playlist.Clear();
            foreach (var item in _library.Items)
                _playlist.Add(item);

            foreach (var item in _playlist)
            {
                _current?.Stop();   // остановить предыдущий
                _current = item;

                // Проверка «умеет ли объект управлять звуком?»
                VolumeSlider.Value = item is IVolumeControl vc ? vc.Volume : 50;
                BalanceSlider.Value = item is IVolumeControl bc ? bc.Balance : 0;

                item.Play();
            }

            PlaylistBox.SelectedIndex = _playlist.Count - 1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка воспроизведения плейлиста: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Pause_Click(object sender, RoutedEventArgs e)
    {
        try { _current?.Pause(); }   // ?. — безопасно, если _current == null
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            foreach (var item in _playlist)
                item.Stop();
            _current = null;
            PlaylistBox.SelectedIndex = -1;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
    }

    // ─── Громкость / баланс ───

    // Ловим КОНКРЕТНОЕ ArgumentOutOfRangeException — это то, что бросает SetVolume.
    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (VolumeLabel is null) return;   // слайдер срабатывает до InitializeComponent
        VolumeLabel.Text = ((int)e.NewValue).ToString();

        try
        {
            if (_current is IVolumeControl vc)
                vc.SetVolume((int)e.NewValue);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка валидации");
        }
    }

    private void BalanceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (BalanceLabel is null) return;
        BalanceLabel.Text = ((int)e.NewValue).ToString();

        try
        {
            if (_current is IVolumeControl vc)
                vc.SetBalance((int)e.NewValue);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            MessageBox.Show(ex.Message, "Ошибка валидации");
        }
    }
}

