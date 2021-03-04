//đây chính là product display
export class Product {
    id: string;
    name: string;
    categ: object;
    categId: string;
    uom: object;
    uomId: string;
    uompo: object;
    uompoId: string;
    type: string;
    saleOK: boolean;
    purchaseOK: boolean;
    keToaOK: boolean;
    isLabo: boolean;
}

export class ProductSave {
    name: string;
    categ: object;
    categId: string;
    uom: object;
    uomId: string;
    uompo: object;
    uompoId: string;
    type: string;
    saleOK: boolean;
    purchaseOK: boolean;
    keToaOK: boolean;
    isLabo: boolean;
    
}