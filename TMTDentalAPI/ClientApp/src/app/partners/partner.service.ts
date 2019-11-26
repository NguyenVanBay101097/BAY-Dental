import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Injectable, Inject } from '@angular/core';
import { PartnerSimple, PartnerBasic, PartnerDisplay, PartnerPaged, PagedResult2, City, Ward, District, PartnerInfoViewModel } from './partner-simple';
import { ApplicationUserSimple, ApplicationUserPaged, ApplicationUserDisplay } from '../appointment/appointment';
import { AccountInvoiceDisplay, AccountInvoiceBasic, AccountInvoicePaged, PaymentInfoContent, AccountInvoicePrint } from '../account-invoices/account-invoice.service';
import { DotKhamDisplay } from '../dot-khams/dot-khams';
import { EmployeeSimple } from '../employees/employee';
import { HistoryPaged, HistorySimple } from '../history/history';
import { AccountInvoiceLinePaged, AccountInvoiceLineDisplay } from '../account-invoices/account-invoice-line-display';
import { AddressCheckApi } from '../price-list/price-list';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';
import { AccountPaymentBasic } from '../account-payments/account-payment.service';
import { LaboOrderBasic } from '../labo-orders/labo-order.service';
import { PurchaseOrderBasic } from '../purchase-orders/purchase-order.service';
import * as FileSaver from 'file-saver';
import * as XLSX from 'xlsx';

export class PartnerFilter {
    search: string;
    customer: boolean;
    supplier: boolean;
    employee: boolean;
}

export class SaleOrderLineBasic {
    name: string;
    state: string;
    orderPartnerId: string;
    orderId: string;
    productId: string;
    diagnostic: string;
    dateCreated: string;
}

export class SaleOrderLinePaged {
    limit: number;
    offset: number;
    state: string;
    orderPartnerId: string;
    orderId: string;
    productId: string;
    search: string;
    dateOrderFrom: string;
    dateOrderTo: string;
}

export class ImportExcelDirect {
    isCustomer: boolean;
    isCreateNew: boolean;
}

@Injectable()
export class PartnerService {
    apiUrl = 'api/Partners';
    apiAccountInvoiceUrl = 'api/accountinvoices';
    constructor(private http: HttpClient, @Inject('BASE_API') private baseApi: string) { }


    autocomplete(filter: string, customer: boolean)
        : Observable<PartnerSimple[]> {
        let params = new HttpParams()
            .set('filter', filter)
            .set('customer', customer.toString());
        return this.http.get<PartnerSimple[]>(this.baseApi + this.apiUrl + "/autocomplete", { params });
    }

    autocomplete2(val: PartnerFilter)
        : Observable<PartnerSimple[]> {
        return this.http.post<PartnerSimple[]>(this.baseApi + this.apiUrl + "/autocomplete2", val);
    }

    autocompletePartner(val: PartnerPaged): Observable<PartnerSimple[]> {
        return this.http.post<PartnerSimple[]>(this.baseApi + this.apiUrl + "/autocomplete2", val);
    }

    autocompleteUser(val: ApplicationUserPaged): Observable<ApplicationUserSimple[]> {
        return this.http.post<ApplicationUserSimple[]>(this.baseApi + "api/ApplicationUsers/AutocompleteUser", val);
    }

    getAutocompleteSimple(val: PartnerPaged)
        : Observable<PartnerSimple[]> {
        return this.http.post<PartnerSimple[]>(this.baseApi + this.apiUrl + "/autocompletesimple", val);
    }


    createUpdateCustomer(cust: PartnerDisplay, id: string) {
        if (id == null) {
            return this.http.post(this.baseApi + this.apiUrl + "/", cust);
        } else {
            return this.http.put(this.baseApi + this.apiUrl + "/" + id, cust);
        }
    }

    create(val) {
        return this.http.post(this.baseApi + this.apiUrl, val);
    }

    update(id: string, val) {
        return this.http.put(this.baseApi + this.apiUrl + "/" + id, val);
    }

    delete(id: string) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    deleteCustomer(id) {
        return this.http.delete(this.baseApi + this.apiUrl + "/" + id);
    }

    getPartner(id): Observable<PartnerDisplay> {
        return this.http.get<PartnerDisplay>(this.baseApi + this.apiUrl + "/" + id);
    }

    getUser(id): Observable<ApplicationUserDisplay> {
        return this.http.get<ApplicationUserDisplay>(this.baseApi + "api/ApplicationUsers/" + id);
    }

