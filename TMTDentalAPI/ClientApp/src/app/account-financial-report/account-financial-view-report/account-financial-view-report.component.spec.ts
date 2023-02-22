import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountFinancialViewReportComponent } from './account-financial-view-report.component';

describe('AccountFinancialViewReportComponent', () => {
  let component: AccountFinancialViewReportComponent;
  let fixture: ComponentFixture<AccountFinancialViewReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AccountFinancialViewReportComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountFinancialViewReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
