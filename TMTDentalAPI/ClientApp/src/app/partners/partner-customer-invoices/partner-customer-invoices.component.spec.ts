import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerInvoicesComponent } from './partner-customer-invoices.component';

describe('PartnerCustomerInvoicesComponent', () => {
  let component: PartnerCustomerInvoicesComponent;
  let fixture: ComponentFixture<PartnerCustomerInvoicesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerInvoicesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerInvoicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
