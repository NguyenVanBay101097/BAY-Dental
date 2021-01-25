import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerSupplierFormPaymentComponent } from './partner-supplier-form-payment.component';

describe('PartnerSupplierFormPaymentComponent', () => {
  let component: PartnerSupplierFormPaymentComponent;
  let fixture: ComponentFixture<PartnerSupplierFormPaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerSupplierFormPaymentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerSupplierFormPaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
