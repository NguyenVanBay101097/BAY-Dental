import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class PhieuThuChiService {
  apiUrl = 'api/PhieuThuChi';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  
}
