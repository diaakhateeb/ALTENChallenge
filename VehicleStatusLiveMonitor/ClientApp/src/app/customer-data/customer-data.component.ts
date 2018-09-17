import { Component, Inject, OnInit, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import { CustomerService } from "../../../services/customer-service";

@Component({
  selector: "app-customer-data",
  templateUrl: "./customer-data.component.html"
})
@NgModule({
  declarations: [CustomerService],
  exports: [CustomerService]
})

export class CustomerDataComponent implements OnInit {
  customerServiceObject: CustomerService;
  customerId: number;
  customerName: string;
  customerAddress: string;

  customers: any[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.customerServiceObject = new CustomerService(http, baseUrl);
  }

  ngOnInit(): void {
    this.getCustomers();
  }

  getCustomers(): void {
    this.customerServiceObject.getAllCustomers().subscribe(result => {
      console.log(result);
      this.customers = result as any[];
    },
      error => { console.log(error); });
  }

  addCustomer(): void {
    if (this.customerName.length === 0 || this.customerAddress.trim().length === 0 ||
      this.customerAddress.length === 0 || this.customerAddress.trim().length === 0) return;
    this.customerServiceObject.addCustomer(this.customerName, this.customerAddress).subscribe(result => {
      console.log(result);
      this.customers.push({ id: result.id, name: result.name, address: result.address });
      this.customerName = "";
      this.customerAddress = "";
      alert("Customer created successfully.");
    });
  }

  deleteCustomer(id: number): void {
    const confirmDelete = confirm("Are you sure you?");
    if (confirmDelete) {
      this.customerServiceObject.deleteCustomer(id).subscribe(result => {
        console.log(result);
        if (result === -1) {
          alert("Can not delete Customer. Please make sure that Customer has no Vehicle(s) attached.");
          return;
        }
        this.customers = this.customers.filter(el => el.id !== id);
        alert("Customer deleted successfully.");
      }, error => {
        console.log(error);
      });
    }
  }
}
