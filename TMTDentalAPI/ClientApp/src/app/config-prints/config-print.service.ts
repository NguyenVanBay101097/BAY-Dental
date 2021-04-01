import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { PrintPaperSizeBasic } from './print-paper-size.service';

export class ConfigPrintBasic {
  id:string;
  name:string;
  code:string;
  paperSizeId:string;
  printPaperSize: PrintPaperSizeBasic;
  isInfoCompany: boolean;
}

export class ConfigPrintSave {
  id:string;
  name:string;
  paperSizeId:string;
  isInfoCompany: boolean;
  companyId:string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfigPrintService {
  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/ConfigPrints";

  get(){
    return this.http.get(this.base_api + this.apiUrl );
  }

  createOrUpdate(val){
    return this.http.post(this.base_api + this.apiUrl , val);
  }

}
