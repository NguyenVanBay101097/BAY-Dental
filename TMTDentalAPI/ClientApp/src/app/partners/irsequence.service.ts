import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { inject } from '@angular/core/testing';
import { Observable } from 'rxjs';

export class OdataIRSequence {
  value: IrSequenceDisplay[]
}

export class IrSequenceDisplay {
  Id: string;
  code: string;
  numberNext: number;
  name: string;
  prefix: string;
  padding: number;
}

@Injectable({
  providedIn: 'root'
})
export class IrsequenceService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "odata/IrSequences"

  get(code): Observable<OdataIRSequence> {
    return this.http.get<OdataIRSequence>(this.base_api + this.apiUrl + `?$filter=Code eq '${code}'`);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

}
