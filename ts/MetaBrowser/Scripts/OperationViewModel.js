var OperationViewModel = /** @class */ (function () {
    function OperationViewModel(operation) {
        this.operation = operation;
        var op = MetaThrift.unwrap(operation);
        this.name = op.name;
        this.prettyName = MetaThrift.prettyPrint(op);
    }
    return OperationViewModel;
}());
//# sourceMappingURL=OperationViewModel.js.map