import { HttpClient } from '@angular/common/http';
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

export class PrintTemplateConfigChangeType {
  type: string;
  printPaperSizeId: string;
  isDefault: boolean;
}

export class GenerateReq {
  content: string;
  type: string;
  printPaperSizeId: string;
  isDefault: boolean;
}

export class PrintTestReq {
  type: string;
}


@Pipe({ name: "safeHtml" })
export class SafeHtmlPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) { }

  transform(value) {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
}

@Injectable()
export class PrintTemplateConfigService {
  apiUrl = 'api/PrintTemplateConfigs';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getDisplay(val: PrintTemplateConfigChangeType): Observable<PrintTemplateConfigDisplay> {
    return this.http.post<PrintTemplateConfigDisplay>(this.baseApi + this.apiUrl + '/GetDisplay', val);
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

}
