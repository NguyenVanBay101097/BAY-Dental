import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

@Injectable({
  providedIn: 'root'
})
export class AccountPaymentsOdataService extends ODataService {

  constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, "AccountPayments"); }

  public getPrint(id: string) {
    return this.getFunction(id, "GetPrint");
  }

}
