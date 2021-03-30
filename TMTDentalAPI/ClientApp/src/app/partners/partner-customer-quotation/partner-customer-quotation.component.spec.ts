import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerQuotationComponent } from './partner-customer-quotation.component';

describe('PartnerCustomerQuotationComponent', () => {
  let component: PartnerCustomerQuotationComponent;
  let fixture: ComponentFixture<PartnerCustomerQuotationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerQuotationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerQuotationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
