import { Component, Inject, OnInit, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import { VehicleService } from "../../../services/vehicle-service";
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: "app-vehicle-edit",
  templateUrl: "./vehicle-edit.component.html"
})
@NgModule({
  declarations: [VehicleService],
  exports: [VehicleService]
})

export class VehicleEditComponent implements OnInit {
  vehicleServiceObject: VehicleService;
  vehicleId: number;
  vehicleCode: string;
  vehicleRegNum: string;
  customers: any[];
  selectedCustomerValue: number;

  vehicle: any;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private readonly route: ActivatedRoute, private readonly router: Router) {
    this.vehicleServiceObject = new VehicleService(http, baseUrl);
    console.log(this.route.snapshot.queryParams["id"]);
    this.vehicleId = this.route.snapshot.queryParams["id"];
  }

  ngOnInit(): void {
    this.getCustomers();
    this.vehicleServiceObject.getVehicle(this.vehicleId).subscribe(result => {
      console.log(result);
      this.vehicleCode = result.code;
      this.vehicleRegNum = result.regNumber;
      this.selectedCustomerValue = result.customerId;

      console.log(this.customers);

    });
  }

  getCustomers(): void {
    this.vehicleServiceObject.getAllCustomers().subscribe(result => {
      console.log("result " + result);
      this.customers = result as any[];
    },
      error => { console.log(error); });
  }

  editVehicle(): void {
    console.log(this.selectedCustomerValue);
    this.vehicleServiceObject.editVehicle(this.vehicleId, this.vehicleCode, this.vehicleRegNum, this.selectedCustomerValue).subscribe(result => {
      console.log(result.name);
      location.href = "/vehicle-data";
    });
  }

  backToVehicles() {
    this.router.navigate(["vehicle-data"]);
  }

  onCustomerSelect(cust: any): void {
    console.log(cust);
  }

  onCustomerDeSelect(cust: any): void {
    console.log(cust);
  }

}
