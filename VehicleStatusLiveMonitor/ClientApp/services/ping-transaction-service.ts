import { Inject, Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import "rxjs/add/operator/map";
import { Observable } from "rxjs/Observable";

@Injectable()
export class PingTransactionService {
  private readonly httpClient: HttpClient;
  private readonly baseUrl: string;

  constructor(http: HttpClient, @Inject("BASE_URL") baseUrl: string) {
    this.httpClient = http;
    this.baseUrl = baseUrl;
  }

  getVehicleStatusTransById(pValue: number): Observable<any[]> {
    const data = { status: String(pValue) };
    return this.httpClient.get<any[]>(this.baseUrl + "api/VehicleConnectionStatusService/GetConnectionStatusTransById", { params: data });
  }
}

