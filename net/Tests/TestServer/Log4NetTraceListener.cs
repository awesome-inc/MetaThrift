using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;

// makes log4net parse the app.config (no call needed: log4net.Config.XmlConfigurator.Configure())

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TestServer
{
    /// <summary>
    /// Trace listener that will write all trace messages to log4net. This listener makes sure not
    /// to break existing trace functionality.
    /// </summary>
    sealed class Log4NetTraceListener : TraceListener
    {
        private const string ItemsSourceTimingIssueTrace = "ContentAlignment; DataItem=null;";
        private readonly ILog _log;

        private List<TraceSource> TraceSourceCollection { get; set; }
        public TraceLevel ActiveTraceLevel { get; set; }

        public Log4NetTraceListener() : this(LogManager.GetLogger("TraceOutput")) 
        { }

        public Log4NetTraceListener(ILog log)
        {
            if (log == null) throw new ArgumentNullException("log");
            _log = log;
            // Set default values
            Name = "Log4net Trace Listener";
            ActiveTraceLevel = ToTraceLevel(_log);

            AddAdditonalTraceSources();
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, 
            params object[] args)
        {
            TraceEvent(eventCache, source, eventType, id, string.Format(format, args));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            // Should we ignore this line?
            if (message.Contains(ItemsSourceTimingIssueTrace)) return;

            // Check the type
            switch (eventType)
            {
                case TraceEventType.Error:
                    if (ActiveTraceLevel >= TraceLevel.Error)
                        _log.Error(message);
                    break;

                case TraceEventType.Warning:
                    if (ActiveTraceLevel >= TraceLevel.Warning)
                        _log.Warn(message);
                    break;

                case TraceEventType.Information:
                    if (ActiveTraceLevel >= TraceLevel.Info)
                        _log.Info(message);
                    break;

                    // Everything else is verbose
                    //case TraceEventType.Verbose:
                default:
                    if (ActiveTraceLevel == TraceLevel.Verbose)
                        _log.Debug(message);
                    break;
            }
        }

        public override void Write(string message)
        {
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            // Invoke the event (but only when output is set to verbose)
            if (ActiveTraceLevel == TraceLevel.Verbose)
            {
                _log.Debug(message);
            }
        }

        private static TraceLevel ToTraceLevel(ILog logger)
        {
            if (logger.IsDebugEnabled) return TraceLevel.Verbose;
            if (logger.IsInfoEnabled) return TraceLevel.Info;
            if (logger.IsWarnEnabled) return TraceLevel.Warning;
            if (logger.IsErrorEnabled || logger.IsFatalEnabled) return TraceLevel.Error;
            return TraceLevel.Off;
        }

        [Conditional("DEBUG")] // trace data binding errors only in debug mode
        private void AddAdditonalTraceSources()
        {
            // Create additional trace sources list
            TraceSourceCollection = new List<TraceSource>
            {
                // WPF
                //PresentationTraceSources.DataBindingSource,
                //PresentationTraceSources.DependencyPropertySource,
                //PresentationTraceSources.MarkupSource,
                //PresentationTraceSources.ResourceDictionarySource
            };

            // Add additional trace sources - .NET framework
            //TraceSources.Add(System.Diagnostics.

            // Add additional trace sources - WPF

            // Subscribe to all trace sources
            foreach (var traceSource in TraceSourceCollection)
                traceSource.Listeners.Add(this);
        }
    }
}