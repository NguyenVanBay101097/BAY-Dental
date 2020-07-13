import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AccountAccountService {
  apiUrl = 'api/AccountAccounts';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }
}
