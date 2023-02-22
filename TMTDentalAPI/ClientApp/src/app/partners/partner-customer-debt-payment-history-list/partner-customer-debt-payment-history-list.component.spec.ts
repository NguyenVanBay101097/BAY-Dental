import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerDebtPaymentHistoryListComponent } from './partner-customer-debt-payment-history-list.component';

describe('PartnerCustomerDebtPaymentHistoryListComponent', () => {
  let component: PartnerCustomerDebtPaymentHistoryListComponent;
  let fixture: ComponentFixture<PartnerCustomerDebtPaymentHistoryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerDebtPaymentHistoryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerDebtPaymentHistoryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
