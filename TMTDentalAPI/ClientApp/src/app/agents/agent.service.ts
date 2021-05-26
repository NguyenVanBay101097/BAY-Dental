import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export class AgentPaged {
  limit: number;
  offset: number;
  search: string;
}


export class AgentBasic {
  id: string;
  name: string;
  phone: string;
}

export class AgentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: AgentBasic[];
}

export class AgentDisplay {
  id : string;
  name : string;
  gender : string
  birthYear: number;
  birthMonth: number;
  birthDay: number;
  jobTitle:string;
  Phone: string;
  email: string;
  address :string;
}

export class AgentSave {
  name : string;
  gender : string
  birthYear: number;
  birthMonth: number;
  birthDay: number;
  jobTitle:string;
  Phone: string;
  email: string;
  address :string;
}


@Injectable({
  providedIn: 'root'
})

export class AgentService {
  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/Agents";

  getPaged(val): Observable<AgentPagging> {
    return this.http.get<AgentPagging>(this.base_api + this.apiUrl, { params: new HttpParams({fromObject: val}) });
  }

  get(id): Observable<AgentDisplay> {
    return this.http.get<AgentDisplay>(this.base_api + this.apiUrl + "/" + id);
  }

  create(val): Observable<AgentDisplay> {
    return this.http.post<AgentDisplay>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + "/" + id);
  }
}
