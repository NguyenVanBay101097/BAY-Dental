import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { inject } from '@angular/core/testing';
import { Observable } from 'rxjs';
import { ODataService } from '../odata.service';



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
export class IrsequenceService extends ODataService {

  constructor(private httpClient: HttpClient, @Inject("BASE_API") private base_api: string) {
    super(httpClient, base_api, "IRSequences")
  }

  getByCode(code?: string, state?: any) {
    return this.query2(Object.assign({}, state, {
      filter: {
        filters: [
          {
            field: "Code", operator: "eq", value: code
          }
        ],
        logic: "and"
      }
    }));
  }
  updateAny(id, value) {
    return this.update(id, value);
  }

  // getById(id: any, state?: any) {
  //   return this.query2(Object.assign({}, state, {
  //     filter: {
  //       filters: [
  //         {
  //           field: "Id", operator: "eq", value: id
  //         }
  //       ],
  //       logic: "and"
  //     }
  //   }));
  // }

  // get(code): Observable<OdataIRSequence> {
  //   return this.http.get<OdataIRSequence>(this.base_api + this.apiUrl + `?$filter=Code eq '${code}'`);
  // }

  // update(id, val) {
  //   return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  // }

}
