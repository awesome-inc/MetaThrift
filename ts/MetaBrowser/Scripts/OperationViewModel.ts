class OperationViewModel {
    name: string;
    prettyName: string;
    constructor(public operation: MetaOperation) {
        const op = MetaThrift.unwrap(operation);
        this.name = op.name;
        this.prettyName = MetaThrift.prettyPrint(op);
    }
}
