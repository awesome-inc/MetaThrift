var ServiceViewModel = /** @class */ (function () {
    function ServiceViewModel(serviceInfo, operations) {
        this.serviceInfo = serviceInfo;
        this.name = (this.serviceInfo ? this.serviceInfo.name : "");
        this.operations = operations.map(function (o) { return new OperationViewModel(o); });
    }
    ServiceViewModel.empty = new ServiceViewModel(null, []);
    return ServiceViewModel;
}());
//# sourceMappingURL=ServiceViewModel.js.map