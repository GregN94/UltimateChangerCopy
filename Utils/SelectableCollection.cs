using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Utils
{
    public class SelectableCollection<T> : ObservableCollection<Selectable<T>>
    {
        public SelectableCollection(IEnumerable<T> source)
        : base(source.Select(e => new Selectable<T> {Selected = false, Value = e}))
        {
        }

        public bool AnySelected()
        {
            return Items.Any(i => i.Selected);
        }

        public int CountSelected()
        {
            return Items.Where(i => i.Selected).Count();
        }

        public IEnumerable<Selectable<T>> GetAllSelected()
        {
            return Items.Where(i => i.Selected);
        }

        public void ToggleSelection(T element)
        {
            int index = -1;
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].Value.Equals(element))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                return;
            }

            var selected = base[index].Selected ? false : true;

            base.SetItem(index, new Selectable<T>{Value = element, Selected = selected});
        }
    }
}
