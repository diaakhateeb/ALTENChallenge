import { Component, Inject, OnInit, NgModule, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import 'rxjs/add/operator/map';
import { CustomerService } from "../../../services/customer-service";
import { VehicleService } from "../../../services/vehicle-service";
import { PingTransactionService } from "../../../services/ping-transaction-service";
import { TimerService } from "../../../services/timer-service";

@Component({
  selector: 'app-vehicle-status',
  templateUrl: './ping-transaction.component.html',
  styleUrls: ["./ping-transaction.component.css"]
})
@NgModule({
  declarations: [CustomerService, VehicleService, PingTransactionService, TimerService],
  exports: [CustomerService, VehicleService, PingTransactionService, TimerService]
})
export class PingTransactionDataComponent implements OnInit, OnDestroy {
  customerServiceObject: CustomerService;
  vehicleServiceObject: VehicleService;
  pingTransactionServiceObject: PingTransactionService;
  timerServiceObject: TimerService;

  vehicleStatuses: any[];
  customers: any[];
  vehicles: any[];
  customerSubscriptionObject: object = null;
  vehicleSubscriptionObject: object = null;
  nextTickTime: string;
  selectedCustomerValue = 0;
  selectedVehicleValue = 0;
  selectedStatusValue = -1;
  intervalObject = null;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.customerServiceObject = new CustomerService(http, baseUrl);
    this.vehicleServiceObject = new VehicleService(http, baseUrl);
    this.pingTransactionServiceObject = new PingTransactionService(http, baseUrl);
    this.timerServiceObject = new TimerService();

    console.log("in constructor");
    this.getAllCustomers();
    this.getAllVehicles();
  }

  ngOnInit(): void {
    console.log("in ngOnInit");

    this.init();

    this.intervalObject = setInterval(() => {
      console.log("in interval");
      this.vehicleStatuses = [];
      if (this.selectedCustomerValue !== 0) this.getCustomerTransById(this.selectedCustomerValue);
      else if (this.selectedVehicleValue !== 0) this.getVehicleTransById(this.selectedVehicleValue);
      else this.getVehicleStatusTransById(this.selectedStatusValue);

      this.nextTickTime = this.timerServiceObject.getNextTime();

    },
      10000);
  }

  ngOnDestroy(): void {
    // Destroy Observables to avoid memory leaks.
    this.customerSubscriptionObject = null;
    this.vehicleSubscriptionObject = null;
    this.vehicleStatuses = null;

    clearInterval(this.intervalObject);
    this.intervalObject = null;
  }

  init(): void {
    this.vehicleStatuses = [];
    this.intervalObject = null;
    this.getVehicleStatusTransById(this.selectedStatusValue);
    this.nextTickTime = this.timerServiceObject.getNextTime();
  }

  getAllCustomers(): void {
    this.customerSubscriptionObject = this.customerServiceObject.getAllCustomers().subscribe(result => {
      this.customers = result as any[];
    },
      error => { console.log(error); });
  }

  getCustomerTransById(pValue: number): void {
    this.selectedVehicleValue = 0;
    this.selectedStatusValue = -1;
    this.selectedCustomerValue = pValue;

    this.customerServiceObject.getCustomerTransById(pValue).subscribe(result => {
      console.log("am filtered");
      this.vehicleStatuses = result as any[];
    },
      error => { console.log(error) });
  }

  getAllVehicles(): void {
    this.vehicleSubscriptionObject = this.vehicleServiceObject.getAllVehiclesExt().subscribe(result => {
      this.vehicles = result as any[];
    },
      error => { console.log(error); });
  }

  getVehicleTransById(pValue: number): void {
    this.selectedCustomerValue = 0;
    this.selectedStatusValue = -1;
    this.selectedVehicleValue = pValue;

    this.vehicleServiceObject.getVehicleTransById(pValue).subscribe(result => {
      this.vehicleStatuses = result as any[];
    },
      error => { console.log(error) });
  }

  getVehicleStatusTransById(pValue: number): void {
    this.selectedCustomerValue = 0;
    this.selectedVehicleValue = 0;
    this.selectedStatusValue = pValue;

    this.pingTransactionServiceObject.getVehicleStatusTransById(pValue).subscribe(result => {
      this.vehicleStatuses = result as any[];
      console.log(this.vehicleStatuses);
    },
      error => { console.log(error) });
  }

  setVehicleStatusColor(status) {
    if (status === "On") {
      return "#78e08f";
    }
    return "#e74c3c";
  }

  clearInterval() {
    clearInterval(this.intervalObject);
    this.intervalObject = null;
  }
}
