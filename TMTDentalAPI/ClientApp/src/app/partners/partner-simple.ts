import { EmployeeSimple } from '../employees/employee';
import { HistorySimple } from '../history/history';

export class PartnerSimple {
    id: string;
    name: string;
    displayName: string;
}

export class PartnerSimpleContact {
    id: string;
    name: string;
    phone: string;
}
export class PartnerBasic {
    id: string;
    name: string;
    phone: string;
    gender: string;
    ref: string;
    address: string;
    birthYear: number;
}
export class PartnerDisplay extends PartnerBasic {
    supplier: boolean;
    customer: boolean;
    comment: string;
    active: boolean;
    type: string;
    employee: boolean;
    employees: EmployeeSimple;
    barcode: string;
    medicalHistory: string;
    birthDay: number;
    birthMonth: number;
    categories: PartnerCategorySimple[];
    histories: HistorySimple[];
    jobTitle: string;
    fax: string;
    street: string;
    avatar: string;
    city: PDWSimple;
    district: PDWSimple;
    ward: PDWSimple;
    source: string;
    note: string;
    email: string;
    cityName: string;
    districtName: string;
    wardName: string;
    zaloId: string;
}

export class PartnerCategorySimple {
    id: string;
    name: string;
}

export class PartnerPaged {
    offset: number;
    limit: number;
    searchNamePhoneRef: string;
    customer: boolean;
    employee: boolean;
    supplier: boolean;
}

export class PagedResult2<T>{
    offset: number;
    limit: number;
    totalItems: number;
    items: T[];
}

export class TokenResult {
    access_token: string;
    expires_in: number;
    token_type: string;
}

export class TokenAccess {
    id: string;
    name: string;
}

export class City {
    code: string;
    name: string;
}

export class District {
    cityCode: string;
    cityName: string;
    code: string;
    name: string;
}

export class Ward {
    cityCode: string;
    cityName: string;
    districtCode: string;
    districtName: string;
    code: string;
    name: string;
}

export class AshipRequest {
    data: AshipData;
    provider: string = "Undefined";
}

export class AshipData {
    code: string;
}

export class PDWSimple {
    code: string;
    name: string;
}

// export class FormFile {
//     contentType: string;
//     contentDisposition: string;
//     length: number;
//     name: string;
//     fileName: string;
// }

export class PartnerInfoViewModel {
    name: string;
    ref: string;
    phone: string;
    email: string;
    address: string;
    categories: string;
    dateOfBirth: string;
    gender: string;
    medicalHistory: string;
    jobTitle: string;
}




