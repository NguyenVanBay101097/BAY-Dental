import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { PrintPaperSizeDisplay } from '../config-prints/print-paper-size.service';

export class PrintTemplateDefault {
  type: string;
}

@Injectable()
export class PrintTemplateService {

  apiUrl = 'api/PrintTemplates';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  getDisplay(val: PrintTemplateDefault): Observable<string> {
    return this.http.post<string>(this.baseApi + this.apiUrl + '/GetPrintTemplateDefault', val);
  }


}
