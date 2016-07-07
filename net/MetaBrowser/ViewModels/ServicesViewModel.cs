using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using MetaBrowser.Models.Entities;
using MetaBrowser.Models.Services;
using Action = System.Action;

namespace MetaBrowser.ViewModels
{
    [Export(typeof(ServicesViewModel))]
    public class ServicesViewModel : Screen
    {
        private readonly IMetaServiceProvider _serviceProvider;
        private readonly IFileDialogService _saveFileDialogService;
        private readonly IWindowManager _windowManager;
        private readonly IMessageBox _messageBox;

        private readonly BindableCollection<IMetaService> _services = new BindableCollection<IMetaService>();
        private IMetaService _selectedService;
        private MetaOperation _selectedOperation;

        private readonly BindableCollection<IMetaServiceExporter> _serviceExporters =
            new BindableCollection<IMetaServiceExporter>();

        [ImportingConstructor]
        public ServicesViewModel(IWindowManager windowManager, 
            IMetaServiceProvider serviceProvider, 
            IEnumerable<IMetaServiceExporter> serviceExporters,
            [Import("SaveFileService")]IFileDialogService saveFileDialogService,
            IMessageBox messageBox)
        {
            if (windowManager == null) throw new ArgumentNullException("windowManager");
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");
            if (serviceExporters == null) throw new ArgumentNullException("serviceExporters");
            if (saveFileDialogService == null) throw new ArgumentNullException("saveFileDialogService");
            if (messageBox == null) throw new ArgumentNullException("messageBox");

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "MetaBrowser";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor

            _windowManager = windowManager;
            _messageBox = messageBox;
            _serviceProvider = serviceProvider;
            _saveFileDialogService = saveFileDialogService;

            ServiceExporters.AddRange(serviceExporters);
        }

        public string Uri
        {
            get { return _serviceProvider.Uri; }
            set
            {
                if (_serviceProvider.Uri == value) return;
                _serviceProvider.Uri = value;
                NotifyOfPropertyChange(() => Uri);
            }
        }

        public BindableCollection<IMetaService> Services { get { return _services; } }

        public IMetaService SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                NotifyOfPropertyChange(() => SelectedService);
                NotifyOfPropertyChange(() => CanExport);

                SelectedOperation = _selectedService != null && _selectedService.Operations != null
                                       ? _selectedService.Operations.FirstOrDefault()
                                       : null;
            }
        }

        public MetaOperation SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                _selectedOperation = value;
                NotifyOfPropertyChange(() => SelectedOperation);
                NotifyOfPropertyChange(() => CanExecute);
            }
        }

        public BindableCollection<IMetaServiceExporter> ServiceExporters
        {
            get { return _serviceExporters; }
        }

        public void RefreshServices()
        {
            PerformUiAction(() =>
                {
                    _services.Clear();
                    _services.AddRange(_serviceProvider.Refresh());
                    NotifyOfPropertyChange(() => Services);

                    SelectedService = _services.FirstOrDefault();

                    if (_services.Count == 0)
                        _messageBox.Show("No (valid) MetaServices.", "Information");
                }, "Could not refresh service list");
        }

        private void PerformUiAction(Action action, string couldNot)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.TraceError("{0} : {1}", couldNot, ex);
                _messageBox.Show(ex.Message, "Error");
            }
        }

        public bool CanExport { get { return SelectedService != null; } }

        public void Export(IMetaServiceExporter exporter)
        {
            PerformUiAction(() =>
                {
                    _saveFileDialogService.Filter = GetFileFilter(exporter);
                    if (_saveFileDialogService.ShowDialog())
                    {
                        var fileName = _saveFileDialogService.FileName;
                        using (var stream = new FileStream(fileName, FileMode.Create))
                            exporter.Export(SelectedService, stream);
                    }
                }, "Could not export service interface snapshot");
        }

        private static string GetFileFilter(IMetaServiceExporter exporter)
        {
            return String.Format("{0} files (*.{1}|*.{1}",
                exporter.Info.Name, exporter.Info.Format);
        }

        public bool CanExecute
        {
            get { return SelectedService != null && SelectedOperation != null; }
        }

        public void Execute()
        {
            var viewModel = new ExecuteViewModel(SelectedService, _messageBox, SelectedOperation);
            _windowManager.ShowDialog(viewModel);
        }
    }
}
