import { BrowserModule } from '@angular/platform-browser';
import { NgModule, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { MatSelectModule } from "@angular/material/select";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { PingTransactionDataComponent } from './ping-transaction/ping-transaction.component';
import { CustomerDataComponent } from './customer-data/customer-data.component';
import { VehicleDataComponent } from './vehicle-data/vehicle-data.component';
import { CustomerEditComponent } from "./customer-edit/customer-edit.component";
import { VehicleEditComponent } from "./vehicle-edit/vehicle-edit.component";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PingTransactionDataComponent,
    CustomerDataComponent,
    VehicleDataComponent,
    CustomerEditComponent,
    VehicleEditComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    MatSelectModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'ping-transaction', component: PingTransactionDataComponent },
      { path: 'customer-data', component: CustomerDataComponent },
      { path: 'vehicle-data', component: VehicleDataComponent },
      { path: 'customer-edit', component: CustomerEditComponent },
      { path: 'vehicle-edit', component: VehicleEditComponent }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
