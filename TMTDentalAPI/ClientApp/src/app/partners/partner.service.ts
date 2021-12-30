import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountInvoiceLineDisplay } from '../account-invoices/account-invoice-line-display';
import { AccountInvoiceBasic, AccountInvoiceDisplay, AccountInvoicePaged, AccountInvoicePrint, PaymentInfoContent } from '../account-invoices/account-invoice.service';
import { AccountPaymentBasic } from '../account-payments/account-payment.service';
import { ApplicationUserDisplay, ApplicationUserPaged, ApplicationUserSimple, AppointmentDisplay } from '../appointment/appointment';
import { DotKhamDisplay } from '../dot-khams/dot-khams';
import { EmployeeSimple } from '../employees/employee';
import { HistorySimple } from '../history/history';
import { LaboOrderBasic } from '../labo-orders/labo-order.service';
import { AddressCheckApi } from '../price-list/price-list';
import { PurchaseOrderBasic } from '../purchase-orders/purchase-order.service';
import { SaleOrderBasic } from '../sale-orders/sale-order-basic';
import { IrAttachmentBasic } from '../shared/shared';
import { ToothDisplay } from '../teeth/tooth.service';
import { City, District, PagedResult2, PartnerBasic, PartnerDisplay, PartnerInfoViewModel, PartnerPaged, PartnerPrint, PartnerSimple, PartnerSimpleContact, PartnerSimpleInfo, Ward } from './partner-simple';

export class PartnerFilter {
    search: string;
    customer: boolean;
    supplier: boolean;
    employee: boolean;
    limit: number;
    offset: number;
    active: boolean;
}

export class SaleOrderLineBasic {
    name: string;
    state: string;
    orderPartnerId: string;
    orderId: string;
    productId: string;
    diagnostic: string;
    dateCreated: string;
    teeth: ToothDisplay[];
    product: any;
    toothType: string;
    toothCategoryId: string;
    order: any;
    employee: any;
}

export class SaleOrderLinePaged {
    limit: number;
    offset: number;
    state: string;
    orderPartnerId: string;
    orderId: string;
    productId: string;
    search: string;
    dateOrderFrom: any;
    dateOrderTo: any;
    employeeId: string;
    companyId: string;
    dateFrom: string;
    dateTo: string;
}

export class PartnerReportLocationCitySearch {
    cityCode: string;
    districtCode: string;
    wardCode: string;
}

export class PartnerReportLocationCity {
    cityCode: string;
    cityName: string;
    total: number;
    percentage: number;
}

export class PartnerReportLocationDistrict {
    districtCode: string;
    districtName: string;
    total: number;
    percentage: number;
}

export class PartnerReportLocationWard {
    wardCode: string;
    wardName: string;
    total: number;
    percentage: number;
}

export class ImportExcelDirect {
    isCustomer: boolean;
    isCreateNew: boolean;
}

export class PartnerAddRemoveTags {
    id: string;
    tagIds: string[];
}

export class PartnerImageBasic {
    id: string;
    name: string;
    date: string;
    note: string;
    uploadId: string;
    url: string;
}



export class GenderPartner {
    id: string;
    name: string;
}

export class PartnerImageSave {
    name: string;
    date: string;
    note: string;
    formData: FormData;
}

export class PartnerImageViewModel {
    date: string;
    partnerImages: PartnerImageBasic[];
}

export class PartnerCustomerReportInput {
    dateFrom: string;
    companyId: string;
    dateTo: string;
}

export class PartnerCustomerReportOutput {
    constructor() {
        this.totalOldPartner = 0;
        this.totalNewPartner = 0;
    }
    totalOldPartner: number;
    totalNewPartner: number;
}

export class CustomerStatisticsInput {
    dateFrom: string;
    dateTo: string;
}

export class CustomerStatisticsDetails {
    location: string;
    customerTotal: number;
    customerOld: number;
    customerNew: number;
}

export class CustomerStatisticsOutput {
    customerTotal: number;
    customerOld: number;
    customerNew: number;
    details: CustomerStatisticsDetails;
}

export class PartnerActivePatch {
    active: boolean;
}

export class PartnerGetDebtPagedFilter {
    limit: number;
    offset: number;
    search: string;
    companyId: string;
}

export class PartnerInfoDisplay {
    id: string;
    dateCreated: string;
    ref: string;
    avatar?: any;
    displayName: string;
    name: string;
    phone: string;
    email: string;
    birthYear: number;
    birthMonth: number;
    birthDay: number;
    orderState: string;
    orderResidual?: any;
    totalDebit?: any;
    memberLevelId?: any;
    memberLevel?: any;
    categories: any[];
    dateOfBirth: string;
    age: string;
    companyName: string;
    appointmentDate: string;
    saleOrderDate: string;
}

