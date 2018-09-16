import { Component, Inject, OnInit, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import { VehicleService } from "../../../services/vehicle-service";
import { CustomerService } from "../../../services/customer-service";
import { Router } from '@angular/router';

@Component({
  selector: "app-vehicle-data",
  templateUrl: "./vehicle-data.component.html"
})
@NgModule({
  declarations: [VehicleService, CustomerService],
  exports: [VehicleService, CustomerService]
})

export class VehicleDataComponent implements OnInit {
  vehicleServiceObject: VehicleService;
  customerServiceObject: CustomerService;
  vehicleCode: string;
  vehicleRegNum: string;
  selectedCustomerValue: number;

  customers: any[];
  vehicles: any[];

  constructor(http: HttpClient, @Inject("BASE_URL") baseUrl: string, private readonly router: Router) {
    this.vehicleServiceObject = new VehicleService(http, baseUrl);
    this.customerServiceObject = new CustomerService(http, baseUrl);
  }

  ngOnInit(): void {
    this.getVehicles();
    this.getAllCustomers();
  }

  getVehicles(): void {
    this.vehicleServiceObject.getAllVehicles().subscribe(result => {
      this.vehicles = result as any[];
      console.log(this.vehicles);
    },
      error => { console.log(error); });
  }

  getAllCustomers(): void {
    this.customerServiceObject.getAllCustomers().subscribe(result => {
      this.customers = result as any[];
    },
      error => { console.log(error); });
  }

  addVehicle(): void {
    console.log(this.selectedCustomerValue);
    if (this.vehicleCode.length === 0 || this.vehicleCode.trim().length === 0 ||
      this.vehicleRegNum.length === 0 || this.vehicleRegNum.trim().length === 0 ||
      this.selectedCustomerValue === undefined) return;
    this.vehicleServiceObject.addVehicle(this.vehicleCode, this.vehicleRegNum, this.selectedCustomerValue).subscribe(result => {
      console.log(result);
      this.vehicles.push({ id: result.id, code: result.code, regNumber: result.regNumber, customerName: result.customerName });
      this.vehicleCode = "";
      this.vehicleRegNum = "";
      this.selectedCustomerValue = undefined;
      alert("Vehicle added successfully");
    });
  }

  editVehicle(id: number): void {
    this.customerServiceObject.editCustomer(id, "", "", null, null).subscribe(result => {
      console.log(result);
    });
  }

  deleteVehicle(id: number): void {
    const confirmDelete = confirm("Are you sure you?");
    if (confirmDelete) {
      this.vehicleServiceObject.deleteVehicle(id).subscribe(result => {
        console.log(result);
        console.log(this.vehicles);
        location.href = location.href;
      });
    }
  }

  onSubmit() {
    this.addVehicle();
  }
}
