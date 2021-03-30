import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerQuotationListComponent } from './partner-customer-quotation-list.component';

describe('PartnerCustomerQuotationListComponent', () => {
  let component: PartnerCustomerQuotationListComponent;
  let fixture: ComponentFixture<PartnerCustomerQuotationListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerQuotationListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerQuotationListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
