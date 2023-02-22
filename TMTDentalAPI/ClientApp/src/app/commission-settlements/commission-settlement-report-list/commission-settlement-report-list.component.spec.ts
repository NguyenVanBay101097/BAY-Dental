import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionSettlementReportListComponent } from './commission-settlement-report-list.component';

describe('CommissionSettlementReportListComponent', () => {
  let component: CommissionSettlementReportListComponent;
  let fixture: ComponentFixture<CommissionSettlementReportListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionSettlementReportListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionSettlementReportListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
