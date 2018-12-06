"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
exports.__esModule = true;
/// <reference path="tsUnit/tsUnit.ts" />
/// <reference path="ServicesViewModel.ts" />
var tsUnit = require("./tsUnit/tsUnit");
window.onload = function () {
    var test = new tsUnit.Test();
    Tests.Composer.compose(test);
    test.run().showResults(document.getElementById("result"));
};
var Tests;
(function (Tests) {
    var Composer = /** @class */ (function () {
        function Composer() {
        }
        Composer.compose = function (test) {
            // Using a composer means we don't need to export the test classes
            // and it means changes are isolated to this file
            test.addTestClass(new MetaThriftTests(), "MetaThriftTests");
            test.addTestClass(new ViewModelTests(), "ViewModelTests");
        };
        return Composer;
    }());
    Tests.Composer = Composer;
    var MetaThriftTests = /** @class */ (function (_super) {
        __extends(MetaThriftTests, _super);
        function MetaThriftTests() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        // ReSharper disable InconsistentNaming
        MetaThriftTests.prototype.isNullOrEmpty_Null_should_be_true = function () {
            this.areIdentical(true, MetaThrift.isNullOrEmpty(null));
        };
        MetaThriftTests.prototype.isNullOrEmpty_Empty_should_be_true = function () {
            this.areIdentical(true, MetaThrift.isNullOrEmpty(""));
        };
        MetaThriftTests.prototype.unwrap_should_strip_serviceName = function () {
            var op = new MetaOperation();
            op.name = "MetaServer/sayHello";
            var unwrapped = MetaThrift.unwrap(op);
            this.areIdentical("sayHello", unwrapped.name);
        };
        MetaThriftTests.prototype.unwrap_should_preserve = function () {
            var op = new MetaOperation();
            op.name = "MetaServer/numberOfDay";
            op.inputTypeName = "string";
            op.outputTypeName = "int";
            op.description = "Gets the weekday number for the specified Day";
            var unwrapped = MetaThrift.unwrap(op);
            this.areIdentical(op.inputTypeName, unwrapped.inputTypeName);
            this.areIdentical(op.outputTypeName, unwrapped.outputTypeName);
            this.areIdentical(op.displayName, unwrapped.displayName);
            this.areIdentical(op.description, unwrapped.description);
        };
        MetaThriftTests.prototype.prettyPrint_should_be_pretty = function () {
            var op = new MetaOperation();
            op.name = "sayHello";
            op.inputTypeName = "string";
            op.outputTypeName = "string";
            op.description = "says hello to the specified user";
            var pretty = MetaThrift.prettyPrint(op);
            this.areIdentical("string sayHello(string value); // says hello to the specified user", pretty);
            op.inputTypeName = "string";
            op.outputTypeName = "";
            op.description = "";
            pretty = MetaThrift.prettyPrint(op);
            this.areIdentical("void sayHello(string value);", pretty);
            op.inputTypeName = "";
            op.outputTypeName = "string";
            op.description = "";
            pretty = MetaThrift.prettyPrint(op);
            this.areIdentical("string sayHello();", pretty);
            op.inputTypeName = "";
            op.outputTypeName = "";
            pretty = MetaThrift.prettyPrint(op);
            this.areIdentical("void sayHello();", pretty);
        };
        return MetaThriftTests;
    }(tsUnit.TestClass));
    Tests.MetaThriftTests = MetaThriftTests;
    var ViewModelTests = /** @class */ (function (_super) {
        __extends(ViewModelTests, _super);
        function ViewModelTests() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        ViewModelTests.prototype.shouldFilterServices = function () {
            var info1 = new MetaServiceInfo();
            info1.name = "MetaServer";
            var info2 = new MetaServiceInfo();
            info2.name = "MyServer";
            var infos = [info1, info2];
            var op1 = new MetaOperation();
            op1.name = "MetaServer/searchDoodles";
            var op2 = new MetaOperation();
            op2.name = "MetaServer/sayHello";
            op2.inputTypeName = "string";
            var op3 = new MetaOperation();
            op3.name = "MyServer/doSomething";
            op3.outputTypeName = "string";
            var op4 = new MetaOperation();
            op4.name = "MyServer/inc";
            op4.inputTypeName = "int";
            op4.outputTypeName = "int";
            var ops = [op1, op2, op3, op4];
            var services = ServicesViewModel.filterServices(infos, ops);
            this.areIdentical(2, services.length);
            this.areIdentical("MetaServer", services[0].name);
            this.areIdentical("MyServer", services[1].name);
            this.areIdentical(2, services[0].operations.length);
            this.areIdentical(2, services[1].operations.length);
            this.areIdentical("searchDoodles", services[0].operations[0].name);
            this.areIdentical("sayHello", services[0].operations[1].name);
            this.areIdentical("doSomething", services[1].operations[0].name);
            this.areIdentical("inc", services[1].operations[1].name);
        };
        return ViewModelTests;
    }(tsUnit.TestClass));
    Tests.ViewModelTests = ViewModelTests;
})(Tests || (Tests = {}));
//# sourceMappingURL=tests.js.map