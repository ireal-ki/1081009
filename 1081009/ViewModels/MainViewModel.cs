using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using _1081009.Resources;

namespace _1081009.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /*
        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }
        */
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            this.Items.Add(new ItemViewModel()
            {
                ID = "0",
                AItem = "A",
                AItemName = "Apple",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203811.jpg",
                ViewTotalNums = 1000,
                LikeTotalNums = 20,
                IsLike = false,
                IsCompleted = false
            });
            this.Items.Add(new ItemViewModel()
            {
                ID = "1",
                AItem = "B",
                AItemName = "Banana",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203810.jpg",
                ViewTotalNums = 2000,
                LikeTotalNums = 21,
                IsLike = true,
                IsCompleted = false
            });
            this.Items.Add(new ItemViewModel()
            {
                ID = "2",
                AItem = "C",
                AItemName = "Carrot",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203808.jpg",
                ViewTotalNums = 3000,
                LikeTotalNums = 22,
                IsLike = false,
                IsCompleted = false
            });
            this.Items.Add(new ItemViewModel()
            {
                ID = "1",
                AItem = "B",
                AItemName = "Banana",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203807.jpg",
                ViewTotalNums = 4000,
                LikeTotalNums = 23,
                IsLike = true,
                IsCompleted = false
            });
            this.Items.Add(new ItemViewModel()
            {
                ID = "2",
                AItem = "C",
                AItemName = "Carrot",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203806.jpg",
                ViewTotalNums = 4000,
                LikeTotalNums = 24,
                IsLike = true,
                IsCompleted = false
            });
            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}