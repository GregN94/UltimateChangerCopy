using FittingSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using Utils;


namespace ApplicationUI.ViewModels
{
    public class StatsViewModel : ObservableObject
    {
        public SelectableCollection<FS> FittingSoftwares { get; set; }

        private List<KeyValuePair<string, int>> myStats;
        public List<KeyValuePair<string, int>> MyStats
        {
            get => myStats;
            set
            {
                if (value != this.myStats)
                {
                    myStats = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand RefreshCommand { get; set; }
        public StatsViewModel(SelectableCollection<FS> fittingSoftwares)
        {
            FittingSoftwares = fittingSoftwares;
            RefreshCommand = new RelayCommand(RefreshGraph);
            MyStats = new List<KeyValuePair<string, int>>();
        }

        public void RefreshGraph(object obj)
        {
            List<KeyValuePair<string, int>> tmp = new List<KeyValuePair<string, int>>();
            foreach (var item in FittingSoftwares)
            {
                int i = 0;
                try
                {
                    i = item.Value.StatisticsManager.GetCountDaysInstallationLastFS();
                }
                catch (Exception)
                {

                }
                tmp.Add(new KeyValuePair<string, int>(item.Value.Name.ToString(), i));
            }
            MyStats = tmp;
        }

    }

}
