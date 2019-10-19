import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountInvoiceReportIndexComponent } from './account-invoice-report-index.component';

describe('AccountInvoiceReportIndexComponent', () => {
  let component: AccountInvoiceReportIndexComponent;
  let fixture: ComponentFixture<AccountInvoiceReportIndexComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountInvoiceReportIndexComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountInvoiceReportIndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
