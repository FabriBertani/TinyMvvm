﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TinyMvvm.IoC;
using TinyNavigationHelper;
using TinyPubSubLib;

namespace TinyMvvm
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private INavigationHelper _navigation;

        public ViewModelBase()
        {

        }

        public ViewModelBase(INavigationHelper navigation)
        {
            _navigation = navigation;
        }

        public INavigationHelper Navigation
        {
            get
            {
                if (_navigation == null && Resolver.IsEnabled)
                {
                    return Resolver.Resolve<INavigationHelper>();
                }
                else if (_navigation != null)
                {
                    return _navigation;
                }

                throw new NullReferenceException("Please pass a INavigation implementation to the constructor");
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsNotBusy");
            }
        }

        public bool IsNotBusy
        {
            get
            {
                return !IsBusy;
            }
        }

        public async Task PublishMessageAsync(string channel, string argument = null)
        {
            await TinyPubSub.PublishAsync(channel, argument);
        }

        public void SubscribeToMessageChannel(string channel, Action action)
        {
            TinyPubSub.Subscribe(channel, action);
        }

        public void SubscribeToMessageChannel(string channel, Action<string> action)
        {
            TinyPubSub.Subscribe(channel, action);
        }

        public void UnSubscribeFromMessageChannel(string channel)
        {
            TinyPubSub.Unsubscribe(channel);
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}