import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerQuotationFormComponent } from './partner-customer-quotation-form.component';

describe('PartnerCustomerQuotationFormComponent', () => {
  let component: PartnerCustomerQuotationFormComponent;
  let fixture: ComponentFixture<PartnerCustomerQuotationFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerQuotationFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerQuotationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
