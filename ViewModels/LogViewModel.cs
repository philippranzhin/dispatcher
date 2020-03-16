namespace DispatcherDesktop.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Data;
    using Domain.Logger;
    using Infrastructure.Models;
    using Prism.Mvvm;
    using Properties;

    public class LogViewModel : BindableBase
    {
        private readonly object logSync = new object();
        private ObservableCollection<UiLog> logs;

        public LogViewModel(ILogger logger)
        {
            this.logs = new ObservableCollection<UiLog>();

            Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
            {
                BindingOperations.EnableCollectionSynchronization(this.logs, this.logSync);
            }));

            logger.InfoLogged += (s, e) => this.AddLog(0, e);

            logger.ErrorLogged += (s, e) => this.AddLog(1, e);
        }

        public ObservableCollection<UiLog> Logs
        {
            get => this.logs;
            set => this.SetProperty(ref this.logs, value);
        }

        private void AddLog(int severity, string body)
        {
            try
            {
                lock (this.logSync)
                {
                    if (this.logs.Count >= Settings.Default.LogsMaxCount)
                    {
                        this.logs.RemoveAt(0);
                    }

                    this.Logs.Add(new UiLog(severity, body));
                }
            }
            catch
            {
                // so, where I should log this??
            }
        }
    }
} 
