import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ProductBasic2 } from '../products/product.service';
import { Observable } from 'rxjs';
import { PagedResult2 } from '../employee-categories/emp-category';
import { ProductCategoryPaging, ProductCategoryBasic } from '../product-categories/product-category.service';
import { PartnerCategoryPaged, PartnerCategoryBasic } from '../partner-categories/partner-category.service';
import { ProductSimple } from '../products/product-simple';
import { ProductPriceListBasic, ProductPriceListSave } from './price-list';

@Injectable({
  providedIn: 'root'
})
export class PriceListService {
  apiUrl = 'api/productPriceLists';
  constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

  loadServicesAutocomplete2(params): Observable<ProductSimple[]> {
    return this.http.post<ProductSimple[]>(this.baseApi + 'api/products/autocomplete2', params);
  }

  loadServiceCategories(params): Observable<ProductCategoryBasic[]> {
    return this.http.post<ProductCategoryBasic[]>(this.baseApi + 'api/productcategories/Autocomplete', params);
  }

  partnerCategoryAutocomplete(val: PartnerCategoryPaged): Observable<PartnerCategoryBasic[]> {
    return this.http.post<PartnerCategoryBasic[]>(this.baseApi + 'api/partnercategories/autocomplete', val);
  }

  loadServiceDetail(id): Observable<ProductBasic2> {
    return this.http.get<ProductBasic2>(this.baseApi + 'api/products/' + id);
  }

  loadPriceListList(paged): Observable<PagedResult2<ProductPriceListBasic>> {
    return this.http.get<PagedResult2<ProductPriceListBasic>>(this.baseApi + this.apiUrl, { params: paged });
  }

  getPriceList(id): Observable<ProductPriceListSave> {
    return this.http.get<ProductPriceListSave>(this.baseApi + this.apiUrl + '/' + id);
  }

  createPriceList(val): Observable<ProductPriceListBasic> {
    return this.http.post<ProductPriceListBasic>(this.baseApi + this.apiUrl, val);
  }

  updatePriceList(val, id) {
    return this.http.put(this.baseApi + this.apiUrl + '/' + id, val);
  }

  deletePriceList(id) {
    return this.http.delete(this.baseApi + this.apiUrl + '/' + id);
  }

}
