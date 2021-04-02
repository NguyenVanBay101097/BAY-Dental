import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PagedResult2 } from '../core/paged-result-2';
import { PartnerSimple } from '../partners/partner-simple';
import { LaboOrderLineDisplay } from '../labo-order-lines/labo-order-line.service';
import { DotKhamBasic } from '../dot-khams/dot-khams';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';
import { ToothBasic } from '../teeth/tooth.service';
import { SaleOrderLineBasic } from '../partners/partner.service';
import { ProductSimple } from '../products/product-simple';
import { IrAttachmentBasic } from '../shared/shared';

export class LaboOrderPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    dateOrderFrom: string;
    dateOrderTo: string;
    datePlannedFrom: string;
    datePlannedTo: string;
    state: string;
    saleOrderId: string;
    saleOrderLineId: string;
}

export class LaboOrderBasic {
    id: string;
    partnerName: string;
    customerName: string;
    name: string;
    dateOrder: string;
    state: string;
    amountTotal: string;
    saleOrderId: string;
    saleOrderName: string;
    datePlanned: string;
}

export class LaboOrderDefaultGet {
    dotKhamId: string;
    saleOrderLineId: string;
}

export class LaboOrderDisplay {
    id: string;
    name: string;
    quantity: number;
    priceUnit: number;
    color: string;
    state: string;
    partnerRef: string;
    partnerId: string;
    partner: PartnerSimple;
    saleOrder: SaleOrderBasic;
    dateOrder: string;
    amountTotal: number;
    datePlanned: string;
    dotKhamId: string;
    orderLines: LaboOrderLineDisplay[];
    dotKham: DotKhamBasic;
    teeth: ToothBasic[];
    saleOrderLine: SaleOrderLineBasic;
    warrantyCode: string;
    warrantyPeriod: string;
    product: ProductSimple;
    laboOrderProducts: ProductSimple[];
    images: IrAttachmentBasic[];
}

export class LaboOrderStatisticsPaged {
    limit: number;
    offset: number;
    search: string;
    partnerId: string;
    productId: string;
    dateOrderFrom: string;
    dateOrderTo: string;
    datePlannedFrom: string;
    datePlannedTo: string;
}

export class OrderLaboPaged {
    limit: number;
    offset: number;
    search: string;
    state: string;
}

export class LaboOrderReportInput {
    dateFrom: string;
    dateTo: string;
    companyId: string;
    state: string;
}

export class LaboOrderReportOutput {
    laboReceived: number;
    laboAppointment: number;
}

export class LaboImageBasic {
    name: string;
    url: string;
}

export class ExportLaboPaged {
    limit: number;
    offset: number;
    search: string;
    state: string;
    dateExportFrom: string;
    dateExportTo: string;
    dateExport: string;
}

@Injectable({ providedIn: 'root' })
export class LaboOrderService {
    apiUrl = 'api/LaboOrders';
    apiPrintUrl = 'LaboOrder';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<PagedResult2<LaboOrderBasic>> {
        return this.http.get<PagedResult2<LaboOrderBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    get(id): Observable<LaboOrderDisplay> {
        return this.http.get<LaboOrderDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: LaboOrderDisplay): Observable<LaboOrderDisplay> {
        return this.http.post<LaboOrderDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: LaboOrderDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    defaultGet(val: any): Observable<LaboOrderDisplay> {
        return this.http.post<LaboOrderDisplay>(this.baseApi + this.apiUrl + '/DefaultGet', val);
    }

    // getPrint(id) {
    //     return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetPrint');
    // }

    getPrint(id) {
        return this.http.get(this.baseApi + this.apiPrintUrl  + '/Print' + `?id=${id}`, { responseType: 'text' });
    }

    buttonConfirm(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonConfirm', ids);
    }

    buttonCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/ButtonCancel', ids);
    }

    unlink(ids: string[]) {
        return this.http.post(this.baseApi + this.apiUrl + '/Unlink', ids);
    }

    getStatistics(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Statistics', data);
    }

    GetFromSaleOrder_OrderLine(val: any): Observable<PagedResult2<LaboOrderBasic>> {
        return this.http.get<PagedResult2<LaboOrderBasic>>(this.baseApi + this.apiUrl + '/GetFromSaleOrder_OrderLine', { params: new HttpParams({ fromObject: val }) });
    }

    getCountLaboOrder(data: any) {
        return this.http.post<LaboOrderReportOutput>(this.baseApi + this.apiUrl + '/LaboOrderGetCount', data);
    }

    getLaboForSaleOrderLine(val: any): Observable<PagedResult2<any>> {
        return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl + '/GetLaboForSaleOrderLine', { params: new HttpParams({ fromObject: val }) });
    }

    getOrderLabo(val: any): Observable<PagedResult2<any>> {
        return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl + '/GetOrderLabo', { params: new HttpParams({ fromObject: val }) });
    }

    updateReceiptLabo(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UpdateOrderLabo", val);
    }

    getExportLabo(val: any): Observable<PagedResult2<any>> {
        return this.http.get<PagedResult2<any>>(this.baseApi + this.apiUrl + '/GetExportLabo', { params: new HttpParams({ fromObject: val }) });
    }

    updateExportLabo(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/UpdateExportLabo", val);
    }

    actionCancelReceipt(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/ActionCancelReceipt", val);
    }

    checkExistWarrantyCode(val) {
        return this.http.post(this.baseApi + this.apiUrl + "/CheckExistWarrantyCode", val);
    }
}