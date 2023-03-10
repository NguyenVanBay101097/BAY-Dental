import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountInvoiceCbx } from '../account-invoices/account-invoice.service';
import { LaboOrderLineBasic } from '../labo-order-lines/labo-order-line.service';
import { LaboOrderBasic } from '../labo-orders/labo-order.service';
import { IrAttachmentBasic, IrAttachmentSearchRead } from '../shared/shared';
import { ToaThuocBasic } from '../toa-thuocs/toa-thuoc.service';
import { DotKhamDefaultGet, DotKhamDisplay, DotkhamEntitySearchBy, DotKhamPaging, DotKhamStepDisplay } from './dot-khams';



@Injectable({ providedIn: 'root' })
export class DotKhamService {
    apiUrl = 'api/dotkhams';
    readonly currentModel = 'dotkham';
    readonly irApiUrl = 'api/IrAttachments';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }

    getPaged(val: any): Observable<DotKhamPaging> {
        return this.http.get<DotKhamPaging>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
    }

    defaultGet(val: DotKhamDefaultGet): Observable<DotKhamDisplay> {
        return this.http.post<DotKhamDisplay>(this.baseApi + this.apiUrl + "/defaultget", val);
    }

    get(id): Observable<DotKhamDisplay> {
        return this.http.get<DotKhamDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    create(val: DotKhamDisplay): Observable<DotKhamDisplay> {
        return this.http.post<DotKhamDisplay>(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val: DotKhamDisplay) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    patch(id: string, dkpatch) {
        return this.http.patch(this.baseApi + this.apiUrl + "/" + id, dkpatch);
    }

    actionConfirm(id: string) {
        return this.http.post(this.baseApi + this.apiUrl + `/${id}/ActionConfirm`, null);
    }

    getToaThuocs(id: string): Observable<ToaThuocBasic[]> {
        return this.http.post<ToaThuocBasic[]>(this.baseApi + this.apiUrl + `/${id}/GetToaThuocs`, null);
    }

    // getDotKhamLines(id: string): Observable<DotKhamLineBasic[]> {
    //     return this.http.post<DotKhamLineBasic[]>(this.baseApi + this.apiUrl + `/${id}/GetDotKhamLines`, null);
    // }

    // getDotKhamLines2(id: string): Observable<DotKhamLineBasic[][]> {
    //     return this.http.post<DotKhamLineBasic[][]>(this.baseApi + this.apiUrl + `/${id}/GetDotKhamLines2`, null);
    // }

    getLaboOrderLines(id: string): Observable<LaboOrderLineBasic[]> {
        return this.http.post<LaboOrderLineBasic[]>(this.baseApi + this.apiUrl + `/${id}/GetLaboOrderLines`, null);
    }

    getLaboOrders(id: string): Observable<LaboOrderBasic[]> {
        return this.http.get<LaboOrderBasic[]>(this.baseApi + this.apiUrl + `/${id}/GetLaboOrders`);
    }

    getCustomerInvoices(id: string): Observable<AccountInvoiceCbx[]> {
        return this.http.get<AccountInvoiceCbx[]>(this.baseApi + this.apiUrl + '/GetCustomerInvoices/' + id);
    }

    //C??c c??ng ??o???n theo ?????t kh??m ID
    getDotKhamStepsByDKId(id: string): Observable<DotKhamStepDisplay[]> {
        return this.http.get<DotKhamStepDisplay[]>(this.baseApi + this.apiUrl + `/${id}/VisibleSteps`);
    }

    getDotKhamStepsByDKId2(id: string, filter: string): Observable<DotKhamStepDisplay[][]> {
        return this.http.get<DotKhamStepDisplay[][]>(this.baseApi + this.apiUrl + `/${id}/VisibleSteps2?show=` + filter);
    }

    //T???t c??? c??ng ??o???n
    getDotKhamStepsByInvoice(id: string): Observable<DotKhamStepDisplay[]> {
        return this.http.get<DotKhamStepDisplay[]>(this.baseApi + this.apiUrl + `/${id}/VisibleSteps`);
    }

    patchDotKhamStep(id, stepSavePatch) {
        return this.http.patch(this.baseApi + 'api/DotKhamSteps/' + id, stepSavePatch);
    }

    createDKSteps(steps) {
        return this.http.post(this.baseApi + 'api/DotKhamSteps', steps);
    }

    deleteDKSteps(id) {
        return this.http.delete(this.baseApi + 'api/DotKhamSteps/' + id);
    }

    //L???y ds d???ch v??? ???? ????ng k?? 
    getInvoiceLineByInvoiceId(id): Observable<any[]> {
        return this.http.get<any[]>(this.baseApi + 'api/accountInvoiceLines/GetDotKhamInvoiceLine/' + id);
    }

    //S???p x???p l???i th??? t??? c??c dotKhamStep
    reorder(index, list) {
        return this.http.put(this.baseApi + 'api/DotKhamSteps/Reorder/' + index, list);
    }

    uploadFiles(id, formData: FormData): Observable<IrAttachmentBasic[]> {
        // e.headers = e.headers.append("Access-Control-Allow-Credentials", "false");
        return this.http.post<IrAttachmentBasic[]>(this.baseApi + this.apiUrl + `/${id}/BinaryUploadAttachment`, formData);
    }

    getImageIds(irSR: IrAttachmentSearchRead): Observable<IrAttachmentBasic[]> {
        return this.http.post<IrAttachmentBasic[]>(this.baseApi + this.irApiUrl + '/SearchRead', irSR);
    }

    deleteAttachment(id) {
        return this.http.delete(this.baseApi + this.irApiUrl + '/' + id);
    }

    getSearchedDotKham(search: DotkhamEntitySearchBy): Observable<DotKhamDisplay> {
        return this.http.post<DotKhamDisplay>(this.baseApi + this.apiUrl + '/GetSearchedDotKham', search);
    }

    getInfo(id) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetInfo');
    }
}