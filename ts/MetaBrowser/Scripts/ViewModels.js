/// <reference path="typings/knockout/knockout.d.ts" />
/// <reference path="MetaThrift.d.ts" />
/// <reference path="MetaThrift.ts" />
//declare var require;
//require(['MetaThrift.min'], function () { });
//declare var define;
//define(function () { return new ServicesViewModel(); });
var UiHelper = (function () {
    function UiHelper() {
    }
    UiHelper.handledCall = function (action, couldNot) {
        try {
            action();
        }
        catch (ex) {
            alert(couldNot + ": " + ex.name + " " + ex.message);
        }
    };
    return UiHelper;
})();
var OperationViewModel = (function () {
    function OperationViewModel(operation) {
        this.operation = operation;
        var op = MetaThrift.unwrap(operation);
        this.name = op.name;
        this.prettyName = MetaThrift.prettyPrint(op);
    }
    return OperationViewModel;
})();
var ServiceViewModel = (function () {
    function ServiceViewModel(serviceInfo, operations) {
        this.serviceInfo = serviceInfo;
        this.name = (this.serviceInfo ? this.serviceInfo.name : "");
        this.operations = operations.map(function (o) { return new OperationViewModel(o); });
    }
    ServiceViewModel.empty = new ServiceViewModel(null, []);
    return ServiceViewModel;
})();
var ServicesViewModel = (function () {
    function ServicesViewModel() {
        var _this = this;
        this.brokerUri = ko.observable("http://localhost:9091/services/metabroker/");
        this.validUri = ko.computed(function () {
            return !MetaThrift.isNullOrEmpty(_this.brokerUri());
        });
        this.broker = new MetaBrokerClient(MetaThrift.createProtocol(this.brokerUri()));
        this.services = ko.observableArray([]);
        this.selectedService = ko.observable(ServiceViewModel.empty);
        this.selectedOperation = ko.observable();
        this.operationSelected = ko.computed(function () {
            return (_this.selectedOperation() != null);
        });
        this.inputData = ko.observable("");
        this.outputData = ko.observable("");
    }
    ServicesViewModel.prototype.refreshServices = function () {
        var _this = this;
        this.services([]);
        this.selectedService(ServiceViewModel.empty);
        this.selectedOperation(null);
        UiHelper.handledCall(function () {
            _this.broker.input.getTransport().url = _this.brokerUri();
            _this.broker.output.getTransport().url = _this.brokerUri();
            var operations = _this.broker.getOperations();
            if (operations.length == 0) {
                alert("No MetaServices registered.");
                return;
            }
            var infos = _this.broker.getInfos();
            var services = ServicesViewModel.filterServices(infos, operations);
            _this.services(services);
        }, "Could not refresh services");
    };
    ServicesViewModel.prototype.showExecuteModal = function () {
        this.inputData("");
        this.outputData("");
    };
    ServicesViewModel.prototype.executeOperation = function () {
        var _this = this;
        var operation = this.selectedOperation().operation;
        UiHelper.handledCall(function () {
            var input = new MetaObject();
            input.typeName = operation.inputTypeName;
            input.data = _this.inputData();
            var output = _this.broker.call(operation, input);
            _this.outputData(output.data);
        }, "Could not execute operation");
    };
    ServicesViewModel.filterServices = function (infos, operations) {
        var services = infos.map(function (info) {
            var ops = operations.filter(function (o) { return o.name.indexOf(info.name + "/") == 0; });
            return new ServiceViewModel(info, ops);
        });
        return services;
    };
    return ServicesViewModel;
})();
//# sourceMappingURL=ViewModels.js.map