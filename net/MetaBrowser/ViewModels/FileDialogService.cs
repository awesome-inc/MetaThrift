using System;
using System.ComponentModel.Composition;
using Microsoft.Win32;

namespace MetaBrowser.ViewModels
{
    [Export(typeof(IFileDialogService))]
    public class FileDialogService : IFileDialogService
    {
        private readonly FileDialog _dlg;

        [ImportingConstructor]
        public FileDialogService(FileDialog dlg)
        {
            if (dlg == null) throw new ArgumentNullException("dlg");
            _dlg = dlg;
        }

        public string Title
        {
            get { return _dlg.Title; }
            set
            {
                _dlg.Title = value; 
            }
        }

        public string FileName { get { return _dlg.FileName; } }

        public string Filter
        {
            get { return _dlg.Filter; }
            set
            {
                _dlg.Filter = value;
            }
        }

        public bool ShowDialog()
        {
            return (_dlg.ShowDialog() == true);
        }
    }
}