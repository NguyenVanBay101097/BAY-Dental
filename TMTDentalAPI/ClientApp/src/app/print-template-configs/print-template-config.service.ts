import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { PrintPaperSizeDisplay } from '../config-prints/print-paper-size.service';

export class PrintTemplateConfigDisplay {
  content: string;
  type: string;
  printPaperSizeId: string;
  printPaperSize: PrintPaperSizeDisplay;
  companyId: string;
}

export class PrintTemplateConfigSave {
  content: string;
  type: string;
  printPaperSizeId: string;
  companyId: string;
}

export class PrintTemplateConfigChangePaperSize {
  type: string;
  printPaperSizeId: string;
}

export class GenerateReq {
  content: string;
  type: string;
  printPaperSizeId: string;
  isDefault: boolean;
}

export class PrintTestReq {
  type: string;
  content: string;
  printPaperSizeId: string;
}


@Pipe({ name: "safeHtml" })
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) { }

  transform(value) {
    value = `<link rel="stylesheet" type="text/css" href="/css/print.css" /> \n ` + value;
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
}

@Injectable()
export class PrintTemplateConfigService {
  apiUrl = 'api/PrintTemplateConfigs';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getDisplay(type: string): Observable<PrintTemplateConfigDisplay> {
    return this.http.get<PrintTemplateConfigDisplay>(this.baseApi + this.apiUrl + '/GetDisplay?type=' + type);
  }

  createOrUpdate(val: PrintTemplateConfigSave) {
    return this.http.put(this.baseApi + this.apiUrl + '/CreateOrUpdate', val);
  }

  generate(val: GenerateReq) {
    return this.http.post(this.baseApi + this.apiUrl + '/Generate', val, { responseType: 'text' });
  }

  printTest(val: PrintTestReq) {
    return this.http.post(this.baseApi + this.apiUrl + '/PrintTest', val, { responseType: 'text' });
  }

  changePaperSize(val) {
    return this.http.post(this.baseApi + this.apiUrl + '/ChangePaperSize', val);
  }

}
