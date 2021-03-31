import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerQuotationCuComponent } from './partner-customer-quotation-cu.component';

describe('PartnerCustomerQuotationCuComponent', () => {
  let component: PartnerCustomerQuotationCuComponent;
  let fixture: ComponentFixture<PartnerCustomerQuotationCuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerQuotationCuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerQuotationCuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
