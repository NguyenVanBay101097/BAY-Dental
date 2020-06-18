import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerProductMedicineListComponent } from './partner-customer-product-medicine-list.component';

describe('PartnerCustomerProductMedicineListComponent', () => {
  let component: PartnerCustomerProductMedicineListComponent;
  let fixture: ComponentFixture<PartnerCustomerProductMedicineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerProductMedicineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerProductMedicineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
