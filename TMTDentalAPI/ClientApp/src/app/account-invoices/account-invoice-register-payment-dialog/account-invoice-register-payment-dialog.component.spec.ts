import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceRegisterPaymentDialogComponent } from './account-invoice-register-payment-dialog.component';

describe('AccountInvoiceRegisterPaymentDialogComponent', () => {
  let component: AccountInvoiceRegisterPaymentDialogComponent;
  let fixture: ComponentFixture<AccountInvoiceRegisterPaymentDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceRegisterPaymentDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceRegisterPaymentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
