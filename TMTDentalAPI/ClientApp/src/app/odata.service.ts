import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { toODataString } from '@progress/kendo-data-query';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

export class OdataViewModel {
  value: any;
}

export abstract class ODataService extends BehaviorSubject<GridDataResult | null> {
  protected BASE_URL: string = 'odata/';
  private headers: Headers;

  protected _state: any = null;
  protected _expand: string;
  protected _params: string;

  public loading: boolean;

  constructor(
    public http: HttpClient,
    @Inject('BASE_API') public baseUrl: string,
    public tableName: string
  ) {
    super(null);
    this.headers = new Headers();
    this.headers.append('Content-Type', 'application/json');
    this.headers.append('Access-Control-Allow-Origin', '*');
    this.BASE_URL = baseUrl + 'odata/';
  }

  public query(state: any): void {
    if (state != null) {
      this._state = state;
    }

    this.fetch(this.tableName, state)
      .subscribe(x => super.next(x));
  }

  public query2(state: any) {
    if (state != null) {
      this._state = state;
    }

    return this.fetch2(this.tableName, state);
  }

  public fetch2(tableName: string, state: any | null): Observable<OdataViewModel> {
    if (state != null) {
      this._state = state;
    } else {
      state = this._state;
    }

    if (state == null) {
      state = {};
    }

    var queryStr = `${toODataString(state)}&$count=true`;
    if (this._expand) {
      queryStr += `&$expand=${this._expand}`;
    }

    if (this._params) {
      queryStr += `&${this._params}`;
    }

    return this.http.get<OdataViewModel>(`${this.BASE_URL}${tableName}?${queryStr}`)
  }

  public fetch(tableName: string, state: any | null): Observable<GridDataResult> {
    if (state != null) {
      this._state = state;
    } else {
      state = this._state;
    }

    if (state == null) {
      state = {};
    }

    var queryStr = `${toODataString(state)}&$count=true`;
    if (this._expand) {
      queryStr += `&$expand=${this._expand}`;
    }

    if (this._params) {
      queryStr += `&${this._params}`;
    }

    this.loading = true;

    return this.http
      .get(`${this.BASE_URL}${tableName}?${queryStr}`)
      .pipe(map(response => response))
      .pipe(
        map(response => (<GridDataResult>{
          data: response['value'],
          total: parseInt(response["@odata.count"], 10)
        })),
        tap(() => this.loading = false)
      );
  }

  public insert(value: any): Promise<any> {
    return this.http.post(`${this.BASE_URL}${this.tableName}`, value)
      .pipe(map((res) => this.resolveDocs(res)))
      .toPromise();
  }

  public update(id: any, value: any): Promise<any> {
    return this.http.put(`${this.BASE_URL}${this.tableName}(${id})`, value)
      .pipe(map((res) => this.resolveDocs(res)))
      .toPromise();
  }

  public getData(id: any, expand: any | null): Observable<any> {
    if (expand != null) {
      return this.http.get(`${this.BASE_URL}${this.tableName}(${id})?${expand}`)
        .pipe(map(response => response));
    } else {
      return this.http.get(`${this.BASE_URL}${this.tableName}(${id})`)
        .pipe(map(response => response));
    }
  }

  public delete(id: any): Promise<any> {
    return this.http.delete(`${this.BASE_URL}${this.tableName}(${id})`)
      .pipe(map((res) => this.resolveDocs(res)))
      .toPromise();
  }

  async resolveDocs(response: any) {
    return response;
  }

  public getState(): any {
    return this._state || { take: 10, skip: 0 };
  }

  public setState(state: any): void {
    this._state = state;
  }

  public setParams(params: string): void {
    this._params = params;
  }

  public setExpand(expand: string): void {
    this._expand = expand;
  }
}
