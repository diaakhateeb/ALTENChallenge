import { Component, Inject, OnInit, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import { CustomerService } from "../../../services/customer-service";
import { VehicleService } from "../../../services/vehicle-service";
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: "app-customer-edit",
  templateUrl: "./customer-edit.component.html"
})
@NgModule({
  declarations: [CustomerService, VehicleService],
  exports: [CustomerService, VehicleService]
})

export class CustomerEditComponent implements OnInit {
  customerServiceObject: CustomerService;
  vehicleServiceObject: VehicleService;
  customerId: number;
  customerName: string;
  customerAddress: string;
  currentVehicleValues: any;
  newVehicleValues: any;

  customer: any;
  vehicles: any[];
  newVehicles: any[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: ActivatedRoute, private readonly router: Router) {
    this.customerServiceObject = new CustomerService(http, baseUrl);
    this.vehicleServiceObject = new VehicleService(http, baseUrl);
    console.log(this.route.snapshot.queryParams["id"]);
    this.customerId = this.route.snapshot.queryParams["id"];
  }

  ngOnInit(): void {
    this.customerServiceObject.getCustomer(this.customerId).subscribe(result => {
      console.log(result.name);
      this.customerName = result.name;
      this.customerAddress = result.address;
    });
    this.getAssociatedVehicles();
    this.getUnassociatedVehicles();
  }

  editCustomer(): void {
    this.customerServiceObject.editCustomer(this.customerId, this.customerName, this.customerAddress, String(this.currentVehicleValues),
      String(this.newVehicleValues)).subscribe(result => {
        console.log("current " + this.currentVehicleValues + "..." + typeof (this.currentVehicleValues));
        console.log(String(this.currentVehicleValues).split(","));
        console.log("new " + this.newVehicleValues);
        this.router.navigate(["/customer-data"]);
      });
  }

  getAssociatedVehicles(): void {
    this.customerServiceObject.getAssociatedVehicles(this.customerId).subscribe(result => {
      this.vehicles = result as any[];
      console.log(result);
    },
      error => { console.log(error); });
  }

  getUnassociatedVehicles(): void {
    this.vehicleServiceObject.getUnassociatedVehicles().subscribe(result => {
      this.newVehicles = result as any[];
      console.log(result);
    },
      error => { console.log(error); });
  }

  backToCustomers(): void {
    this.router.navigate(["/customer-data"]);
  }
}
