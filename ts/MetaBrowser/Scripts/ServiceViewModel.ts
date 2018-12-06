class ServiceViewModel {
    name: string;
    operations: OperationViewModel[];
    constructor(public serviceInfo: MetaServiceInfo, operations: MetaOperation[]) {
        this.name = (this.serviceInfo ? this.serviceInfo.name : "");
        this.operations = operations.map(o => new OperationViewModel(o));
    }
    static empty = new ServiceViewModel(null, []);
}