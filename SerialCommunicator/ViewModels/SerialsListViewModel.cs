using SerialCommunicator.Controls;
using SerialCommunicator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SerialCommunicator.ViewModels
{
    public class SerialsListViewModel : BaseViewModel
    {
        public ObservableCollection<SerialItem> SerialItems { get; set; }

        private SerialItem _selectedItem;
        public SerialItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                RaisePropertyChanged(ref _selectedItem, value); 
                //ItemChanged?.Invoke(value);
            }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                RaisePropertyChanged(ref _selectedIndex, value);
            }
        }

        //public Action<SerialItem> ItemChanged { get; set; }
        public Action ScrollReceivedIntoViewCallback { get; set; }
        public Action ScrollSentIntoViewCallback { get; set; }
        public Action<string> MessageReceivedCallback { get; set; }

        public ICommand NewSerialItemCommand { get; set; }
        public ICommand RemoveSerialItemCommand { get; set; }

        public SerialsListViewModel()
        {
            SerialItems = new ObservableCollection<SerialItem>();
            NewSerialItemCommand = new Command(CreateSerialItem);
            RemoveSerialItemCommand = new Command(RemoveSerialItem);
        }

        public void CreateSerialItem()
        {
            SerialItem item = new SerialItem($"Serial {SerialItems.Count}");
            item.Close = this.RemoveSerialItem;
            AddSerialItem(item);
        }

        public void RemoveSerialItem()
        {
            if (SelectedItem != null)
                RemoveSerialItem(SelectedItem);
        }

        public void AddSerialItem(SerialItem item)
        {
            SerialItems.Add(item);
        }

        public void RemoveSerialItem(SerialItem item)
        {
            if (item.SerialView != null)
                item.SerialView.ShutdownEverything();
            SerialItems.Remove(item);
        }
    }
}
