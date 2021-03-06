/// <reference path="tsUnit/tsUnit.ts" />
/// <reference path="ServicesViewModel.ts" />
import * as tsUnit from "./tsUnit/tsUnit";

window.onload = () => {
    var test = new tsUnit.Test();
    Tests.Composer.compose(test);
    test.run().showResults(document.getElementById("result"));
};

module Tests {
   export class Composer {
       static compose(test: tsUnit.Test) {
           // Using a composer means we don't need to export the test classes
           // and it means changes are isolated to this file
           test.addTestClass(new MetaThriftTests(), "MetaThriftTests");
           test.addTestClass(new ViewModelTests(), "ViewModelTests");
       }
   }

   export class MetaThriftTests extends tsUnit.TestClass {
      
       // ReSharper disable InconsistentNaming
       isNullOrEmpty_Null_should_be_true() {
           this.areIdentical(true, MetaThrift.isNullOrEmpty(null));
       }

       isNullOrEmpty_Empty_should_be_true() {
           this.areIdentical(true, MetaThrift.isNullOrEmpty(""));
       }

       unwrap_should_strip_serviceName() {
           const op = new MetaOperation();
           op.name = "MetaServer/sayHello";
           const unwrapped = MetaThrift.unwrap(op);
           this.areIdentical("sayHello", unwrapped.name);
       }

       unwrap_should_preserve() {
           const op = new MetaOperation();
           op.name = "MetaServer/numberOfDay";
           op.inputTypeName = "string";
           op.outputTypeName = "int";
           op.description = "Gets the weekday number for the specified Day";
           const unwrapped = MetaThrift.unwrap(op);
           this.areIdentical(op.inputTypeName, unwrapped.inputTypeName);
           this.areIdentical(op.outputTypeName, unwrapped.outputTypeName);
           this.areIdentical(op.displayName, unwrapped.displayName);
           this.areIdentical(op.description, unwrapped.description);
       }

       prettyPrint_should_be_pretty() {
           const op = new MetaOperation(); 
           op.name = "sayHello";
           op.inputTypeName = "string";
           op.outputTypeName = "string";
           op.description = "says hello to the specified user";
           let pretty = MetaThrift.prettyPrint(op);
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
       }
    }

   export class ViewModelTests extends tsUnit.TestClass {
       shouldFilterServices() {
           const info1 = new MetaServiceInfo();
           info1.name = "MetaServer";
           const info2 = new MetaServiceInfo();
           info2.name = "MyServer";
           const infos = [info1, info2];

           const op1 = new MetaOperation();
           op1.name = "MetaServer/searchDoodles";
           const op2 = new MetaOperation();
           op2.name = "MetaServer/sayHello";
           op2.inputTypeName = "string";
           const op3 = new MetaOperation();
           op3.name = "MyServer/doSomething";
           op3.outputTypeName = "string";
           const op4 = new MetaOperation();
           op4.name = "MyServer/inc";
           op4.inputTypeName = "int";
           op4.outputTypeName = "int";
           const ops = [op1, op2, op3, op4];

           const services = ServicesViewModel.filterServices(infos, ops);
           this.areIdentical(2, services.length);
           this.areIdentical("MetaServer", services[0].name);
           this.areIdentical("MyServer", services[1].name);
           this.areIdentical(2, services[0].operations.length);
           this.areIdentical(2, services[1].operations.length);

           this.areIdentical("searchDoodles", services[0].operations[0].name);
           this.areIdentical("sayHello", services[0].operations[1].name);
           this.areIdentical("doSomething", services[1].operations[0].name);
           this.areIdentical("inc", services[1].operations[1].name);
       }
   }
}
