using System;
using System.Diagnostics;
using Caliburn.Micro;
using MetaBrowser.Models.Entities;

namespace MetaBrowser.ViewModels
{
    public class ExecuteViewModel : Screen
    {
        private readonly IMetaService _service;
        private readonly IMessageBox _messageBox;
        private string _outputData;

        public ExecuteViewModel(IMetaService service, IMessageBox messageBox, MetaOperation operation)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (messageBox == null) throw new ArgumentNullException("messageBox");
            if (operation == null) throw new ArgumentNullException("operation");
            _service = service;
            _messageBox = messageBox;
            Operation = operation;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = String.Format("Execute \"{0}: {1}\"...", _service.Name, Operation.DisplayName);
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public MetaOperation Operation { get; private set; }
        public string InputType { get { return Operation.InputType; } }
        public string OutputType { get { return Operation.OutputType; } }
        public string InputData { get; set; }

        public string OutputData
        {
            get { return _outputData; }
            private set
            {
                _outputData = value;
                NotifyOfPropertyChange(() => OutputData);
            }
        }

        public void Execute()
        {
            try
            {
                var input = new MetaObject { Type = InputType, Data = InputData };
                OutputData = _service.Call(Operation, input).Data;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not execute: " + ex);
                _messageBox.Show("Could not execute: " + ex, "Error");
            }
        }
    }
}