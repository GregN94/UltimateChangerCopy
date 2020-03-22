namespace Utils
{
    public class Selectable<T> : ObservableObject
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }

        public void ToggleSelection()
        {
            Selected = Selected ? false : true;
        }
    }
}