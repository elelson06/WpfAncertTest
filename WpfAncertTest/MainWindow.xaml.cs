using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WpfAncertTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ICronometroService cronometroService;
        private string tiempoActual;

        public event PropertyChangedEventHandler PropertyChanged;
        [STAThread]
        public static void Main()
        {
            MainWindow mainWindow = new MainWindow(new CronometroService());
            mainWindow.ShowDialog();
        }
        public string TiempoActual
        {
            get { return tiempoActual; }
            set
            {
                tiempoActual = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TiempoActual"));
            }
        }
        public MainWindow(ICronometroService cronometroService)
        {
            InitializeComponent();
            DataContext = this;

            this.cronometroService = cronometroService;
            this.cronometroService.TiempoActualizado += CronometroService_TiempoActualizado;

            TiempoActual = "00:00:00";
        }
        private void CronometroService_TiempoActualizado(object sender, TimeSpan tiempo)
        {
            TiempoActual = tiempo.ToString(@"hh\:mm\:ss");
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            cronometroService.Iniciar();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            cronometroService.Pausar();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            cronometroService.Detener();
        }
    }

    public interface ICronometroService
    {
        event EventHandler<TimeSpan> TiempoActualizado;
        void Iniciar();
        void Pausar();
        void Detener();
    }

    public class CronometroService : ICronometroService
    {
        private readonly DispatcherTimer timer;
        private TimeSpan tiempo;

        public event EventHandler<TimeSpan> TiempoActualizado;

        public CronometroService()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tiempo = tiempo.Add(TimeSpan.FromSeconds(1));
            TiempoActualizado?.Invoke(this, tiempo);
        }

        public void Iniciar()
        {
            timer.Start();
        }

        public void Pausar()
        {
            timer.Stop();
        }

        public void Detener()
        {
            timer.Stop();
            Thread.Sleep(1000);
            tiempo = TimeSpan.Zero;
            TiempoActualizado?.Invoke(this, tiempo);
        }
    }
}
