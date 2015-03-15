using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaThrift
{
    public class DynamicMetaService : AbstractMetaService
    {
        private readonly Dictionary<MetaOperation, Func<object, object>> _registeredOperations = 
            new Dictionary<MetaOperation, Func<object, object>>();

        public DynamicMetaService(string name, string displayName = null, string description = null)
        {
            Name = name;
            DisplayName = displayName ?? String.Empty;
            Description = description ?? String.Empty;
        }

        public override IEnumerable<MetaOperation> Operations { get { return _registeredOperations.Keys.ToList(); } }

        public override object Call(MetaOperation operation, object value)
        {
            var onFunction = _registeredOperations[operation];
            return onFunction(value); // casting is done by internal anonymous function
        }

        public void RegisterAction<TInput>(string name, Action<TInput> action, string displayName = null, string description = null)
        {
            var key = name.ToMetaAction<TInput>(displayName, description);
            _registeredOperations.Add(key, o => { action((TInput)o); return null; });
        }

        public void RegisterAction(string name, Action action, string displayName = null, string description = null)
        {
            var key = name.ToMetaAction(displayName, description);
            _registeredOperations.Add(key, o => { action(); return null; });
        }

        public void UnregisterAction<TInput>(string name)
        {
            var key = name.ToMetaAction<TInput>();
            if (!_registeredOperations.Remove(key))
                throw new System.ArgumentException("There is no operation with the specified parameters that could be unregistered.");
        }

        public void UnregisterAction(string name)
        {
            var key = name.ToMetaAction();
            if (!_registeredOperations.Remove(key))
                throw new System.ArgumentException("There is no operation with the specified parameters that could be unregistered.");
        }

        public void RegisterFunc<TInput, TOutput>(string name, Func<TInput, TOutput> func, string displayName = null, string description = null)
        {
            var key = name.ToMetaFunction<TInput, TOutput>(displayName, description);
            _registeredOperations.Add(key, o => func((TInput)o));
        }

        public void RegisterFunc<TOutput>(string name, Func<TOutput> func, string displayName = null, string description = null)
        {
            var key = name.ToMetaFunction<TOutput>(displayName, description);
            _registeredOperations.Add(key, o => func());
        }

        public void UnregisterFunc<TInput, TOutput>(string name)
        {
            var key = name.ToMetaFunction<TInput, TOutput>();
            if (!_registeredOperations.Remove(key))
                throw new System.ArgumentException("There is no operation with the specified parameters that could be unregistered.");
        }

        public void UnregisterFunc<TOutput>(string name)
        {
            var key = name.ToMetaFunction<TOutput>();
            if (!_registeredOperations.Remove(key))
                throw new System.ArgumentException("There is no operation with the specified parameters that could be unregistered.");
        }
    }
}