export class PartnerInfoPaged {
    limit: number;
    offset: number;
    search: string;
    categIds: string[];
    hasOrderResidual?: number;
    hasTotalDebit?: number;
    memberLevelId: string;
    cardTypeId: string;
    orderState: string;
    companyId: string;
    active?: boolean;
}

export class PartnerGetExistReq{
    phone: string;
}

@Injectable({ providedIn: 'root' })
export class PartnerService {
    apiUrl = 'api/Partners';
    apiAccountInvoiceUrl = 'api/accountinvoices';
    apiPartnerImage = "api/PartnerImages";
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

    autocomplete3(val: PartnerPaged)
        : Observable<PartnerSimpleContact[]> {
        return this.http.post<PartnerSimpleContact[]>(this.baseApi + this.apiUrl + "/Autocomplete3", val);
    }

    autocompletePartner(val: PartnerPaged): Observable<PartnerSimple[]> {
        return this.http.post<PartnerSimple[]>(this.baseApi + this.apiUrl + "/autocomplete2", val);
    }

    autocompletePartnerInfo(val: PartnerPaged): Observable<PartnerSimpleInfo[]> {
        return this.http.post<PartnerSimpleInfo[]>(this.baseApi + this.apiUrl + "/AutocompleteInfos", val);
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

    getValidServiceCards(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + "/" + id + '/GetValidServiceCards');
    }

    reportLocationCity(val: PartnerReportLocationCitySearch): Observable<PartnerReportLocationCity[]> {
        return this.http.post<PartnerReportLocationCity[]>(this.baseApi + this.apiUrl + '/ReportLocationCity', val);
    }

    reportLocationDistrict(val: PartnerReportLocationCity): Observable<PartnerReportLocationDistrict[]> {
        return this.http.post<PartnerReportLocationDistrict[]>(this.baseApi + this.apiUrl + '/ReportLocationDistrict', val);
    }

    reportLocationWard(val: PartnerReportLocationDistrict): Observable<PartnerReportLocationWard[]> {
        return this.http.post<PartnerReportLocationWard[]>(this.baseApi + this.apiUrl + '/ReportLocationWard', val);
    }

    updateCustomersZaloId() {
        return this.http.post(this.baseApi + this.apiUrl + '/UpdateCustomersZaloId', {});
    }

    actionImport(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionImport', val);
    }

