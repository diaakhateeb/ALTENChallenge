import { Inject, Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import "rxjs/add/operator/map";
import { Observable } from "rxjs/Observable";

@Injectable()
export class CustomerService {
  private readonly httpClient: HttpClient;
  private readonly baseUrl: string;
  headers = new Headers({
    'Cache-Control': 'no-cache',
    'Pragma': 'no-cache'
  });

  constructor(http: HttpClient, @Inject("BASE_URL") baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  getAllCustomers(): Observable<any[]> {
    return this.httpClient.get<any[]>(this.baseUrl + "api/CustomerService/GetCustomers");
  }

  getCustomer(id: number): Observable<any> {
    const data = { id: String(id) };
    return this.httpClient.get<any>(this.baseUrl + "api/CustomerService/GetCustomer", { params: data });
  }

  getAssociatedVehicles(id: number): Observable<any> {
    const data = { id: String(id) };
    return this.httpClient.get<any>(this.baseUrl + "api/CustomerService/GetAssociatedVehicles", { params: data });
  }

  getCustomerTransById(pValue: number): Observable<any[]> {
    const data = { id: String(pValue) };
    return this.httpClient.get<any[]>(this.baseUrl + "api/CustomerService/GetCustomerTransById",
      { headers: new HttpHeaders().set("Cache-Control", "no-cache").set("Pragma", "no-cache"), params: data });
  }

  addCustomer(name: string, address: string): Observable<any> {
    const data = { name: name, address: address };
    return this.httpClient.get<any>(this.baseUrl + "api/CustomerService/AddCustomer", { params: data });
  }

  editCustomer(id: number, name: string, address: string, currentVehiclesIds: string, newVehiclesIds: string): Observable<any> {
    console.log("customer-service: " + currentVehiclesIds);
    const data = {
      id: id,
      name: name,
      address: address,
      currentVehiclesIds: currentVehiclesIds === "undefined" ? "" : JSON.stringify(currentVehiclesIds),
      newVehiclesIds: newVehiclesIds === "undefined" ? "" : JSON.stringify(newVehiclesIds)
    };
    console.log("json: " + JSON.stringify(currentVehiclesIds));
    console.log(data);
    return this.httpClient.post<any>(this.baseUrl + "api/CustomerService/EditCustomer", data);
  }

  deleteCustomer(id: number): Observable<any> {
    const data = { id: String(id) };
    console.log(id);
    return this.httpClient.get<any>(this.baseUrl + "api/CustomerService/DeleteCustomer", { params: data });
  }
}

