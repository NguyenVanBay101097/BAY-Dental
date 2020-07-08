import { Injectable, Inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

export class PartnerSourceBasic {
  id: string;
  name: string;
  type: string;
}
export class PartnerSourceSave {
  name: string;
  type: string;
}

export class PartnerSourcePaged {
  limit: number;
  offset: number;
  search: string;
}

export class PartnerSourcePaging {
  offset: number;
  limit: number;
  totalItems: number;
  items: PartnerSourceBasic[];
}



@Injectable()
export class PartnerSourceService {
  apiUrl = "api/PartnerSources";
  constructor(
    private http: HttpClient,
    @Inject("BASE_API") private baseApi: string
  ) {}

  getPaged(val: any): Observable<PartnerSourcePaging> {
    return this.http.get<PartnerSourcePaging>(this.baseApi + this.apiUrl, {
      params: val,
    });
  }

  get(id: string) {
    return this.http.get<PartnerSourceSave>(
      this.baseApi + this.apiUrl + "/" + id
    );
  }

  create(val: PartnerSourceSave) {
    return this.http.post(this.baseApi + this.apiUrl, val);
  }

  update(id: string, val: PartnerSourceSave) {
    return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
  }

  autocomplete(val: PartnerSourcePaged): Observable<PartnerSourceBasic[]> {
    return this.http.post<PartnerSourceBasic[]>(this.baseApi + this.apiUrl + "/Autocomplete", val);
  }

  delete(id: string) {
    return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
  }

 
}