    getPartnerPaged(partnerPaged: PartnerPaged): Observable<PagedResult2<PartnerBasic>> {
        var params = new HttpParams()
            .set('offset', partnerPaged.offset.toString())
            .set('limit', partnerPaged.limit.toString())
            .set('customer', partnerPaged.customer.toString())
            .set('supplier', partnerPaged.supplier.toString());
        if (partnerPaged.searchNamePhoneRef) {
            params = params.set('searchNamePhoneRef', partnerPaged.searchNamePhoneRef);
        };
        return this.http.get<PagedResult2<PartnerBasic>>(this.baseApi + this.apiUrl + "?" + params);
    }

    getPartnerCategories() {
        return this.http.get(this.baseApi + "api/PartnerCategories/");
    }
    getProductCategory(uid) {
        return this.http.get(this.baseApi + "api/ProductCategories/" + uid);
    }

    getDefaultRegisterPayment(id: string) {
        return this.http.get(this.baseApi + "api/Partners/" + id + '/GetDefaultRegisterPayment');
    }

    readonly ashipApiUrl = "https://aship.skyit.vn/api/ApiShipping";

    getProvinceAship(request): Observable<City[]> {
        return this.http.post<City[]>(this.ashipApiUrl + "City/GetCities", request);
    }

    getDistrictAship(request): Observable<District[]> {
        return this.http.post<District[]>(this.ashipApiUrl + "District/GetDistricts", request);
    }

    getWardAship(request): Observable<Ward[]> {
        return this.http.post<Ward[]>(this.ashipApiUrl + "Ward/GetWards", request);
    }

    uploadImage(id, img: File) {
        var formData: FormData = new FormData();
        formData.append('file', img);
        // e.headers = e.headers.append("Access-Control-Allow-Credentials", "false");
        return this.http.post(this.baseApi + this.apiUrl + "/UploadImage/" + id, formData);
    }

    getPaged(params?: HttpParams): Observable<PagedResult2<PartnerBasic>> {
        return this.http.get<PagedResult2<PartnerBasic>>(this.baseApi + this.apiUrl + "", { params: params });
    }

    getInfo(id: string): Observable<PartnerInfoViewModel> {
        return this.http.get<PartnerInfoViewModel>(this.baseApi + this.apiUrl + "/" + id + '/GetInfo');
    }

    getCustomerInvoices(val: AccountInvoicePaged): Observable<PagedResult2<AccountInvoiceBasic>> {
        var param = new HttpParams()
            .set('partnerId', val.partnerId);
        if (val.searchNumber) {
            param = param.set('searchNumber', val.searchNumber.toString());
        }
        if (val.limit) {
            param = param.set('limit', val.limit.toString());
        }
        if (val.offset) {
            param = param.set('offset', val.offset.toString());
        }
        return this.http.get<PagedResult2<AccountInvoiceBasic>>(this.baseApi + this.apiAccountInvoiceUrl + "?" + param);
    }

    getInvoiceDetail(id): Observable<AccountInvoiceDisplay> {
        return this.http.get<AccountInvoiceDisplay>(this.baseApi + this.apiAccountInvoiceUrl + "/" + id);
    }

    getDotKhamList(id: string): Observable<DotKhamDisplay[]> {
        return this.http.get<DotKhamDisplay[]>(this.baseApi + this.apiAccountInvoiceUrl + `/${id}/GetDotKhamList2`);
    }

    getPaymentInfoJson(id: string): Observable<PaymentInfoContent[]> {
        return this.http.get<PaymentInfoContent[]>(this.baseApi + this.apiAccountInvoiceUrl + `/${id}/GetPaymentInfoJson`);
    }

    actionCancel(ids: string[]) {
        return this.http.post(this.baseApi + this.apiAccountInvoiceUrl + "/ActionCancel", ids);
    }

    actionCancelDraft(ids: string[]) {
        return this.http.post(this.baseApi + this.apiAccountInvoiceUrl + "/ActionCancelDraft", ids);
    }

    createInvoice(val: AccountInvoiceDisplay): Observable<AccountInvoiceDisplay> {
        return this.http.post<AccountInvoiceDisplay>(this.baseApi + this.apiAccountInvoiceUrl, val);
    }

    updateInvoice(id: string, val: AccountInvoiceDisplay) {
        return this.http.put(this.baseApi + this.apiAccountInvoiceUrl + "/" + id, val);
    }

