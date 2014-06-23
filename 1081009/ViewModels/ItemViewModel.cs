using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace _1081009.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        public const string LIKE_STRING = "*";
        public const string VIEW_STRING = "view : ";

        private string _aItem;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        public string AItem
        {
            get
            {
                return _aItem;
            }
            set
            {
                if (value != _aItem)
                {
                    _aItem = value;
                    NotifyPropertyChanged("AItem");
                }
            }
        }

        private string _aItemName;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        public string AItemName
        {
            get
            {
                return _aItemName;
            }
            set
            {
                if (value != _aItemName)
                {
                    _aItemName = value;
                    NotifyPropertyChanged("AItemName");
                }
            }
        }

        private int _likeTotalNums;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        public int LikeTotalNums
        {
            get
            {
                return _likeTotalNums;
            }
            set
            {
                if (value != _likeTotalNums)
                {
                    _likeTotalNums = value;
                    LikeTotal = value.ToString();
                    NotifyPropertyChanged("LikeTotalNums");
                }
            }
        }

        private string _likeTotal;
        public string LikeTotal
        {
            get
            {
                return _likeTotal;
            }
            set
            {
                if (value != _likeTotal)
                {
                    _likeTotal =  value;
                    NotifyPropertyChanged("LikeTotal");
                }
            }
        }

        private string _likeIconURI = "/Assets/Feed/icon-like-deactivate.png";
        public string LikeIconURI
        {
            get
            {
                return _likeIconURI;
            }
            set
            {
                if (value != _likeIconURI)
                {
                    _likeIconURI = value;
                    NotifyPropertyChanged("LikeIconURI");
                }
            }
        }

        private bool _isLike;
        public bool IsLike
        {
            get
            {
                return _isLike;
            }
            set
            {
                if (value != _isLike)
                {
                    _isLike = value;

                    NotifyPropertyChanged("IsLike");
                }

                LikeIconURI = _isLike ? "/Assets/Feed/icon-like-activate.png" : "/Assets/Feed/icon-like-deactivate.png";
            }
        }

        private bool _isCompleted;
        /// <summary>
        /// Used to determine if an item has been successfully entered or not
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            set
            {
                if (value != _isCompleted)
                {
                    _isCompleted = value;
                    NotifyPropertyChanged("IsCompleted");
                }
            }
        }

        private string _imageURI;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        public string ImageURI
        {
            get
            {
                return _imageURI;
            }
            set
            {
                if (value != _imageURI)
                {
                    _imageURI = value;
                    NotifyPropertyChanged("ImageURI");
                }
            }
        }

        private int _viewTotalNums;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        public int ViewTotalNums
        {
            get
            {
                return _viewTotalNums;
            }
            set
            {
                if (value != _viewTotalNums)
                {
                    _viewTotalNums = value;
                    ViewTotal = value.ToString();
                    NotifyPropertyChanged("ViewTotalNums");
                }
            }
        }

        private string _viewTotalString;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        public string ViewTotal
        {
            get
            {
                return "views : " + _viewTotalNums;
            }
            set
            {
                if (value != _viewTotalString)
                {
                    _viewTotalString = value;
                    NotifyPropertyChanged("ViewTotal");
                }
            }
        }

        private string _id;
        /// <summary>
        /// Sample ViewModel property; this property is used to identify the object.
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
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