import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceListComponent } from './account-invoice-list.component';

describe('AccountInvoiceListComponent', () => {
  let component: AccountInvoiceListComponent;
  let fixture: ComponentFixture<AccountInvoiceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
