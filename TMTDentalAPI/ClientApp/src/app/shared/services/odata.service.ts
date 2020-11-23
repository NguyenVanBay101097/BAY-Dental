import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { GridDataResult } from "@progress/kendo-angular-grid";
import { toODataString } from "@progress/kendo-data-query";
import { BehaviorSubject, Observable } from "rxjs";
import { finalize, map } from "rxjs/operators";

export abstract class ODataService extends BehaviorSubject<GridDataResult | null> {
  protected BASE_URL: string = "/odata/";
  public loading: boolean;
  constructor(
    protected http: HttpClient,
    @Inject("BASE_API") baseUrl: string,
    protected tableName: string
  ) {
    super(null);
    this.BASE_URL = baseUrl + "odata/";
  }

  header() {
    let headers = new HttpHeaders();
    headers.append("Content-Type", "application/json;odata.metadata=minimal");
    headers.append("Accept", "application/json");
    return headers;
  }

  public query(state: any, options?: any): void {
    this.fetch(this.tableName, state, options || {}).subscribe((x: any) => {
      super.next(x);
    });
  }

  public fetch(
    tableName: string,
    state: any | null,
    options?: any
  ): Observable<GridDataResult> {
    options = options || {};
    var queryStr = `${toODataString(state)}&$count=true`;
    if (options.params) {
      queryStr =
        queryStr +
        "&" +
        new HttpParams({ fromObject: options.params }).toString();
    }

    if (options.expand) {
      queryStr = queryStr + "&$expand=" + options.expand;
    }
    if (options.select) {
      queryStr = queryStr + "&$select=" + options.select;
    }
    if (options.orderby) {
      queryStr = queryStr + "&$orderby=" + options.orderby;
    }

    if (queryStr) {
      // encode everything which looks like a GUID
      var guid = /('[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}')/gi;
      queryStr = queryStr.replace(guid, function (x) {
        return x.substring(1, x.length - 1);
      });
    }

    this.loading = true;

    return this.http.get(`${this.BASE_URL}${tableName}?${queryStr}`).pipe(
      map(
        (response: any) =>
          <GridDataResult>{
            data: response.value,
            total: parseInt(response["@odata.count"], 10),
          }
      ),
      finalize(() => (this.loading = false))
    );
  }

  public fetch2(state: any | null, options?: any): Observable<GridDataResult> {
    options = options || {};
    var queryStr = `${toODataString(state)}&$count=true`;
    if (options.params) {
      queryStr =
        queryStr +
        "&" +
        new HttpParams({ fromObject: options.params }).toString();
    }

    if (options.expand) {
      queryStr = queryStr + "&$expand=" + options.expand;
    }

    if (queryStr) {
      // encode everything which looks like a GUID
      var guid = /('[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}')/gi;
      queryStr = queryStr.replace(guid, function (x) {
        return x.substring(1, x.length - 1);
      });
    }

    this.loading = true;

    return this.http.get(`${this.BASE_URL}${this.tableName}?${queryStr}`).pipe(
      map(
        (response: any) =>
          <GridDataResult>{
            data: response.value,
            total: parseInt(response["@odata.count"], 10),
          }
      ),
      finalize(() => (this.loading = false))
    );
  }



  public get(id: any, obj: any | null): Observable<any> {
    return this.http.get(`${this.BASE_URL}${this.tableName}(${id})`, { params: new HttpParams({ fromObject: obj }) });
  }

  public create(value: any) {
    return this.http.post(`${this.BASE_URL}${this.tableName}`, value);
  }

  public update(id: any, value: any) {
    return this.http.put(`${this.BASE_URL}${this.tableName}(${id})`, value);
  }

  public patch(id: any, value: any) {
    return this.http.patch(`${this.BASE_URL}${this.tableName}(${id})`, value);
  }

  public delete(id: any) {
    return this.http.delete(`${this.BASE_URL}${this.tableName}(${id})`);
  }

  public getFunction(id: string, func: string, options?: any) {
    if (options) {
      return this.http.get(
        `${this.BASE_URL}${this.tableName}(${id})/${func}`,
        options
      );
    } else {
      return this.http.get(`${this.BASE_URL}${this.tableName}(${id})/${func}`);
    }
  }

  public getCollectionFunction(func: string, options?: any) {
    if (options) {
      return this.http.get(
        `${this.BASE_URL}${this.tableName}/${func}`,
        options
      );
    } else {
      return this.http.get(`${this.BASE_URL}${this.tableName}/${func}`);
    }
  }
}
