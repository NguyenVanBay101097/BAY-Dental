import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

export class DotKhamVm {
  Id: string;
  Name: string;
  SaleOrderId: string;
  SaleOrder: any;
  PartnerId: string;
  Partner: any;
  Date: any;
  Reason: string;
  State: string;
  CompanyId: string;
  DoctorId: string;
  Doctor: any;
  Lines: any;
  Steps: any;
  DotKhamImages: any;
}

export class DotKhamLineDisplay{
  Id: string;
  Name: string;
  DotKhamId: string;
  ProductId: string;
  Product: any;
  Sequence: number;
  State: string;
  ToothIds: string[];
}

@Injectable({
  providedIn: 'root'
})
export class DotkhamOdataService extends ODataService {
  constructor(http: HttpClient,  @Inject('BASE_API') baseUrl: string) {super(http, baseUrl, 'DotKhams'); }
}
