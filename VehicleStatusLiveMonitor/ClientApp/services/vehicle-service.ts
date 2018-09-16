import { Inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import "rxjs/add/operator/map";
import { Observable } from "rxjs/Observable";

@Injectable()
export class VehicleService {
  private readonly httpClient: HttpClient;
  private readonly baseUrl: string;

  constructor(http: HttpClient, @Inject("BASE_URL") baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  getAllVehicles(): Observable<any[]> {
    return this.httpClient.get<any[]>(this.baseUrl + "api/VehicleService/GetVehicles");
  }

  getAllVehiclesExt(): Observable<any[]> {
    return this.httpClient.get<any[]>(this.baseUrl + "api/VehicleService/GetVehiclesExt");
  }

  getAllCustomers(): Observable<any[]> {
    return this.httpClient.get<any[]>(this.baseUrl + "api/CustomerService/GetCustomers");
  }

  getVehicle(id: number): Observable<any> {
    const data = { id: String(id) };
    return this.httpClient.get<any>(this.baseUrl + "api/VehicleService/GetVehicle", { params: data });
  }

  getVehicleTransById(pValue: number): Observable<any[]> {
    const data = { id: String(pValue) };
    return this.httpClient.get<any[]>(this.baseUrl + "api/VehicleService/GetVehicleTransById", { params: data });
  }

  addVehicle(code: string, regNum: string, customerId: number): Observable<any> {
    const data = { code: code, regNum: regNum, customerId: String(customerId) };
    return this.httpClient.get<string>(this.baseUrl + "api/VehicleService/AddVehicle", { params: data });
  }

  editVehicle(id: number, code: string, regNum: string, customerId: number): Observable<any> {
    const data = { id: String(id), code: code, regNum: regNum, customerId: String(customerId) };
    return this.httpClient.get<any>(this.baseUrl + "api/VehicleService/EditVehicle", { params: data });
  }

  deleteVehicle(id: number): Observable<any> {
    const data = { id: String(id) };
    console.log(id);
    return this.httpClient.get<any>(this.baseUrl + "api/VehicleService/DeleteVehicle", { params: data });
  }

  getUnassociatedVehicles(): Observable<any[]> {
    return this.httpClient.get<any[]>(this.baseUrl + "api/VehicleService/GetUnassociatedVehicles");
  }
}

