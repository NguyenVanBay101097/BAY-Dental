import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ODataService } from './odata.service';

export class UserSimple {
  Id: string;
  Name: string;
}
@Injectable({
  providedIn: 'root'
})
export class UserOdataService extends ODataService {

  constructor(http: HttpClient, @Inject('BASE_API') baseUrl: string) {super(http, baseUrl, 'ApplicationUsers'); }
}
