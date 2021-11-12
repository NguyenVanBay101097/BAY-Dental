import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';

export class PrintTemplateDefault {
  type: string;
}

@Injectable()
export class PrintTemplateService {

  apiUrl = 'api/PrintTemplates';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getPrintTemplateDefault(val: PrintTemplateDefault) {
    return this.http.post(this.baseApi + this.apiUrl + '/GetPrintTemplateDefault', val);
  }


}
