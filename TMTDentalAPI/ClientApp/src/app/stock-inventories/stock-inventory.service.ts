import { ProductCategoryBasic } from './../product-categories/product-category.service';
import { ProductSimple } from './../products/product-simple';
import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UoMBasic } from '../uoms/uom.service';

export class PrecscriptionPaymentPaged {
  limit: number;
  offset: number;
  search: string;
  dateFrom: string;
  dateTo: string;
  state: string;
}


export class StockInventoryBasic {
  id: string;
  name: string;
  date: Date;
  state: string;
}

export class PrescriptionPaymentPagging {
  limit: number;
  offset: number;
  totalItems: number;
  items: StockInventoryBasic[];
}

export class StockInventorySave {
  date: Date;
  note: string;
  state: string;
  locationId: string;
  productId: string;
  categoryId: string;
  filter: string;
  companyId: string;
  exhausted: boolean;
}

export class StockInventoryDisplay {
  id: string;
  date: Date;
  note: string;
  state: string;
  locationId: string;
  location: any;
  productId: string;
  product: ProductSimple;
  categoryId: string;
  category: ProductCategoryBasic;
  filter: string;
  companyId: string;
  exhausted: boolean;
  lines: StockInventoryLineDisplay[];
  moves: any[];
}

export class StockInventoryLineBasic {
  id: string;
  locationId: string;
  location: any;
  productId: string;
  product: ProductSimple;
  productUOMId: string;
  productUOM: UoMBasic;
  productQty: number;
  theoreticalQty: number;
}

export class StockInventoryLineSave {
  id: string;
  locationId: string;
  productId: string;
  productUOMId: string;
  productQty: number;
  theoreticalQty: number;
}

export class StockInventoryLineDisplay {
  id: string;
  locationId: string;
  location: any;
  productId: string;
  product: ProductSimple;
  productUOMId: string;
  productUOM: UoMBasic;
  productQty: number;
  theoreticalQty: number;
}

export class StockInventoryLineByProductId
{
    productId: string;
    inventoryId: string;
}

export class StockInventoryDefaultGet {
  companyId: string;
}


@Injectable({
  providedIn: 'root'
})
export class StockInventoryService {

  constructor(private http: HttpClient, @Inject("BASE_API") private base_api: string) { }
  apiUrl = "api/StockInventories";

  getPaged(val): Observable<PrescriptionPaymentPagging> {
    return this.http.get<PrescriptionPaymentPagging>(this.base_api + this.apiUrl, { params: val });
  }

  get(id): Observable<StockInventoryDisplay> {
    return this.http.get<StockInventoryDisplay>(this.base_api + this.apiUrl + "/" + id);
  }

  getDefault(){
    return this.http.get(this.base_api + this.apiUrl + '/DefaultGet');
  }

  create(val): Observable<StockInventoryDisplay> {
    return this.http.post<StockInventoryDisplay>(this.base_api + this.apiUrl, val);
  }

  update(id, val) {
    return this.http.put(this.base_api + this.apiUrl + '/' + id, val);
  }

  // getlistProductInventory(id) {
  //   return this.http.get(this.base_api + this.apiUrl  + '/' + id + '/GetlistProductInventory');
  // }

  inventorylineByProductId(val) {
    return this.http.post(this.base_api + this.apiUrl + '/GetInventoryLineByProductId', val)
  }

  prepareInventory(ids) {
    return this.http.post(this.base_api + this.apiUrl + '/PrepareInventory', ids)
  }

  actionDone(ids) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionDone', ids)
  }

  actionCancel(ids) {
    return this.http.post(this.base_api + this.apiUrl + '/ActionCancel', ids)
  }

  delete(id: string) {
    return this.http.delete(this.base_api + this.apiUrl + "/" + id);
  }
}
