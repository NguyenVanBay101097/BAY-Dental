import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceLineDialogComponent } from './account-invoice-line-dialog.component';

describe('AccountInvoiceLineDialogComponent', () => {
  let component: AccountInvoiceLineDialogComponent;
  let fixture: ComponentFixture<AccountInvoiceLineDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceLineDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceLineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
