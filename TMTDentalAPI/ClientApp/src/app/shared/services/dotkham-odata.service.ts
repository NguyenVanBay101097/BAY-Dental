import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

export class DotKhamLineDisplay {
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
