import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountCommonCustomerReportListComponent } from './account-common-customer-report-list.component';

describe('AccountCommonCustomerReportListComponent', () => {
  let component: AccountCommonCustomerReportListComponent;
  let fixture: ComponentFixture<AccountCommonCustomerReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountCommonCustomerReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountCommonCustomerReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
