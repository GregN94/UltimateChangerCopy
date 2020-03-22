using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace ApplicationUI.ViewModels
{
    public class TimeChangerViewModel : ViewModelBase
    {
        private bool modified;
        private Timer timer;

        public ICommand AddHourCommand { get; set; }
        public ICommand SubtractHourCommand { get; set; }
        public ICommand AddMinuteCommand { get; set; }
        public ICommand SubtractMinuteCommand { get; set; }
        public ICommand ApplyChangeCommand { get; set; }
        public ICommand ResetTimeCommand { get; set; }

        public System.DateTime SelectedDate { get; set; }

        public string Hour { get; set;}
        public string Minute { get; set; }

        public TimeChangerViewModel()
        {
            modified = false;
            timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.Elapsed += UpdateDateTime;
            timer.Start();

            AddHourCommand = new RelayCommand(AddHour);
            SubtractHourCommand = new RelayCommand(SubtractHour);
            AddMinuteCommand = new RelayCommand(AddMinute);
            SubtractMinuteCommand = new RelayCommand(SubtractMinute);
            ApplyChangeCommand = new RelayCommand(ApplyChange);
            ResetTimeCommand = new RelayCommand(ResetTime);
            SelectedDate = DateTime.Now;
            Minute = $"{SelectedDate.Minute:D2}";
            Hour = $"{SelectedDate.Hour:D2}";
        }

        private void UpdateDateTime(object sender, ElapsedEventArgs e)
        {
            if (modified == false)
            {
                SelectedDate = DateTime.Now;
                UpdateTime();
            }
            else
            {
                timer.Stop();
            }
        }

        private void ResetTime(object obj)
        {
            modified = false;
            UpdateDateTime(this, null);
            timer.Start();
        }

        private void ApplyChange(object obj)
        {
            // TODO: PAZE Add functionality to change windows date time
        }

        private void SubtractMinute(object obj)
        {
            modified = true;
            SelectedDate = SelectedDate.AddMinutes(-1);
            UpdateTime();
        }

        private void AddMinute(object obj)
        {
            modified = true;
            SelectedDate = SelectedDate.AddMinutes(1);
            UpdateTime();
        }

        private void SubtractHour(object obj)
        {
            modified = true;
            SelectedDate = SelectedDate.AddHours(-1);
            UpdateTime();
        }

        private void AddHour(object obj)
        {
            modified = true;
            SelectedDate = SelectedDate.AddHours(1);
            UpdateTime();
        }

        private void UpdateTime()
        {
            Minute = $"{SelectedDate.Minute:D2}";
            Hour = $"{SelectedDate.Hour:D2}";
            OnPropertyChanged(nameof(Minute));
            OnPropertyChanged(nameof(Hour));
        }
    }
}
