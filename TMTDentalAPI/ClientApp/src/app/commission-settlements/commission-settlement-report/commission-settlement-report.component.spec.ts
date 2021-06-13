import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementReportComponent } from './commission-settlement-report.component';

describe('CommissionSettlementReportComponent', () => {
  let component: CommissionSettlementReportComponent;
  let fixture: ComponentFixture<CommissionSettlementReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
