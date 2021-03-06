﻿using System;
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
            this.ItemsRight = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ItemViewModel> ItemsRight { get; private set; }

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
                ID = "203819",
                AItem = "A",
                AItemName = "Apple",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203819.jpg",
                url = "http://pantip.com/topic/32243815",
                ViewTotalNums = 1000,
                LikeTotalNums = 20,
                IsLike = false,
                IsCompleted = false
            });
            this.ItemsRight.Add(new ItemViewModel()
            {
                ID = "203818",
                AItem = "B",
                AItemName = "Banana",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203818.jpg",
                url = "http://pantip.com/topic/32241475",
                ViewTotalNums = 2000,
                LikeTotalNums = 21,
                IsLike = true,
                IsCompleted = false
            });
            this.Items.Add(new ItemViewModel()
            {
                ID = "203817",
                AItem = "C",
                AItemName = "Carrot",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203817.jpg",
                url = "http://pantip.com/topic/32241516",
                ViewTotalNums = 3000,
                LikeTotalNums = 22,
                IsLike = false,
                IsCompleted = false
            });
            this.ItemsRight.Add(new ItemViewModel()
            {
                ID = "203816",
                AItem = "B",
                AItemName = "Banana",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203816.jpg",
                url = "http://pantip.com/topic/32241196",
                ViewTotalNums = 4000,
                LikeTotalNums = 23,
                IsLike = true,
                IsCompleted = false
            });
            this.Items.Add(new ItemViewModel()
            {
                ID = "203806",
                AItem = "C",
                AItemName = "Carrot",
                ImageURI = "http://1081009.tourismthailand.org/oldweb/uploads/trips/n/203806.jpg",
                url = "http://pantip.com/topic/32241101",
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