using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MetaBrowser.Models.Services;
using MetaBrowser.Models.Services.Thrift;
using MetaBrowser.ViewModels;
using Microsoft.Win32;

namespace MetaBrowser
{

    public class AppBootstrapper : BootstrapperBase
    {
        CompositionContainer _container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ServicesViewModel>();
        }

        protected override void Configure()
        {
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            _container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue<Func<IMessageBox>>(() => _container.GetExportedValue<IMessageBox>());
            
            //batch.AddExportedValue<IMetaServiceProvider>(new DummyProvider());
            batch.AddExportedValue<IMetaServiceProvider>(new ThriftMetaServiceProvider());

            batch.AddExportedValue<IEnumerable<IMetaServiceExporter>>(new IMetaServiceExporter[]
                {
                    new MetaServiceXmlExporter()
                });
            batch.AddExportedValue<IFileDialogService>("SaveFileService", new FileDialogService(new SaveFileDialog()));
            
            batch.AddExportedValue(_container);
            batch.AddExportedValue(catalog);

            // wicked stuff, cf. http://caliburnmicro.codeplex.com/discussions/287228
            MessageBinder.SpecialValues.Add("$orignalsourcecontext", context =>
            {
                var args = context.EventArgs as RoutedEventArgs;
                if (args == null) return null;

                var fe = args.OriginalSource as FrameworkElement;
                if (fe == null) return null;

                return fe.DataContext; // should be of type IMetaServiceExporter
            });

            _container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract).ToList();

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }
    }

}