    actionImportUpdate(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/ActionImportUpdate', val);
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
        if (partnerPaged.search) {
            params = params.set('searchNamePhoneRef', partnerPaged.search);
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

    GetDefaultPayment(val) {
        return this.http.post(this.baseApi + 'api/Partners/GetDefaultPayment', val);
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

    exportUnreconcileInvoices(id: string) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + "/ExportUnreconcileInvoices", { responseType: "blob" });
    }

    uploadImage(id, img: File) {
        var formData: FormData = new FormData();
        formData.append('file', img);
        // e.headers = e.headers.append("Access-Control-Allow-Credentials", "false");
        return this.http.post(this.baseApi + this.apiUrl + "/UploadImage/" + id, formData);
    }

    getPaged(val?: any): Observable<PagedResult2<PartnerBasic>> {
        return this.http.get<PagedResult2<PartnerBasic>>(this.baseApi + this.apiUrl, { params: new HttpParams({ fromObject: val }) });
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

    excelServerExport(paged) {
        return this.http.get(this.baseApi + this.apiUrl + '/ExportExcelFile', { responseType: 'blob', params: paged });
    }

    getPartnerDisplaysByIds(ids: string[]): Observable<PartnerDisplay[]> {
        return this.http.post<PartnerDisplay[]>(this.baseApi + this.apiUrl + '/getPartnerDisplaysByIds', ids);
    }

    defaultGet(val?: any) {
        return this.http.post<PartnerDisplay[]>(this.baseApi + this.apiUrl + '/DefaultGet', val || {});
    }

    getNextAppointment(id): Observable<AppointmentDisplay> {
        return this.http.get<AppointmentDisplay>(this.baseApi + this.apiUrl + '/' + id + '/GetNextAppointment')
    }

    saveAvatar(data: any) {
        return this.http.post(this.baseApi + this.apiUrl + "/SaveAvatar", data);
    }

    addTags(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/AddTags', val);
    }

    removeTags(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/RemoveTags', val);
    }

    updateTags(val) {
        return this.http.post(this.baseApi + this.apiUrl + '/UpdateTags', val);
    }

    getPrint(id: string): Observable<PartnerPrint> {
        return this.http.get<PartnerPrint>(this.baseApi + this.apiUrl + `/${id}/Print`);
    }

    exportPartnerExcelFile(paged) {
        return this.http.post(
            this.baseApi + this.apiUrl + "/ExportExcelFile", paged,
            { responseType: "blob" }
        );
    }

    getReportLocationCompanyWard(val): Observable<PartnerReportLocationWard[]> {
        return this.http.post<PartnerReportLocationWard[]>(this.baseApi + this.apiUrl + '/ReportLocationCompanyWard', val);
    }

    getReportLocationCompanyDistrict(val): Observable<PartnerReportLocationDistrict[]> {
        return this.http.post<PartnerReportLocationDistrict[]>(this.baseApi + this.apiUrl + '/ReportLocationCompanyDistrict', val);
    }

    uploadPartnerImage(val): Observable<PartnerImageBasic[]> {
        return this.http.post<PartnerImageBasic[]>(this.baseApi + this.apiPartnerImage + '/BinaryUploadPartnerImage', val)
    }

    getPartnerImageIds(val): Observable<PartnerImageBasic[]> {
        return this.http.post<PartnerImageBasic[]>(this.baseApi + this.apiPartnerImage + '/SearchRead', val)
    }

    deleteParnerImage(id) {
        return this.http.delete(this.baseApi + this.apiPartnerImage + '/' + id);
    }

    getDefaultTitle(val) {
        return this.http.get(this.baseApi + this.apiUrl + '/GetDefaultTitle', { params: new HttpParams({ fromObject: val }) });
    }

    getPartnerCustomerReport(data: any) {
        return this.http.post<PartnerCustomerReportOutput>(this.baseApi + this.apiUrl + '/PartnerCustomerReport', data);
    }

    getCustomerStatistics(data: any) {
        return this.http.post<CustomerStatisticsOutput>(this.baseApi + this.apiUrl + '/CustomerStatistics', data);
    }

    getUnreconcileInvoices(id: string, search?: string) {
        if (search) {
            return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetUnreconcileInvoices', { params: new HttpParams({ fromObject: { search } }) });
        } else {
            return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetUnreconcileInvoices');
        }
    }

    UpdateActive(id, val): Observable<any> {
        return this.http.post(this.baseApi + "api/Partners/" + id + '/UpdateActive', val);
    }

    getDebtPaged(id: string, val: any) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/GetDebtPaged', { params: new HttpParams({ fromObject: val }) });
    }

    getAmountAdvanceBalance(id) {
        return this.http.get<number>(this.baseApi + this.apiUrl + '/' + id + '/GetAmountAdvanceBalance');
    }

    getAmountAdvanceUsed(id) {
        return this.http.get<number>(this.baseApi + this.apiUrl + '/' + id + '/GetAmountAdvanceUsed');
    }

    getCustomerInfo(id) {
        return this.http.get(this.baseApi + this.apiUrl + '/' + id + '/CustomerInfo');
    }

    checkUpdateLevel() {
        return this.http.post(this.baseApi + this.apiUrl + '/CheckUpdateLevel', {});
    }

    getCustomerBirthDay(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/GetCustomerBirthDay', val);
    }

    getCustomerAppointments(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/GetCustomerAppointments', val);
    }

    getAmountDebtBalance(id) {
        return this.http.get<number>(this.baseApi + this.apiUrl + '/' + id + '/GetAmountDebtBalance');
    }

    getPartnerInfoPaged(val) {
        return this.http.get<PagedResult2<PartnerInfoDisplay>>(this.baseApi + this.apiUrl + '/GetPartnerInfoPaged', { params: new HttpParams({ fromObject: val }) });
    }

    getPartnerInfoPaged2(val) {
        return this.http.get<PagedResult2<PartnerInfoDisplay>>(this.baseApi + this.apiUrl + '/GetPartnerInfoPaged2', { params: new HttpParams({ fromObject: val }) });
    }

    getListAttachment(id) {
        return this.http.get<IrAttachmentBasic[]>(this.baseApi + this.apiUrl + '/' + id + '/GetListAttachment');
    }

    createCustomer(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Customers', val);
    }

    updateCustomer(val: any) {
        return this.http.put(this.baseApi + this.apiUrl + '/Customers', val);
    }

    createSupplier(val: any) {
        return this.http.post(this.baseApi + this.apiUrl + '/Suppliers', val);
    }

    updateSupplier(val: any) {
        return this.http.put(this.baseApi + this.apiUrl + '/Suppliers', val);
    }

    getExist(val){
        return this.http.post<PartnerSimple[]>(this.baseApi + this.apiUrl + '/GetExist', val);
    }
}


