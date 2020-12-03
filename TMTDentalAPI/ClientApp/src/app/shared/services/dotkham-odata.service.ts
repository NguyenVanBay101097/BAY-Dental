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
  Reason = '';
  State: string;
  CompanyId: string;
  DoctorId: string;
  Doctor: any;
  Lines: any[] = [];
  DotKhamImages: any[] = [];
}

export class DotKhamSaveVm {
  Name: string;
  SaleOrderId: string;
  SaleOrder: any;
  PartnerId: string;
  Partner: any;
  Date: any;
  Reason = '';
  State: string;
  CompanyId: string;
  DoctorId: string;
  Doctor: any;
  Lines: any[] = [];
  DotKhamImages: any[] = [];
}

export class DotKhamLineDisplay {
  Id: string ;
  NameStep: string;
  DotKhamId: string = null;
  ProductId: string = null;
  Product: any = null;
  Sequence: number = null;
  State: string = null;
  ToothIds: string[] = [];
  Note: string = null;
  Teeth: any[] = [];
  SaleOrderLineId: string = null;
  SaleOrderLine: any = null;
}

@Injectable({
  providedIn: 'root'
})
export class DotkhamOdataService extends ODataService {
  constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) { super(http, baseUrl, 'DotKhams'); }

  createOrUpdateDotKham(val) {
    debugger
    return this.http.post(`${this.BASE_URL}${this.tableName}` + '/CreateOrUpdateDotKham', val);
  }
}
