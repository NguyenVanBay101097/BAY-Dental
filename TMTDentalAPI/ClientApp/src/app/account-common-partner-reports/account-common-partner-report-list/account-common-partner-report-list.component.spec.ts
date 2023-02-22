import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountCommonPartnerReportListComponent } from './account-common-partner-report-list.component';

describe('AccountCommonPartnerReportListComponent', () => {
  let component: AccountCommonPartnerReportListComponent;
  let fixture: ComponentFixture<AccountCommonPartnerReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountCommonPartnerReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountCommonPartnerReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
