import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashBankTodayReportComponent } from './cash-bank-today-report.component';

describe('CashBankTodayReportComponent', () => {
  let component: CashBankTodayReportComponent;
  let fixture: ComponentFixture<CashBankTodayReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashBankTodayReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashBankTodayReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
