import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountCommonPartnerReportDetailComponent } from './account-common-partner-report-detail.component';

describe('AccountCommonPartnerReportDetailComponent', () => {
  let component: AccountCommonPartnerReportDetailComponent;
  let fixture: ComponentFixture<AccountCommonPartnerReportDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountCommonPartnerReportDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountCommonPartnerReportDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