    deleteInvoice(id: string) {
        return this.http.delete(this.baseApi + this.apiAccountInvoiceUrl + "/" + id);
    }

    invoiceOpen(ids: string[]) {
        return this.http.post(this.baseApi + this.apiAccountInvoiceUrl + "/invoiceopen", ids);
    }

    invoiceDefaultGet(val: AccountInvoiceDisplay): Observable<AccountInvoiceDisplay> {
        return this.http.post<AccountInvoiceDisplay>(this.baseApi + this.apiAccountInvoiceUrl + "/defaultget", val);
    }

    printInvoice(id: string): Observable<AccountInvoicePrint> {
        return this.http.get<AccountInvoicePrint>(this.baseApi + this.apiAccountInvoiceUrl + "/" + id + "/print");
    }

    getEmployeeSimpleList(val): Observable<EmployeeSimple[]> {
        return this.http.post<EmployeeSimple[]>(this.baseApi + "api/Employees/Autocomplete", val);
    }

    getHistories(): Observable<HistorySimple[]> {
        return this.http.get<HistorySimple[]>(this.baseApi + 'api/histories/GetHistoriesCheckbox');
    }

    getInvoiceLineByPartner(paged): Observable<PagedResult2<AccountInvoiceLineDisplay>> {
        return this.http.get<PagedResult2<AccountInvoiceLineDisplay>>(this.baseApi + 'api/AccountInvoiceLines', { params: paged });
    }

    checkAddressApi(text: string): Observable<AddressCheckApi[]> {
        return this.http.get<AddressCheckApi[]>(this.baseApi + this.apiUrl + '/CheckAddress?text=' + text);
    }

    importFromExcelCreate(file: File, dir) {
        var formData = new FormData();
        formData.set('file', file);
        if (dir.isCreateNew)
            return this.http.post(this.baseApi + this.apiUrl + "/ExcelImportCreate", formData, { params: dir });
        else
            return this.http.post(this.baseApi + this.apiUrl + "/ExcelImportUpdate", formData, { params: dir });
    }

    getSaleOrderByPartner(paged): Observable<PagedResult2<SaleOrderBasic>> {
        return this.http.get<PagedResult2<SaleOrderBasic>>(this.baseApi + 'api/SaleOrders', { params: paged });
    }

    getSaleOrderLineByPartner(paged): Observable<PagedResult2<SaleOrderLineBasic>> {
        return this.http.get<PagedResult2<SaleOrderLineBasic>>(this.baseApi + 'api/SaleOrderLines', { params: paged });
    }

    getPayments(val): Observable<PagedResult2<AccountPaymentBasic>> {
        return this.http.get<PagedResult2<AccountPaymentBasic>>(this.baseApi + 'api/AccountPayments', { params: val });
    }

    getLaboOrderByPartner(paged): Observable<PagedResult2<LaboOrderBasic>> {
        return this.http.get<PagedResult2<LaboOrderBasic>>(this.baseApi + 'api/LaboOrders', { params: paged });
    }

    getPurchaseOrderByPartner(paged): Observable<PagedResult2<PurchaseOrderBasic>> {
        return this.http.get<PagedResult2<PurchaseOrderBasic>>(this.baseApi + 'api/PurchaseOrders', { params: paged });
    }

    exportAsExcelFile(json: any[], excelFileName: string): void {
        const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(json);
        const workbook: XLSX.WorkBook = { Sheets: { 'Đối tác': worksheet }, SheetNames: ['Đối tác'], };
        const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
        var wscols = [
            { wch: 6 },
            { wpx: 50 }
        ];
        worksheet["!cols"] = wscols;

        this.saveAsExcelFile(excelBuffer, excelFileName);
    }

    saveAsExcelFile(buffer: any, fileName: string): void {
        const EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
        const EXCEL_EXTENSION = '.xlsx';
        const data: Blob = new Blob([buffer], { type: EXCEL_TYPE });
        FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
    }

    excelServerExport(paged) {
        return this.http.get(this.baseApi + this.apiUrl + '/ExportExcelFile', { responseType: 'blob', params: paged });
    }

    getPartnerDisplaysByIds(ids: string[]): Observable<PartnerDisplay[]> {
        return this.http.post<PartnerDisplay[]>(this.baseApi + this.apiUrl + '/getPartnerDisplaysByIds', ids);
    }
}


